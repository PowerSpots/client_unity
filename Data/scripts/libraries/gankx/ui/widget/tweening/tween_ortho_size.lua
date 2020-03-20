local UITweenOrthoSizeExport = CS.UITweenOrthoSizeExport

widget("TweenOrthoSize", "Tweener")

function _getTweenerExport(self)
    return UITweenOrthoSizeExport
end

function reset(self, duration, from, to)
    UITweenOrthoSizeExport.Reset(self.id, duration, from, to)
end

function setFromTo(self, from, to)
    UITweenOrthoSizeExport.SetFromTo(self.id, from, to)
end

function setTo(self, to)
    UITweenOrthoSizeExport.SetTo(self.id, to)
end

function setFrom(self, from)
    UITweenOrthoSizeExport.SetFrom(self.id, from)
end

function getTo(self)
    return UITweenOrthoSizeExport.GetTo(self.id)
end

function getFrom(self)
    return UITweenOrthoSizeExport.GetFrom(self.id)
end

