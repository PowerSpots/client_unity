module("Gankx", package.seeall)

class("WaitForSeconds", "YieldInstruction")

function constructor(self, seconds)
    self._seconds = seconds
end

function keepWaiting(self)
    local deltaTime = Application.deltaTime

    self._seconds = self._seconds - deltaTime

    if self._seconds > 0 then
        return true
    end

    return false
end

