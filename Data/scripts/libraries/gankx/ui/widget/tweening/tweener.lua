local UITweenerExport = CS.UITweenBaseExport

TweenerStyle =
{
    Once = 0,
    Loop = 1,
    PingPong = 2,
}

widget("Tweener", "Window")

function _getTweenerExport(self)
    return UITweenerExport
end

function play(self, forward, toBeginning)
    self:_getTweenerExport().Play(self.id, forward, toBeginning)
end

function playFromCurrentValue(self, forward, toBeginning)
    self:_getTweenerExport().PlayFromCurrentValue(self.id, forward, toBeginning)
end

function setEnabled(self, enabled)
    self:_getTweenerExport().SetEnabled(self.id, enabled)
end

function getEnabled(self)
    return self:_getTweenerExport().GetEnabled(self.id)
end

function setDuration(self, duration)
    self:_getTweenerExport().SetDuration(self.id, duration)
end

function getDuration(self)
    return self:_getTweenerExport().GetDuration(self.id)
end

function setDelay(self, delay)
    self:_getTweenerExport().SetDelay(self.id, delay)
end

function getDelay(self)
    return self:_getTweenerExport().GetDelay(self.id)
end

function setFinishedReceiver(self, value)
    self:_getTweenerExport().SetFinishedReceiver(self.id, value)
end

function getFinishedReceiver(self)
    return self:_getTweenerExport().GetFinishedReceiver(self.id)
end

function setStyle(self, style)
    if TweenerStyle.Once ~= style or TweenerStyle.Loop ~= style or TweenerStyle.PingPong ~= style then
        self:__error("setStyle", "style is invalid")
        return
    end

    self:_getTweenerExport().SetStyle(self.id, style)
end

function resetToBeginning(self)
    self:_getTweenerExport().ResetToBeginning(self.id)
end