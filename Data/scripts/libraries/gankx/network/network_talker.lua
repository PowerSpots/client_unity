local NetworkScope = NetworkScope
local NetworkService = NetworkService
local NetworkSendType = NetworkSendType

module("Gankx", package.seeall)

class("NetworkTalker")

ShieldMsgTypes = nil
local ShieldCallback
local ShieldContext

function SetShieldMsgTypes(shieldMsgTypes, callback, context)
    ShieldMsgTypes = shieldMsgTypes
    if nil == shieldMsgTypes then
        ShieldCallback = nil
        ShieldContext = nil
    else
        ShieldCallback = callback
        ShieldContext = context
    end
end

function constructor(self, context, scope)
    self._handlers = {}
    self._context = context
    self.scope = scope or NetworkScope.Game
end

function destroy(self)
    self:unrecvAll()
    self._context = nil
    setmetatable(self, nil)
end

local function IsShielded(msgType)
    if nil == ShieldMsgTypes then
        return false
    end

    for _, msgId in pairs(ShieldMsgTypes) do
        if msgType == msgId then
            return true
        end
    end

    return false
end

function error(self, title, msg)
    local contextName = "unknown"
    local context = self._context
    if context ~= nil then
        if context.ToString ~= nil then
            contextName = context:toString()
        else
            contextName = tostring(context)
        end
    end

    Console.error(debug.traceback("NetworkTalker." .. title ..
            " on [" .. contextName .. "]" ..
            " occurred error: \n" .. tostring(msg)))
end

function send(self, msgType, msgObject, sendType)
    if nil == msgType or nil == msgObject then
        error(self, "send", debug.traceback("Invalid Parameters!"))
        return
    end

    if IsShielded(msgType) then
        if ShieldCallback ~= nil then
            if nil == ShieldContext then
                local bOk, result = pcall(ShieldCallback, msgType)
                if not bOk then
                    self:_error("send", "Call custom shield callback failed, msgType: " .. tostring(msgType))
                end
            else
                local bOk, result = pcall(ShieldCallback, ShieldContext, msgType)
                if not bOk then
                    self:_error("send", "Call custom shield callback failed, msgType: " .. tostring(msgType))
                end
            end
        end

        return
    end

    local bOk, msgData = pcall(msgObject.Serialize, msgObject)
    if (not bOk) or (nil == msgData) then
        self:_error("send", "Serialize message(" .. tostring(msgType) .. ") data occured error: " .. tostring(msgData))
        return
    end

    NetworkService.instance:send(self.scope, msgType, msgData, sendType)
end

function blockingsend(self, msgType, msgObject)
    self:send(msgType, msgObject, NetworkSendType.Blocking)
end

local function errorHandle(err)
    return debug.traceback(err)
end

function _onRecv(self, msgType, msgObject)
    local handler = self._handlers[msgType]
    if nil == handler then
        return
    end

    local bOk, errMsg = xpcall(handler, errorHandle, self._context, msgObject)

    if not bOk then
        error(self, "_onRecv", "Handle message(" .. tostring(msgType) .. ") data occured error: " .. errMsg)
        return
    end
end

function onRecvLocal(self, msgType, msgData)
    local handler = self._handlers[msgType]
    if nil == handler then
        return
    end

    local bOk, errMsg = xpcall(handler, errorHandle, self._context, msgData)

    if not bOk then
        error(self, "onRecvLocal", "Handle message(" .. tostring(msgType) .. ") data occured error: " .. errMsg)
        return
    end

end

function recv(self, msgType, msgDefine, func)
    if nil == msgType or nil == msgDefine or nil == func then
        self:_error("recv", debug.traceback("Invalid parameters"))
        return
    end

    self._handlers[msgType] = func

    NetworkService.instance:_addTalker(self.scope, msgType, msgDefine, self)
end

function unrecv(self, msgType)
    if nil == msgType then
        self:_error("unrecv", debug.traceback("Invalid parameters"))
        return
    end

    self._handlers[msgType] = nil

    NetworkService.instance:_removeTalker(self.scope, msgType, self)
end

function unrecvAll(self)
    for msgType, handler in pairs(self._handlers) do
        NetworkService.instance:_removeTalker(self.scope, msgType, self)
    end

    self._handlers = {}
end

function connect(self, platform, url)
    return NetworkService.instance:_connect(self.scope, platform, url)
end

function disconnect(self)
    NetworkService.instance:_disconnect(self.scope)
end

function setMsgLimitEnable(self, enable)
    return NetworkService.instance:_setMsgLimitEnable(self.scope, enable)
end