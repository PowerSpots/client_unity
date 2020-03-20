local UITextExport = Gankx.UI.ExportWrapper(CS.UITextExport)

widget("Text", "Window")

function getText(self)
    return UITextExport:getVector(self, "Text", "")
end

function setText(self, text, isRecursive)
    UITextExport:setVector(self, "Text", text, isRecursive == true)
end

function getFontSize(self)
    return UITextExport:getNumber(self, "FontSize")
end

function setFontSize(self, size)
    UITextExport:setNumber(self, "FontSize", size)
end

function getFontStyle(self)
    return UITextExport:getValue(self, "FontStyle", FontStyle.Normal)
end

function setFontStyle(self, style)
    if style ~= FontStyle.Normal and
            style ~= FontStyle.Bold and
            style ~= FontStyle.Italic and
            style ~= FontStyle.BoldAndItalic then
        self:__error("setFontStyle", "invalid style, should be FontStyle.Normal,FontStyle.Bold,FontStyle.Italic,FontStyle.BoldAndItalic")
        return
    end

    UITextExport:setValue(self, "FontStyle", style)
end

function getAlignment(self)
    return UITextExport:getValue(self, "Alignment", GUIText.Alignment.Automatic)
end

function setAlignment(self, alignment)
    if alignment ~= GUIText.Alignment.Automatic and
            alignment ~= GUIText.Alignment.Left and
            alignment ~= GUIText.Alignment.Center and
            alignment ~= GUIText.Alignment.Right and
            alignment ~= GUIText.Alignment.Justified then
        self:__error("setAlignment", "invalid alignment, " ..
                "should be GUIText.Alignment.Automatic,GUIText.Alignment.Left,GUIText.Alignment.Center,GUIText.Alignment.Right,GUIText.Alignment.Justified")
        return
    end

    UITextExport:setValue(self, "Alignment", alignment)
end

function setSpacingX(self, spacingX)
    UITextExport:setNumber(self, "SpacingX", spacingX)
end

function getSpacingX(self)
    return UITextExport:getNumber(self, "SpacingX")
end

function setSpacingY(self, spacingY)
    UITextExport:setNumber(self, "SpacingY", spacingY)
end

function getSpacingY(self)
    return UITextExport:getNumber(self, "SpacingY")
end

function setMultiLine(self, multiLine)
    UITextExport:setBool(self, "MultiLine", multiLine)
end

function getMultiLine(self)
    return UITextExport:getBool(self, "MultiLine")
end

function setOverflowMethod(self, overFlow)
    UITextExport:setNumber(self, "OverflowMethod", overFlow)
end

function getOverflowMethod(self)
    return UITextExport:getNumber(self, "OverflowMethod")
end

function setLayoutHorizontal(self)
    UITextExport.interface.SetLayoutHorizontal(self.id)
end

function setLayoutVertical(self)
    UITextExport.interface.SetLayoutVertical(self.id)
end

function getPreferredHeight(self)
    return UITextExport.interface.GetPreferredHeight(self.id)
end

function getPreferredWidth(self)
    return UITextExport.interface.GetPreferredWidth(self.id)
end
