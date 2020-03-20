local StopwatchExport =  CS.StopwatchExport

module("Gankx", package.seeall)

class("Stopwatch")

isHighResolution = StopwatchExport.IsHighResolution()
frequency = StopwatchExport.GetFrequency()

local tickFrequency = 10000000 / frequency

function constructor(self)
    self:reset()
end

function isRunning(self)
    return self._isRunning
end

local function getRawElapsedTicks(self)
    local rawElapsedTicks = self._elapsed
    if self._isRunning then
        rawElapsedTicks = rawElapsedTicks + getTimestamp() - self._startTimeStamp
    end
    return rawElapsedTicks
end

local function getElapsedDateTimeTicks(self)
    local rawElapsedTicks = getRawElapsedTicks(self)
    if isHighResolution then
      return rawElapsedTicks * tickFrequency
    end

    return rawElapsedTicks
end

function getElapsedMilliseconds(self)
    return math.floor(getElapsedDateTimeTicks(self) / 10000)
end

function getElapsedTime(self)
    return getElapsedDateTimeTicks(self) / 10000000
end

function getElapsedTicks(self)
    return getRawElapsedTicks(self)
end

function start(self)
    if self._isRunning then
        return
    end

    self._startTimeStamp = getTimestamp()
    self._isRunning = true
end

function startNew()
    local stopWatch = Stopwatch:new()
    stopWatch:start()
    return stopWatch
end

function stop(self)
    if not self._isRunning then
        return
    end

    self._elapsed = self._elapsed + getTimestamp() - self._startTimeStamp
    self._isRunning = false
end

function reset(self)
    self._elapsed = 0
    self._isRunning = false
    self._startTimeStamp = 0
end

function getTimestamp()
    return StopwatchExport.GetTimestamp()
end
