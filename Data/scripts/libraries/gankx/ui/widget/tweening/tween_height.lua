widget("TweenHeight", "Tweener")

local UITweenHeightExport = CS.UITweenHeightExport

function _getTweenerExport(self)
    return UITweenHeightExport
end

function reset(self, duration, from, to)
    UITweenHeightExport.Reset(self.id, duration, from, to)
end

function setFromTo(self, from, to)
    UITweenHeightExport.SetFromTo(self.id, from, to)
end

function setTo(self, to)
    UITweenHeightExport.SetTo(self.id, to)
end

function setFrom(self, from)
    UITweenHeightExport.SetFrom(self.id, from)
end

function getTo(self)
    return UITweenHeightExport.GetTo(self.id)
end

function getFrom(self)
    return UITweenHeightExport.GetFrom(self.id)
end




