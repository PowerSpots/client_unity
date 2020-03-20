local UISelectableExport = Gankx.UI.ExportWrapper(CS.UISelectableExport)
local UIButtonExport = Gankx.UI.ExportWrapper(CS.UIButtonExport)

widget("Button", "Window")

function setInteractable(self, interactable)
    UISelectableExport:setBool(self, "Interactable", interactable)
end

function getInteractable(self)
    return UISelectableExport:getBool(self, "Interactable", false)
end

function setCellData(self, cellData)
    self:setText(cellData)
end

function setText(self, txt)
    if self.children.text then
        self.children.text.Text = txt
    end
end

function getText(self)
    if self.children.text then
        return self.children.text.Text
    end
end

function setImage(self, path)
    if self.children.image then
        self.children.image.Path = path
    end
end

function setRed(self, state)
    if self.children.red then
        self.children.red.Visible = state
    end
end

function setGray(self, state)
    UIButtonExport:setBool(self, "Gray", state)
end

function setButtonInteractable(self, interactable)
    self:setInteractable(interactable)
    self:setGray(not interactable)
end

function getButtonInteractable(self)
    return self:getInteractable()
end

WidgetDefine = {
    widgets = {
        {
            type = "Text",
            path = "Text",
            var = "text",
            required = false,
        },

        {
            type = "Image",
            path = "Image",
            var = "image",
            required = false,
        },

        {
            type = "Image",
            path = "Red",
            var = "red",
            required = false,
        },
    }
}
