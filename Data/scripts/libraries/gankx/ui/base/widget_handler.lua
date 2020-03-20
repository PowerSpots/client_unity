module("Gankx.UI.Widget", package.seeall)

local WidgetHandlerMeta = {}

WidgetHandlerMeta.__call = function(self, ...)
    return self.func(self.widget, ...)
end

function createHandler(widget, func)
    if nil == func then
        return
    end

    local handler = {}
    handler.func = func
    handler.widget = widget
    setmetatable(handler, WidgetHandlerMeta)
    return handler
end



