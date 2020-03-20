class("EventListener")

function constructor(self, context)
    self._context = context or self
    self._slots = {}
end

function _error(self, title, msg)
    Console.error("EventListener." .. title ..
            " occurred error: " .. tostring(msg))
end

function _listenEvent(self, service, name, method)
    local context = self._context

    if type(method) == "string" then
        method = context[method]
    end

    if type(method) ~= "function" then
        self:_error("listenEvent", debug.traceback("绑定的方法不是一个可执行函数"))
        return false
    end

    local slots = self._slots
    if slots[name] ~= nil then
        self:_error("listenEvent", debug.traceback("重复监听事件：" .. name))
        return false
    end

    slots[name] = method
    service:_addListener(name, self)
    return true
end

function _unlistenEvent(self, service, name)
    local slots = self._slots
    if nil == slots[name] then
        return
    end

    slots[name] = nil
    service:_removeListener(name, self)
end

function _unlistenAllEvents(self, service)
    local slots = self._slots
    for k, v in pairs(slots) do
        slots[k] = nil
        service:_removeListener(k, self)
    end
end

function _destroy(self, service)
    local context = self._context
    if nil == context then
        return
    end

    local contextListeners = context.__eventListeners
    contextListeners[service] = nil
    self._context = nil
    self._slots = nil
end

local function slotErrorReplace()
end

function _fireEvent(self, name, ...)
    local slots = self._slots
    local slot = slots[name]
    if nil == slot then
        self:_error("_onEvent", debug.traceback("触发的事件(" .. name .. ")尚未监听"))
        return
    end

    local bOk, err = pcall(slot, self._context, ...)
    if not bOk then
        slots[name] = slotErrorReplace
        self:_error("_onEvent", err)
    end
end
