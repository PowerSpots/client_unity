local UITweenScrollRectExport = CS.UITweenScrollRectExport

widget("TweenScrollRect", "Tweener")

function setToX(self, to)
    UITweenScrollRectExport.SetToX(self.id, to)
end

function setFromX(self, from)
    UITweenScrollRectExport.SetFromX(self.id, from)
end

function setToY(self, to)
    UITweenScrollRectExport.SetToY(self.id, to)
end

function setFromY(self, from)
    UITweenScrollRectExport.SetFromY(self.id, from)
end

function setFromToX(self, from, to)
    UITweenScrollRectExport.SetFromToX(self.id, from, to)
end

function setFromToY(self, from, to)
    UITweenScrollRectExport.SetFromToY(self.id, from, to)
end

function getFromX(self)
    UITweenScrollRectExport.GetFromX(self.id)
end

function getToX(self)
    UITweenScrollRectExport.GetToX(self.id)
end

function getFromY(self)
    UITweenScrollRectExport.GetFromY(self.id)
end

function getToY(self)
    UITweenScrollRectExport.GetToY(self.id)
end