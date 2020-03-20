module("Timer", package.seeall)

local TimerMeta = {}
TimerMeta.__index = TimerMeta
local instanceId = 0

function TimerMeta:new()
    instanceId = instanceId + 1
    self._instanceId = instanceId

    local timer = {}
    setmetatable(timer, self)

    timer.interval = 0
    timer.repeatCount = 0
    timer.alwaysRepeat = false
    timer.curTime = 0
    timer.totalTime = 0
    timer.type = "Timer"
    timer.param = nil

    return timer
end

function TimerMeta:clone()
    local newTimer = TimerMeta:new()
    newTimer.interval = self.interval
    newTimer.alwaysRepeat = self.alwaysRepeat
    if not self.alwaysRepeat then
        newTimer.repeatCount = self.repeatCount
    end
    return newTimer
end

function TimerMeta:updateTimes(event, deltTime)
    self.curTime = self.curTime + deltTime
    self.totalTime = self.totalTime + deltTime

    if self.curTime >= self.interval then
        if event ~= nil then
            event:Fire(self.curTime, self.totalTime, self.param)
            self.repeatCount = self.repeatCount - 1
        end

        self.curTime = 0
    end
end

function TimerMeta:updateAlways(event, deltTime)
    self.curTime = self.curTime + deltTime
    self.totalTime = self.totalTime + deltTime

    if self.curTime >= self.interval then
        if event ~= nil then
            event:Fire(self.curTime, self.totalTime)
        end

        self.curTime = 0
    end
end

TimerMeta.__tostringx = function (p)
    TimerMeta.__tostring = nil
    local s = "TimerMeta ".. tostring(p._instanceId) .. tostring(p)
    TimerMeta.__tostring = TimerMeta.__tostringx
    return s
end
TimerMeta.__tostring = TimerMeta.__tostringx

function always(interval, delayed)
    local timer = TimerMeta:new()
    timer.interval = interval or 0
    timer.alwaysRepeat = true

    if delayed ~= nil then
        timer.curTime = timer.interval - delayed
    end

    return timer
end

function times(interval, count, param)
    local timer = TimerMeta:new()
    timer.interval = interval or 0
    timer.alwaysRepeat = false
    timer.repeatCount = count or 0
    timer.param = param
    return timer
end

function once(interval, param)
    return times(interval, 1, param)
end
