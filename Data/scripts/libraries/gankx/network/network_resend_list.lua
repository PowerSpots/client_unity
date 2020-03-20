local NetworkSendType = NetworkSendType
local NetworkScope = NetworkScope
local NetworkResult = NetworkResult

module("Gankx", package.seeall)

class("NetworkResendList")

function constructor(self, service)
    self._service = service
    self._resendScopes = {}
end

local function error(self, title, msg)
    Console.error("NetworkResendList." .. title ..
            " occurred error: \n" .. tostring(msg))
end

local function warning(self, title, msg)
    Console.warning("NetworkResendList." .. title ..
            " occurred warning: \n" .. tostring(msg))
end

function add(self, scope, msgType, msgData, seq, sendType)
    if nil == seq or nil == sendType then
        return
    end

    local resendScope = self._resendScopes[scope]
    if nil == resendScope then
        resendScope = {}
        self._resendScopes[scope] = resendScope
    end

    if resendScope.sendType == NetworkSendType.Blocking then
        error(self, "add", "Add resend message(" .. tostring(msgType) .. ") on scope("
                .. NetworkScope.ToString(scope) .. ") occurred error: in blocking(" .. tostring(resendScope.msgType) .. ")")
        return
    end

    resendScope.msgType = msgType
    resendScope.msgData = msgData
    resendScope.seq = seq
    resendScope.sendType = sendType

    if sendType == NetworkSendType.Blocking then
        fireEvent("OnNetBlockingResendStart", scope)
    end
end

function remove(self, scope, seq)
    if nil == seq then
        return
    end

    local resendScope = self._resendScopes[scope]
    if nil == resendScope then
        return
    end

    if resendScope.seq ~= seq then
        return
    end

    warning(self, "remove", "Remove the resend message(" ..
            tostring(resendScope.msgType) .. ") on scope(" .. NetworkScope.toString(scope) .. ")")

    local sendType = resendScope.sendType

    resendScope.msgType = nil
    resendScope.msgData = nil
    resendScope.seq = nil
    resendScope.sendType = nil

    if sendType == NetworkSendType.Blocking then
        fireEvent("OnNetBlockingResendStop", scope)
    end
end

function resend(self, scope)
    local resendScope = self._resendScopes[scope]
    if nil == resendScope or nil == resendScope.sendType then
        return
    end

    local result = self._service:_resend(scope, resendScope.msgType, resendScope.msgData, resendScope.seq)

    warning(self, "resend", "The result of resend message(" ..
            tostring(resendScope.msgType) .. ") on scope(" .. NetworkScope.toString(scope) .. ") is: " .. tostring(result))

    if result ~= NetworkResult.Success and result ~= NetworkResult.NoConnection then
        self:remove(scope, resendScope.seq)
    else
        error(self, "resend", "The result of resend message(" ..
                tostring(resendScope.msgType) .. ") on scope(" .. tostring(scope) .. ") is: " .. tostring(result))
    end
end

function clear(self, scope)
    local resendScope = self._resendScopes[scope]
    if nil == resendScope then
        return
    end

    warning(self, "clear", "Clear the scope(" .. NetworkScope.ToString(scope) .. ")!")

    local sendType = resendScope.sendType

    self._resendScopes[scope] = nil

    if sendType == NetworkSendType.Blocking then
        fireEvent("OnNetBlockingResendStop", scope)
    end
end
