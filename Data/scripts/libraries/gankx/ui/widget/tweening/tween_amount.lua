local UITweenAmountExport = CS.UITweenAmountExport

widget("TweenAmount", "Tweener")

function _getTweenerExport(self)
    return UITweenAmountExport
end

function reset(self, duration, from, to)
    UITweenAmountExport.Reset(self.id, duration, from, to)
end

function setFromTo(self, from, to)
    UITweenAmountExport.SetFromTo(self.id, from, to)
end

function setTo(self, to)
    UITweenAmountExport.SetTo(self.id, to)
end

function setFrom(self, from)
    UITweenAmountExport.SetFrom(self.id, from)
end

function setValue(self, value)
    UITweenAmountExport.SetValue(self.id, value)
end

function getTo(self)
    return UITweenAmountExport.GetTo(self.id)
end

function getFrom(self)
    return UITweenAmountExport.GetFrom(self.id)
end

function getValue(self)
    return UITweenAmountExport.GetValue(self.id)
end
