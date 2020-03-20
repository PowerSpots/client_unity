local UIWindowExport = CS.UIWindowExport

service("WidgetEventService")

local function initPredefinedEvents(self)
    local predefinedEvents = UIWindowExport.GetPredefinedEvents()
    for _, eventName in ipairs(predefinedEvents) do
        self._predefinedEvents[eventName] = true
    end
end

function startup(self)
    self._predefinedEvents = {}

    initPredefinedEvents(self)
end

function bindEvent(self, windowId, eventName)
    if nil == eventName then
        return
    end

    if not self._predefinedEvents[eventName] then
        return
    end

    UIWindowExport.BindEvent(windowId, eventName)
end