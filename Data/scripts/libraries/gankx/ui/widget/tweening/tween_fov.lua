local UITweenFOVExport = CS.UITweenFOVExport

widget("TweenFOV", "Tweener")

function _getTweenerExport(self)
    return UITweenFOVExport
end

function reset(self, duration, from, to)
    UITweenFOVExport.Reset(self.id, duration, from, to)
end

function setFromTo(self, from, to)
    UITweenFOVExport.SetFromTo(self.id, from, to)
end

function setTo(self, to)
    UITweenFOVExport.SetTo(self.id, to)
end

function setFrom(self, from)
    UITweenFOVExport.SetFrom(self.id, from)
end

function getTo(self)
    return UITweenFOVExport.GetTo(self.id)
end

function getFrom(self)
    return UITweenFOVExport.GetFrom(self.id)
end
