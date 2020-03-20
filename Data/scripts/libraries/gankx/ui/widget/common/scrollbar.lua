local UIScrollBarExport = Gankx.UI.ExportWrapper(CS.UIScrollbarExport)
local UISelectableExport = Gankx.UI.ExportWrapper(CS.UISelectableExport)

widget("Scrollbar", "Window")

function setInteractable(self, interactable)
    UISelectableExport:setBool(self, "Interactable", interactable)
end

function getInteractable(self)
    return UISelectableExport:getBool(self, "Interactable", false)
end

function setValue(self, value)
    UIScrollBarExport:setNumber(self.id, "Value", value)
end

function getValue(self)
    return UIScrollBarExport:getNumber(self.id, "Value")
end