local Coroutine = Gankx.Coroutine

class("Component")

function constructor(self)
    self.gameObject = nil
    self._enable = true
end

function error(self, title, msg)
    Console.error(self.__classname .. "." .. title ..
            " on [" .. self.gameObject:toString() .. "]" ..
            " occurred error: " .. tostring(msg), 2)
end

function warning(self, title, msg)
    Console.warning(self.__classname .. "." .. title ..
            " on [" .. self.gameObject:toString() .. "]" ..
            " occurred warning: " .. tostring(msg), 2)
end

function info(self, title, msg)
    Console.info(self.__classname .. "." .. title ..
            " on [" .. self.gameObject:toString() .. "]" ..
            " occurred info: " .. tostring(msg), 2)
end

function toString(self)
    return self.gameObject:toString() .. ":Component(" .. self.__classname .. ")"
end

function getSummaryInfo(self, sb, prefix)
    prefix = prefix or ""
    local subPrefix = prefix .. "    "
    sb:append(prefix .. "- " .. self.__classname .. ":")
    sb:appendLine(" enable(" .. tostring(self._enable) .. ")")
end

function errorHandle(err)
    return debug.traceback(err)
end

function _receiveMessage(self, methodName, ...)
    if nil == methodName or string.len(methodName) == 0 then
        self:error("onMessage", debug.traceback())
        return
    end

    local method = self[methodName]
    if method ~= nil then
        local bOk, err = xpcall(method, errorHandle, self, ...)
        if not bOk then
            self:error("on message '" .. methodName .. "'", err)
        end
    end
end

function _awake(self, gameObject)
    self.gameObject = gameObject
    self:_updateEnabled(self:getEnable())
    self:_receiveMessage("awake")
end

function _destroy(self)
    self:_receiveMessage("onDestroy")
    self:unlistenAll()
    Coroutine.stopAll(self)
    self:setEnable(false)
    self.gameObject = nil

    setmetatable(self, nil)
end

function _updateEnabled(self, enabled)
    if enabled then
        ComponentManager.addUpdate(self)
        self:_receiveMessage("onEnable")
    else
        self:_receiveMessage("onDisable")
        ComponentManager.removeUpdate(self)
    end
end

function _updateActiveInHierarchy(self, parentActiveInHierarchy)
    if not self._enable then
        return
    end

    self:_updateEnabled(parentActiveInHierarchy)
end

function setEnable(self, value)
    if self._enable == value then
        return
    end

    self._enable = value

    if self.gameObject:getActiveInHierarchy() then
        self:_updateEnabled(self._enable)
    end
end

function getEnable(self)
    return self._enable and self.gameObject:getActiveInHierarchy()
end

function getComponent(self, componentType)
    return self.gameObject:getComponent(componentType)
end

function getComponents(self, componentType)
    return self.gameObject:getComponents(componentType)
end

function sendMessage(self, methodName, ...)
    return self.gameObject:sendMessage(methodName, ...)
end

function cancelinvoke(self, method)
    Event.unlistenAll(self, method)
end

function invoke(self, method, time)
    Event.listenTimer(Timer.once(time), self, method)
end

function invokeRepeating(self, method, time, repeatRate)
    Event.listenTimer(Timer.always(repeatRate, time), self, method)
end

function isInvoking(self, method)
    return Event.isListeningAll(self, method)
end

function startCoroutine(self, method, ...)
    Coroutine.start(self, method, ...)
end

function stopCoroutine(self, method)
    Coroutine.stop(self, method)
end

function stopAllCoroutines(self)
    Coroutine.stopAll(self)
end

function listenEvent(self, eventName, method)
    Event.listen(eventName, self, method)
end

function unlistenEvent(self, eventName)
    Event.unlisten(eventName, self)
end

function unlistenAll(self)
    Event.unlistenAll(self)
end
