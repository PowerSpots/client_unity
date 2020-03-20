local UIDropdownExport = Gankx.UI.ExportWrapper(CS.UIDropdownExport)
local UISelectableExport = Gankx.UI.ExportWrapper(CS.UISelectableExport)

widget("Dropdown", "Window")

function setInteractable(self, interactable)
    UISelectableExport:setBool(self, "Interactable", interactable)
end

function getInteractable(self)
    return UISelectableExport:getBool(self, "Interactable", false)
end

function setValue(self, value)
    UIDropdownExport:setNumber(self, "Value", value)
end

function getValue(self)
    return UIDropdownExport:getNumber(self, "Value")
end

function setText(self, text)
    UIDropdownExport:setString(self, "Text", text)
end

function getText(self)
    return UIDropdownExport:getString(self, "Text")
end

function addOptions(self, options)
    UIDropdownExport:addValue(self, "Options", options)
end

function clearOptions(self)
    UIDropdownExport:clearValue(self, "Options")
end

function refreshShownValue(self)
    UIDropdownExport.interface.RefreshShownValue(self.id)
end

function show(self)
    UIDropdownExport.interface.Show(self.id)
end

function hide(self)
    UIDropdownExport.interface.Hide(self.id)
end
