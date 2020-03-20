module("Event", package.seeall)

local function responserErrorReplace()
end

EventMeta = {}

function EventMeta:error(title, msg, context)
    Console.error(debug.traceback( "事件(" .. tostring(self.name) .. ")[" .. title .. "]执行错误：" .. tostring(msg)))
end

function EventMeta:new()
    local event = {}
    self.__index = self
    setmetatable(event, self)

    event.name = ""
    event._timer = nil
    event._responsers = {}

    return event
end

function EventMeta:bind(context, method)
    if nil == context then
        self:error("bind", "参数错误", context)
        return false
    end

    if type(method) == "string" then
        method = context[method]
    end

    if type(method) ~= "function" then
        self:error("bind", "绑定的方法不是一个可执行函数", context)
        return false
    end


    local responser = {}
    responser.context = context
    responser.method = method

    if self:isBinding(context, method) then
        self:error("bind", "事件重复注册", context)
        return true
    end


    self:_bind(responser)

    table.insert(self._responsers, responser)
    return true
end

function EventMeta:_bind(responser)
    local context = responser.context
    local contextEvents = rawget(context, "__events")
    if nil == contextEvents then
        contextEvents = {}
        rawset(context, "__events", contextEvents)
    end

    local responsers = contextEvents[self.name]
    if nil == responsers then
        responsers = {}
        contextEvents[self.name] = responsers
    end

    table.insert(responsers, responser)
end

function EventMeta:unbind(context, method)
    if nil == context then
        self:error("unbind", "参数错误", context)
        return
    end

    if type(method) == "string" then
        local methodName = method
        method = context[methodName]
        if nil == method then
            self:error("unbind", "在Context中无法找到Method(" .. methodName .. ")")
            return
        end
    end

    local contextEvents = rawget(context, "__events")
    if nil == contextEvents then
        return
    end

    local responsers = contextEvents[self.name]
    if nil == responsers then
        return
    end

    if nil == method then
        contextEvents[self.name] = nil

        for i, responser in ipairs(responsers) do
            self:_unbind(responser)
        end
    else
        local rmCount = 0
        local responserCount = #responsers
        for i = 1, responserCount do
            local responser = responsers[i - rmCount]
            if responser.Method == method then
                self:_unbind(responser)
                table.remove(responsers, i - rmCount)
                rmCount = rmCount + 1
            end
        end
    end
end

function EventMeta:isBinding(context, method)
    if nil == context then
        self:error("isBinding", "参数错误", context)
        return false
    end

    if type(method) == "string" then
        local methodName = method
        method = context[methodName]
        if nil == method then
            self:error("isBinding", "在Context中无法找到Method(" .. methodName .. ")", context)
            return false
        end
    end

    local contextEvents = rawget(context, "__events")
    if nil == contextEvents then
        return false
    end

    local responsers = contextEvents[self.name]
    if nil == responsers then
        return false
    end

    for i, responser in ipairs(responsers) do
        if nil == method or responser.method == method then
            return true
        end
    end

    return false
end

function EventMeta:_unbind(_responser)
    local responsers = self._responsers
    for i, responser in ipairs(responsers) do
        if responser == _responser then
            responser.context = nil
            responser.method = nil
            table.remove(responsers, i)
            return
        end
    end
end

local errfunc = function(err)
    return  debug.traceback(err)
end

function EventMeta:fire(...)
    local responsers = ArrayPool.get()

    for _, responser in ipairs(self._responsers) do
        Array.add(responsers, responser)
    end

    for _, responser in ipairs(responsers) do

        local bOk, err = xpcall(responser.method, errfunc, responser.context, ...)

        if not bOk then
            responser.method = responserErrorReplace
            self:error("Fire", err)
        end
    end

    ArrayPool.put(responsers)
end

function EventMeta:responserCount()
    local responsers = self._responsers
    return #responsers
end

function EventMeta:setTimer(timer)
    if nil == timer then
        self._timer = nil
    elseif timer.Type == "Timer" then
        self._Timer = timer
    end
end

function EventMeta:update(deltTime)
    local timer = self._Timer

    if nil == timer then
        return
    end

    if timer.alwaysRepeat == true then
        timer:updateAlways(self, deltTime)
    elseif timer.repeatCount > 0 then
        timer:updateTimes(self, deltTime)
    elseif timer.repeatCount <= 0 then
        _close(self.name)
    end
end

function EventMeta:getSummaryInfo()
    local sb = StringBuilder:new()
    sb:appendLine("event name:" .. self.name)
    sb:appendLine("event responsers:" .. self:responserCount())

    local timer = self.Timer
    if timer ~= nil then
        sb:appendLine("event Timer:")
        sb:appendLine("\tInterval:" .. timer.interval)
        sb:appendLine("\tRepeatCount:" .. timer.repeatCount)
        sb:appendLine("\tAlwaysRepeat:" .. tostring(timer.alwaysRepeat))
        sb:appendLine("\tCurTime:" .. timer.curTime)
    end

    return sb:toString()
end
