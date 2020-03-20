local Coroutine = Gankx.Coroutine

module("Application", package.seeall)

deltaTime = 0
totalTime = 0
unscaledDeltaTime = 0
unscaledTotalTime = 0

function startup()
    Console.info("Application - startup")
    collectgarbage("setpause",110)
    collectgarbage("setstepmul",5000)
    Services.startup()
end

function shutdown()
    Console.info("Application - shutdown")
    Services.shutdown()
end

local function updateLimit(dt, udt)
    deltaTime = dt
    totalTime = totalTime + dt
    unscaledDeltaTime = udt
    unscaledTotalTime = unscaledTotalTime + udt

    ComponentManager.update(dt)

    Event.update(dt)

    Coroutine.updateNull()
    Coroutine.updateWaitForSeconds()
    Coroutine.updateAsyncOperation()

    ComponentManager.lateUpdate(dt)

    Coroutine.updateWaitForEndOfFrame()
end

local function updateUnLimit()
    Coroutine.updateNullUnlimit()
end

function update(deltaTime, unscaledDeltaTime, isTimeLimit)
    updateUnLimit()
    if isTimeLimit then
        updateLimit(deltaTime, unscaledDeltaTime)
    end
end

function setRandomSeed()
    math.randomseed(tostring(os.time()):reverse():sub(1, 6))
end

setRandomSeed()

_G.Application_Startup = startup
_G.Application_Update = update
_G.Application_Shutdown = shutdown
