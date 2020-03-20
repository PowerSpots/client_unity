local UISliderExport = Gankx.UI.ExportWrapper(CS.UISliderExport)
local UISelectableExport = Gankx.UI.ExportWrapper(CS.UISelectableExport)

widget("Slider", "Window")

function setInteractable(self, interactable)
    UISelectableExport:setBool(self, "Interactable", interactable)
end

function getInteractable(self)
    return UISelectableExport:getBool(self, "Interactable", false)
end

function getValue(self)
    return UISliderExport:getNumber(self, "Value")
end

function setValue(self, value)
    UISliderExport:setNumber(self, "Value", value)
end

function SetEventEnable(self , enable)
    UISliderExport:setBool(self, "EventEnable", enable)
end
