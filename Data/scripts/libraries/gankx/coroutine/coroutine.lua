local YieldWaitForSeconds = Gankx.WaitForSeconds

module("Gankx.Coroutine", package.seeall)

local YIELD_NULL = 1
local YIELD_WAITFORSECONDS = 2
local YIELD_ASYNCOPERATION = 3
local YIELD_WAITFORENDOFFRAME = 4
local YIELD_NULL_UNLIMIT = 10

local myResumingCoroutine
local myYieldedCoroutines

local function error(title, msg, context)
    local errorMsg
    if context ~= nil and context.ToString ~= nil then
        errorMsg = "Coroutine." .. title .. " on [" .. context:toString() .. "] context" .. " occurred error: " .. msg
    else
        errorMsg = "Coroutine." .. title .. " occurred error: " .. msg
    end

    Console.error(errorMsg)
end

local function safeCall(context, funcName, ...)
    if nil == context then
        return
    end

    local func = context[funcName]
    if nil == func then
        return
    end

    local bok, result = pcall(func, context, ...)
    if not bok then
        error("safeCall", result, context)
        return
    end

    return result
end

function _startOnContext(yieldedCoroutine)
    local context = yieldedCoroutine.context

    local contextCoroutines = context.__coroutines
    if nil == contextCoroutines then
        contextCoroutines = {}
        context.__coroutines = contextCoroutines
    end

    table.insert(contextCoroutines, yieldedCoroutine)
end

function _stopOnContext(yieldedCoroutine)
    local context = yieldedCoroutine.context
    if nil == context then
        return
    end

    local contextCoroutines = context.__coroutines
    if nil == contextCoroutines then
        return
    end

    for i, contextCoroutine in ipairs(contextCoroutines) do
        if contextCoroutine == yieldedCoroutine then
            contextCoroutine.context = nil
            contextCoroutine.method = nil
            table.remove(contextCoroutines, i)
            return
        end
    end
end

local function _resume(yieldedCoroutine, ...)
    yieldedCoroutine.yieldType = nil
    yieldedCoroutine.yieldInstruction = nil

    myResumingCoroutine = yieldedCoroutine

    local bOk, err = coroutine.resume(yieldedCoroutine.coroutine, ...)
    if not bOk then
        error("resume", err, yieldedCoroutine.context)
        Console.error(debug.traceback(yieldedCoroutine.coroutine, err))
    end

    myResumingCoroutine = nil
end

local function _yield(yieldType, yieldInstruction)
    local resumeCoroutine = myResumingCoroutine
    if nil == resumeCoroutine then
        error("yield(" .. yieldType .. ")", "please call in a coroutine ! ")
        return nil
    end

    if nil == myYieldedCoroutines then
        myYieldedCoroutines = {}
    end

    if nil == myYieldedCoroutines[yieldType] then
        myYieldedCoroutines[yieldType] = {}
    end

    resumeCoroutine.yieldType = yieldType
    resumeCoroutine.yieldInstruction = yieldInstruction

    table.insert(myYieldedCoroutines[yieldType], resumeCoroutine)

    safeCall(yieldInstruction, "onYield")

    if yieldType == YIELD_ASYNCOPERATION then
        fireEvent("OnAsyncOperationStart", yieldInstruction)
    end

    coroutine.yield(resumeCoroutine.coroutine)
end

local function _Update(yieldType)
    if nil == myYieldedCoroutines then
        return
    end

    local yieldedCoroutines = myYieldedCoroutines[yieldType]
    if nil == yieldedCoroutines then
        return
    end

    myYieldedCoroutines[yieldType] = nil

    for i, yieldedCoroutine in ipairs(yieldedCoroutines) do
        local yieldInstruction = yieldedCoroutine.yieldInstruction

        if nil == yieldedCoroutine.context or nil == yieldedCoroutine.method then

            yieldedCoroutine.yieldType = nil
            yieldedCoroutine.yieldInstruction = nil
            safeCall(yieldInstruction, "onStop")
            safeCall(yieldInstruction, "onResume")

        elseif safeCall(yieldInstruction, "keepWaiting") ~= true then
            -- Resume when:
            -- 1. yieldInstruction is nil
            -- 2. keepWaiting is not defined
            -- 3. keepWaiting occurred error
            -- 4. keepWaiting not return true

            safeCall(yieldInstruction, "onResume")

            if yieldType == YIELD_ASYNCOPERATION then
                fireEvent("OnAsyncOperationStop", yieldInstruction)
            end

            _resume(yieldedCoroutine)

            if yieldedCoroutine.yieldType == nil then
                _stopOnContext(yieldedCoroutine)
            end
        else
            if myYieldedCoroutines[yieldType] == nil then
                myYieldedCoroutines[yieldType] = {}
            end
            table.insert(myYieldedCoroutines[yieldType], yieldedCoroutine)
        end
    end
end

function start(context, method, ...)
    if nil == context then
        error("start", debug.traceback("invalid parameter!"))
        return
    end

    if type(method) == "string" then
        method = context[method]
    end

    if type(method) ~= "function" then
        error("start", debug.traceback("method is not a function!"))
        return
    end

    local myCoroutine = {}
    myCoroutine.context = context
    myCoroutine.method = method
    myCoroutine.coroutine = coroutine.create(method)
    myCoroutine.yieldType = nil
    myCoroutine.yieldInstruction = nil

    local oldResumingCoroutine = myResumingCoroutine

    myResumingCoroutine = nil

    _resume(myCoroutine, context, ...)

    myResumingCoroutine = oldResumingCoroutine

    if myCoroutine.yieldType ~= nil then
        _startOnContext(myCoroutine)
    else
        myCoroutine.context = nil
        myCoroutine.method = nil
    end
end

function stopAll(context)
    if nil == context then
        error("stopAll", debug.traceback("invalid parameter!"))
        return
    end

    local contextCoroutines = context.__coroutines
    if nil == contextCoroutines then
        return
    end

    context.__coroutines = nil

    for _, contextCoroutine in ipairs(contextCoroutines) do
        contextCoroutine.context = nil
        contextCoroutine.method = nil
    end
end

function stop(context, method)
    if nil == context then
        error("stop", debug.traceback("invalid parameter!"))
        return
    end

    if type(method) == "string" then
        method = context[method]
    end

    if type(method) ~= "function" then
        error("stop", debug.traceback("method is not a function!"))
        return
    end

    local contextCoroutines = context.__coroutines
    if nil == contextCoroutines then
        return
    end

    context.__coroutines = nil

    for i, contextCoroutine in ipairs(contextCoroutines) do
        if contextCoroutine.method == method then
            contextCoroutine.context = nil
            contextCoroutine.method = nil
        else
            if nil == context.__coroutines then
                context.__coroutines = {}
            end
            table.insert(context.__coroutines, contextCoroutine)
        end
    end
end

local function _print(yieldType, yieldName, sb, prefix)
    prefix = prefix or ""
    local subPrefix = prefix .. "    "
    if nil == myYieldedCoroutines then
        return
    end

    local yieldedCoroutines = myYieldedCoroutines[yieldType]
    if nil == yieldedCoroutines then
        return
    end

    sb:appendLine(prefix .. "- " .. yieldName .. ":")

    for i, yieldedCoroutine in ipairs(yieldedCoroutines) do
        local context = yieldedCoroutine.context
        local method = yieldedCoroutine.method
        local methodInfo = debug.getinfo(method, "S")
        sb:appendLine(subPrefix .. "- [" .. context:toString() .. "]: at " .. methodInfo.source .. ":" .. methodInfo.linedefined)
    end
end

function print()
    local sb = StringBuilder:new()
    local prefix = "    "
    sb:appendLine("")
    sb:appendLine("Coroutines:")

    _print(YIELD_NULL, "YIELD_NULL", sb, prefix)
    _print(YIELD_WAITFORSECONDS, "YIELD_WAITFORSECONDS", sb, prefix)
    _print(YIELD_ASYNCOPERATION, "YIELD_ASYNCOPERATION", sb, prefix)
    _print(YIELD_WAITFORENDOFFRAME, "YIELD_WAITFORENDOFFRAME", sb, prefix)

    _print(YIELD_NULL_UNLIMIT, "YIELD_NULL_UNLIMIT", sb, prefix)

    Console.info(sb:toString())
end

function yieldNull()
    return _yield(YIELD_NULL)
end

function updateNull()
    return _Update(YIELD_NULL)
end

function yieldNullUnlimit()
    return _yield(YIELD_NULL_UNLIMIT)
end

function updateNullUnlimit()
    return _Update(YIELD_NULL_UNLIMIT)
end

function yieldWaitForSeconds(seconds)
    return _yield(YIELD_WAITFORSECONDS, YieldWaitForSeconds:new(seconds))
end

function updateWaitForSeconds()
    return _Update(YIELD_WAITFORSECONDS)
end

function yieldAsyncOperation(asyncOperation)
    if nil == asyncOperation then
        return
    end

    return _yield(YIELD_ASYNCOPERATION, asyncOperation)
end

function updateAsyncOperation()
    return _Update(YIELD_ASYNCOPERATION)
end

function yieldWaitForEndOfFrame()
    return _yield(YIELD_WAITFORENDOFFRAME)
end

function updateWaitForEndOfFrame()
    return _Update(YIELD_WAITFORENDOFFRAME)
end
