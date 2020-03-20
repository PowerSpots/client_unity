module("TimeControl", package.seeall)

local TimeControlExport = CS.TimeControlExport

local ScriptLayer = 1

local LayerScales =
{
    speed = 1,

    running = 1,

    chase = 1,

    boost = 1,
}

local function applyTimeScale()
    local timeScale = 1
    for k, v in pairs(LayerScales) do
        timeScale = timeScale * v
    end
    TimeControlExport.SetScale(ScriptLayer, timeScale)
end

local function error(title, msg)
    Console.error("TimeControl." .. title .. " occurred error: " .. tostring(msg))
end

function setSpeed(value)
    if nil == value or value < 0 then
        error("setSpeed", debug.traceback("Invalid parameters!"))
        return
    end

    LayerScales.speed = value

    applyTimeScale()
end

function getSpeed()
    return LayerScales.speed
end

function setChase(value)
    if nil == value or value < 0 then
        error("setChase", debug.traceback("Invalid parameters!"))
        return
    end

    LayerScales.chase = value

    applyTimeScale()
end

function getChase()
    return LayerScales.chase
end

function setBoost(value)
    if nil == value or value < 0 then
        error("setBoost", debug.traceback("Invalid parameters!"))
        return
    end

    LayerScales.boost = value

    applyTimeScale()
end

function getBoost()
    return LayerScales.boost
end

function isPaused()
    return LayerScales.running == 0
end

function pause()
    if not isPaused() then
        LayerScales.running = 0
        applyTimeScale()
    end
end

function resume()
    if isPaused() then
        LayerScales.running = 1
        applyTimeScale()
    end
end

function getDeltaTime()
    return TimeControlExport.GetDeltaTime()
end

function getUnscaledDeltaTime()
    return TimeControlExport.GetUnscaledDeltaTime()
end