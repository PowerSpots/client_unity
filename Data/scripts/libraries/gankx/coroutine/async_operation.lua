module("Gankx", package.seeall)

class("AsyncOperation", "YieldInstruction")

function constructor(self)
    self._isDone = false
    self._progress = 0
end

function keepWaiting(self)
    return not self:getIsDone()
end

function getProgress(self)
    return self._progress
end

function getIsDone(self)
    return self._isDone
end

