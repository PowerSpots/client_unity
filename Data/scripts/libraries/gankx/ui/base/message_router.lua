module("Gankx.UI.MessageRouter", package.seeall)

local MessageRouterMeta = {}

function MessageRouterMeta:getWidgetCount()
    local widgetHandles = self.__handles

    local count = 0

    for k1, v1 in pairs(widgetHandles) do
        if v1 then
            for k2, v2 in pairs(v1) do
                count = count + 1
            end
        end
    end

    return count
end

function MessageRouterMeta:attachHandle(windowId, msgName, widget)
    local widgetHandles = self.__handles

    if nil == widgetHandles[windowId] then
        widgetHandles[windowId] = {}
    end

    widgetHandles[windowId][msgName] = widget
end

function MessageRouterMeta:detachHandle(windowId, msgName)
    local widgetHandles = self.__handles

    if nil == widgetHandles[windowId] then
        return
    end

    widgetHandles[windowId][msgName] = nil

    if next(widgetHandles[windowId]) == nil then
        widgetHandles[windowId] = nil
    end
end

function MessageRouterMeta:attachGlobeHandle(control)
    self.__globeHandle = control
end

function MessageRouterMeta:handleMessage(id, msgName, ...)
    local widgetHandles = self.__handles

    if id ~= nil
            and widgetHandles[id] ~= nil
            and widgetHandles[id][msgName] ~= nil then

        widgetHandles[id][msgName](id, msgName, ...)

        return true
    end

    return false
end

function MessageRouterMeta:routeMessage(id, msgName, ...)
    if nil == id or nil == msgName then
        return
    end

    if self:handleMessage(id, msgName, ...) then
        return
    end

    if self.__globeHandle ~= nil and
            self.__globeHandle(id, msgName, ...) then
        return
    end
end

MessageRouterMeta.__index = MessageRouterMeta

function create()
    local object = {}
    object.__handles = {}
    object.__globeHandle = nil
    setmetatable(object, MessageRouterMeta)
    return object
end
