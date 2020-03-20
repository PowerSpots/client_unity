local UITweenMaterialFloatExport = CS.UITweenMaterialFloatExport

widget("TweenMaterialFloat", "Tweener")

function _getTweenerExport(self)
    return UITweenMaterialFloatExport
end

function reset(self, duration, from, to, key)
    UITweenMaterialFloatExport.Reset(self.id, duration, from, to, key)
end

function setFromTo(self, from, to, key)
    UITweenMaterialFloatExport.SetFromTo(self.id, from, to, key)
end

function setTo(self, to)
    UITweenMaterialFloatExport.SetTo(self.id, to)
end

function setFrom(self, from)
    return UITweenMaterialFloatExport.SetFrom(self.id, from)
end

function play(self)
    return UITweenMaterialFloatExport.Play(self.id)
end