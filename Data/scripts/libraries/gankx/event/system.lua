module("Event", package.seeall)

local function error(msg, context)
end

local needUpdateEvents = {}
local needUpdateKeys = {}
local needUpdateKeysCount = 0;
local noNeedUpdateEvents = {}

function _find(name)
    if nil == name then
        return nil
    end

    if needUpdateEvents[name] ~= nil then
        return needUpdateEvents[name]
    end

    if noNeedUpdateEvents[name] ~= nil then
        return noNeedUpdateEvents[name]
    end

    return nil
end

function _close(name)

    if nil == name then
        return
    end

    needUpdateEvents[name] = nil
    noNeedUpdateEvents[name] = nil
end

function listen(name, context, method)
    if nil == name then
        error("事件名称为空！")
        return
    end

    local event = noNeedUpdateEvents[name]
    if nil == event then
        event = EventMeta:new()
        event.name = name
        noNeedUpdateEvents[name] = event
    end

    event:bind(context, method)
end

function listenTimer(timer, context, method)
    if nil == timer or timer.Type ~= "Timer" then
        error("Timer指定无效，" ..
                "请使用Timer.always(intv),Timer.once(intv),Timer.Times(intv, times)这三种" ..
                "方式中的一种进行创建！")
        return
    end

    local name = "Timer_" .. tostring(timer)
    local event = needUpdateEvents[name]
    if nil == event then
        event = EventMeta:new()
        event.name = name
        event:setTimer(timer)

        if event:bind(context, method) then
            needUpdateEvents[name] = event
        end

    else
        event:bind(func)
    end
end

function unlisten(name, context, method)
    local event = _find(name)
    if nil == event then
        return
    end

    event:unbind(context, method)

    if event:responserCount() <= 0 then
        _close(event.name)
    end
end

function unlistenAll(context, method)
    if nil == context then
        error("参数错误！")
        return
    end

    local contextEvents = rawget(context, "__events")
    if nil == contextEvents then
        return
    end

    local unlistenEvents = {}
    for eventName, _ in pairs(contextEvents) do
        table.insert(unlistenEvents, eventName)
    end

    for i, eventName in ipairs(unlistenEvents) do
        unlisten(eventName, context, method)
    end
end

function isListening(name, context, method)
    local event = _find(name)
    if nil == event then
        return false
    end

    return event:isBinding(context, method)
end

function isListeningAll(context, method)
    if nil == context then
        error("参数错误！")
        return false
    end

    local contextEvents = rawget(context, "__events")
    if nil == contextEvents then
        return false
    end

    for eventName, _ in pairs(contextEvents) do
        if isListening(eventName, context, method) then
            return true
        end
    end

    return false
end

function fire(name, ...)
    if nil == name then
        return
    end

    local event = noNeedUpdateEvents[name]

    if event ~= nil then
        event:fire(...)
    end

    return false
end

local getSummaryInfo = function(pre, events)
    local sb = StringBuilder:new()
    sb:appendLine("")
    sb:appendLine(pre)

    for k, v in pairs(events) do
        sb:append("\t")
        sb:append(k)
        sb:append(": (")
        sb:append(v:ResponserCount())
        sb:appendLine(")")
    end
end

function print(name)
    local summaryInfo

    if name ~= nil then
        local event = _find(name)
        if event ~= nil then
            summaryInfo = event:getSummaryInfo()
        end
    else
        summaryInfo = getSummaryInfo("轮询事件列表：", needUpdateEvents) ..
                getSummaryInfo("触发事件列表：", noNeedUpdateEvents)
    end


    Console.info(summaryInfo)
end

function update(deltTime)

    needUpdateKeysCount = 0
    for k, event in pairs(needUpdateEvents) do
        needUpdateKeysCount = needUpdateKeysCount + 1
        needUpdateKeys[needUpdateKeysCount] = k
    end

    for i = 1, needUpdateKeysCount do
        local key = needUpdateKeys[i]
        local event = needUpdateEvents[key]
        if event ~= nil then
            event:update(deltTime)
        end
    end
end

_G.fireEvent = fire
