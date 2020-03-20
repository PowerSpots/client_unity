class("EventService")

local weakMetaTable = { __mode = "k" }

function constructor(self)
    self._allListeners = {}
end

function _error(self, title, msg)
    Console.error("EventService." .. title ..
            " occurred error: " .. tostring(msg))
end

function _addListener(self, name, listener)
    local listeners = self._allListeners[name]
    if nil == listeners then
        self._allListeners[name] = { listener }
        return
    end

    SteadyArray.add(listeners, listener)
end

function _removeListener(self, name, listener)
    local listeners = self._allListeners[name]
    if nil == listeners then
        return
    end

    SteadyArray.remove(listeners, listener)
end

local function _doFireEvent(listener, name, ...)
    listener:_fireEvent(name, ...)
end

function fireEvent(self, name, ...)
    local listeners = self._allListeners[name]
    if nil == listeners then
        return
    end

    SteadyArray.foreach(listeners, _doFireEvent, name, ...)
end

function listenEvent(self, name, context, method)
    if nil == context or nil == name then
        self:_error("listenEvent", debug.traceback("参数错误"))
        return
    end

    local contextListeners = context.__eventListeners
    if nil == contextListeners then
        contextListeners = {}
        setmetatable(contextListeners, weakMetaTable)
        context.__eventListeners = contextListeners
    end

    local contextListener = contextListeners[self]
    if nil == contextListener then
        contextListener = EventListener:new(context)
        contextListeners[self] = contextListener
    end

    contextListener:_listenEvent(self, name, method)
end

function unlistenEvent(self, name, context)
    if nil == context or nil == name then
        self:_error("unlistenEvent", debug.traceback("参数错误"))
        return
    end

    local contextListeners = context.__eventListeners
    if nil == contextListeners then
        return
    end

    local contextListener = contextListeners[self]
    if nil == contextListener then
        return
    end

    contextListener:_unlistenEvent(self, name)
end

function unlistenAllEvents(self, context)
    if nil == context then
        self:_error("unlistenAllEvents", debug.traceback("参数错误"))
        return
    end

    local contextListeners = context.__eventListeners
    if nil == contextListeners then
        return
    end

    local contextListener = contextListeners[self]
    if nil == contextListener then
        return
    end

    contextListener:_unlistenAllEvents(self)
end

function clear(self, context)
    if nil == context then
        self:_error("clear", debug.traceback("参数错误"))
        return
    end

    local contextListeners = context.__eventListeners
    if nil == contextListeners then
        return
    end

    local contextListener = contextListeners[self]
    if nil == contextListener then
        return
    end

    contextListener:_unlistenAllEvents(self)
    contextListener:_destroy(self)
end

local function _doDestroy(listener, service)
    listener:_destroy(service)
end

function clearAll(self)
    local allListeners = self._allListeners
    for k, listeners in pairs(allListeners) do
        SteadyArray.clear(listeners, _doDestroy, self)
    end
end

function destroy(self)
    self:clearAll()
    self._allListeners = nil
end