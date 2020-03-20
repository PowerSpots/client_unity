local UITweenRotationExport = CS.UITweenRotationExport

widget("TweenRotation", "Tweener")

function _getTweenerExport(self)
    return UITweenRotationExport
end

function reset(self, duration, xFrom, yFrom, zFrom, xTo, yTo, zTo)
    UITweenRotationExport.Reset(self.id, duration, xFrom, yFrom, zFrom, xTo, yTo, zTo)
end

function setFromTo(self, xFrom, yFrom, zFrom, xTo, yTo, zTo)
    UITweenRotationExport.SetFromTo(self.id, xFrom, yFrom, zFrom, xTo, yTo, zTo)
end

function setTo(self, x, y, z)
    UITweenRotationExport.SetTo(self.id, x, y, z)
end

function setFrom(self, x, y, z)
    UITweenRotationExport.SetFrom(self.id, x, y, z)
end

function getXFrom(self)
    return UITweenRotationExport.GetXFrom(self.id)
end

function getYFrom(self)
    return UITweenRotationExport.GetYFrom(self.id)
end

function getZFrom(self)
    return UITweenRotationExport.GetZFrom(self.id)
end

function getXTo(self)
    return UITweenRotationExport.GetXTo(self.id)
end

function getYTo(self)
    return UITweenRotationExport.GetYTo(self.id)
end

function getZTo(self)
    return UITweenRotationExport.getZTo(self.id)
end

