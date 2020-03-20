local UITweenScaleExport = CS.UITweenScaleExport

widget("TweenScale", "Tweener")


function _getTweenerExport(self)
    return UITweenScaleExport
end

function playOnOnce(self)
    UITweenScaleExport.PlayOnOnce(self.id)
end

function reset(self, duration, xFrom, yFrom, zFrom, xTo, yTo, zTo)
    UITweenScaleExport.Reset(self.id, duration, xFrom, yFrom, zFrom, xTo, yTo, zTo)
end

function setFromTo(self, xFrom, yFrom, zFrom, xTo, yTo, zTo)
    UITweenScaleExport.SetFromTo(self.id, xFrom, yFrom, zFrom, xTo, yTo, zTo)
end

function setTo(self, x, y, z)
    UITweenScaleExport.SetTo(self.id, x, y, z)
end

function setFrom(self, x, y, z)
    UITweenScaleExport.SetFrom(self.id, x, y, z)
end

function getXFrom(self)
    return UITweenScaleExport.GetXFrom(self.id)
end

function getYFrom(self)
    return UITweenScaleExport.GetYFrom(self.id)
end

function getZFrom(self)
    return UITweenScaleExport.GetZFrom(self.id)
end

function getXTo(self)
    return UITweenScaleExport.GetXTo(self.id)
end

function getYTo(self)
    return UITweenScaleExport.GetYTo(self.id)
end

function getZTo(self)
    return UITweenScaleExport.GetZTo(self.id)
end




