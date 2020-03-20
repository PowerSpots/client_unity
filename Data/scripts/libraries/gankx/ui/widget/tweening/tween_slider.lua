local UITweenSliderExport =  CS.UITweenSliderExport

widget("TweenSlider", "Tweener")

function _getTweenerExport(self)
    return UITweenSliderExport
end

function reset(self, duration, from, to)
    UITweenSliderExport.Reset(self.id, duration, from, to)
end

function setFromTo(self, from, to)
    UITweenSliderExport.SetFromTo(self.id, from, to)
end

function setTo(self, to)
    UITweenSliderExport.SetTo(self.id, to)
end

function setFrom(self, from)
    UITweenSliderExport.SetFrom(self.id, from)
end

function setValue(self, from)
    UITweenSliderExport.SetValue(self.id, from)
end

function getTo(self)
    return UITweenSliderExport.GetTo(self.id)
end

function getFrom(self)
    return UITweenSliderExport.GetFrom(self.id)
end

function getValue(self)
    return UITweenSliderExport.GetValue(self.id)
end
