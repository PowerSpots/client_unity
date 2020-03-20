local UITweenWidthExport = CS.UITweenWidthExport

widget("TweenWidth", "Tweener")

function _getTweenerExport(self)
    return UITweenWidthExport
end

function reset(self, duration, from, to)
    UITweenWidthExport.Reset(self.id, duration, from, to)
end

function setFromTo(self, from, to)
    UITweenWidthExport.SetFromTo(self.id, from, to)
end

function setTo(self, to)
    UITweenWidthExport.SetTo(self.id, to)
end

function setFrom(self, from)
    UITweenWidthExport.SetFrom(self.id, from)
end

function getTo(self)
    return UITweenWidthExport.GetTo(self.id)
end

function getFrom(self)
    return UITweenWidthExport.GetFrom(self.id)
end