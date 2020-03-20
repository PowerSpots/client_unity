local UIToggleExport = Gankx.UI.ExportWrapper(CS.UIToggleExport)
local UISelectableExport = Gankx.UI.ExportWrapper(CS.UISelectableExport)

widget("Toggle", "Window")

function setInteractable(self, interactable)
    UISelectableExport:setBool(self, "Interactable", interactable)
end

function getInteractable(self)
    return UISelectableExport:getBool(self, "Interactable", false)
end

function setValue(self, value)
    UIToggleExport:setBool(self, "Value", value)
end

function getValue(self)
    return UIToggleExport:getBool(self, "Value")
end

function resetToggleGroup(self)
    UIToggleExport.interface.resetToggleGroup(self.id)
end

function setRedPoint(self, state)
    self.children.redPoint.Visible = state
end

function setText(self,text)
    self.children.label.Text = text
end

function setIndex(self,index)
    self.context._index = index
end
function getIndex(self)
    return self.context._index
end

WidgetDefine =
{
    widgets =
    {
        {
            path = "RedPoint",
            type = "Window",
            var = "redPoint",
            required = false,
        },

        {
            path = "Text",
            type = "Text",
            var = "label",
            required = false,
        },
    }
}