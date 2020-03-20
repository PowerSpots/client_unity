local UITweenPositionExport = CS.UITweenPositionExport

widget("TweenPosition", "Tweener")

function _getTweenerExport(self)
    return UITweenPositionExport
end

function reset(self, duration, xFrom, yFrom, zFrom, xTo, yTo, zTo)
    UITweenPositionExport.Reset(self.id, duration, xFrom, yFrom, zFrom, xTo, yTo, zTo)
end

function setFromTo(self, xFrom, yFrom, zFrom, xTo, yTo, zTo)
    UITweenPositionExport.SetFromTo(self.id, xFrom, yFrom, zFrom, xTo, yTo, zTo)
end

function setTo(self, x, y, z)
    UITweenPositionExport.SetTo(self.id, x, y, z)
end

function setFrom(self, x, y, z)
    UITweenPositionExport.SetFrom(self.id, x, y, z)
end

function getXFrom(self)
    UITweenPositionExport.GetXFrom(self.id)
end

function getYFrom(self)
    UITweenPositionExport.GetYFrom(self.id)
end

function getZFrom(self)
    UITweenPositionExport.GetZFrom(self.id)
end

function getXTo(self)
    UITweenPositionExport.GetXTo(self.id)
end

function getYTo(self)
    UITweenPositionExport.GetYTo(self.id)
end

function getZTo(self)
    UITweenPositionExport.getZTo(self.id)
end

function SetXFrom(self, xFrom)
    UITweenPositionExport.SetXFrom(self.id, xFrom)
end

function SetYFrom(self, yFrom)
    UITweenPositionExport.SetYFrom(self.id, yFrom)
end

function SetZFrom(self, zFrom)
    UITweenPositionExport.SetZFrom(self.id, zFrom)
end

function SetXTo(self, xTo)
    UITweenPositionExport.SetXTo(self.id, xTo)
end

function SetYTo(self, yTo)
    UITweenPositionExport.SetYTo(self.id, yTo)
end

function SetZTo(self, zTo)
    UITweenPositionExport.SetZTo(self.id, zTo)
end