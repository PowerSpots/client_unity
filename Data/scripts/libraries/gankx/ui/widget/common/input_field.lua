local UIInputFiedlExport = Gankx.UI.ExportWrapper(CS.UIInputFieldExport)
local UISelectableExport = Gankx.UI.ExportWrapper(CS.UISelectableExport)
local UIInputValidateExport = Gankx.UI.ExportWrapper(CS.UITextFieldValidateExport)

widget("InputField", "Window")

function setInteractable(self, interactable)
    UISelectableExport:setBool(self, "Interactable", interactable)
end

function getInteractable(self)
    return UISelectableExport:getBool(self, "Interactable", false)
end

function getText(self)
    return UIInputFiedlExport:getString(self, "Text")
end

function setText(self, text)
    UIInputFiedlExport:setString(self, "Text", text)
end

function setSafeText(self,text)
    UIInputFiedlExport:setString(self, "SafeText", text)
end

function setPlaceHolderText(self,text)
    UIInputFiedlExport:setString(self, "PlaceHolderText", text)
end

function setCharacterLimit(self, num)
    UIInputFiedlExport:setNumber(self, "CharacterLimit", num)
end

function setLimitLength(self, length)
    UIInputFiedlExport:setNumber(self, "CharacterLimit", 0)
    UIInputValidateExport:setNumber(self, "LimitLength", length)
end

function getTextValidLength(self)
    local text = self:getText()
    return UIInputValidateExport.interface.GetStringLength(text)
end

function getValidLength(self, text)
    return UIInputValidateExport.interface.GetStringLength(text)
end

function getUnicodeCategory(text)
    return UIInputValidateExport.interface.GetUnicodeCategory(text)
end


