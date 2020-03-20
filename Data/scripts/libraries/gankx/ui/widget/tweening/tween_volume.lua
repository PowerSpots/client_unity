local UITweenVolumeExport = CS.UITweenVolumeExport

widget("TweenVolume", "Tweener")

function _getTweenerExport(self)
    return UITweenVolumeExport
end

function reset(self, duration, from, to)
    UITweenVolumeExport.Reset(self.id, duration, from, to)
end

function setFromTo(self, from, to)
    UITweenVolumeExport.SetFromTo(self.id, from, to)
end

function setTo(self, to)
    UITweenVolumeExport.SetTo(self.id, to)
end

function setFrom(self, from)
    UITweenVolumeExport.SetFrom(self.id, from)
end

function getTo(self)
    return UITweenVolumeExport.GetTo(self.id)
end

function getFrom(self)
    return UITweenVolumeExport.GetFrom(self.id)
end
