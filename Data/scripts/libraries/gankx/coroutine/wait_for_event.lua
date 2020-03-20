module("Gankx", package.seeall)

class("WaitForEvent", "AsyncOperation")

function constructor(self, eventName, eventHandler, eventContext)
    __super.constructor(self)

    self._eventName = eventName
    self._eventContext = eventContext
    self._eventHandler = eventHandler
end

function onYield(self)
    Console.Info("Start waitting for event:" .. tostring(self._eventName))
    Event.listen(self._eventName, self, onEvent)
end

function onEvent(self, ...)
    local eventContext = self._eventContext
    local eventHandler = self._eventHandler
    if nil == eventHandler then
        self._isDone = true
        self._progress = 1
        return
    end

    if self._isDone then
        return
    end

    local bOk, result
    if eventContext ~= nil then
        bOk, result = pcall(eventHandler, eventContext, ...)
    else
        bOk, result = pcall(eventHandler, ...)
    end

    if not bOk then
        Console.error("WaitForEvent(" .. tostring(self._eventName) .. ") occurred error:" .. tostring(result))
    end

    self._isDone = result == true

    if self._isDone then
        self._progress = 1
    end
end

function onResume(self)
    Event.unlistenAll(self)
    self._eventName = nil
    self._eventContext = nil
    self._eventHandler = nil
end


