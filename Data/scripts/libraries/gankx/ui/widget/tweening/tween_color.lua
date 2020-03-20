local UITweenColorExport = CS.UITweenColorExport

widget("TweenColor", "Tweener")

function _getTweenerExport(self)
    return UITweenColorExport
end

function reset(self, duration, rFrom, gFrom, bFrom, aFrom, rTo, gTo, bTo, aTo)
    UITweenColorExport.Reset(self.id, duration, rFrom, gFrom, bFrom, aFrom, rTo, gTo, bTo, aTo)
end

function setFromTo(self, rFrom, gFrom, bFrom, aFrom, rTo, gTo, bTo, aTo)
    UITweenColorExport.SetFromTo(self.id, rFrom, gFrom, bFrom, aFrom, rTo, gTo, bTo, aTo)
end

function setTo(self, r, g, b, a)
    UITweenColorExport.SetTo(self.id, r, g, b, a)
end

function setFrom(self, r, g, b, a)
    UITweenColorExport.SetFrom(self.id, r, g, b, a)
end

function getRFrom(self)
    return UITweenColorExport.GetRFrom(self.id)
end

function getGFrom(self)
    return UITweenColorExport.GetGFrom(self.id)
end

function getBFrom(self)
    return UITweenColorExport.GetBFrom(self.id)
end

function getAFrom(self)
    return UITweenColorExport.GetAFrom(self.id)
end

function getRTo(self)
    return UITweenColorExport.GetRTo(self.id)
end

function GetGTo(self)
    return UITweenColorExport.GetGTo(self.id)
end

function GetBTo(self)
    return UITweenColorExport.GetBTo(self.id)
end

function GetATo(self)
    return UITweenColorExport.GetATo(self.id)
end
