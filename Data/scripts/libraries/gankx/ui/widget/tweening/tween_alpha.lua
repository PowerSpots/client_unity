local UITweenAlphaExport = CS.UITweenAlphaExport

widget("TweenAlpha", "Tweener")

function _getTweenerExport(self)
    return UITweenAlphaExport
end

function reset(self, duration, from, to)
    UITweenAlphaExport.Reset(self.id, duration, from, to)
end

function playOnOnce(self)
    UITweenAlphaExport.PlayOnOnce(self.id)
end

function setFromTo(self, from, to)
    UITweenAlphaExport.SetFromTo(self.id, from, to)
end

function setTo(self, to)
    UITweenAlphaExport.SetTo(self.id, to)
end

function setFrom(self, from)
    return UITweenAlphaExport.SetFrom(self.id, from)
end

function getTo(self)
    return UITweenAlphaExport.GetTo(self.id)
end

function getFrom(self)
    return UITweenAlphaExport.GetFrom(self.id)
end
