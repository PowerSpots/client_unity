local ReconnectState = ReconnectState
local NetworkScope = NetworkScope
local NetworkResult = Gankx.NetworkResult
local NetworkState = Gankx.NetworkState
local NetworkExport =  CS.NetworkExport
local ReconnectConfig = Gankx.Config.Network.reconnect

module("Gankx", package.seeall)

class("Reconnector")

function constructor(self, scope, url)
    self.scope = scope
    self.url = url
    self.state = ReconnectState.Connecting

    self:setReconnectConfig(1)

    self.timeout = self._connectingTimeout
    self.connectedTimes = 0

    self:sendReconnectStateMessage()
end

function setReconnectConfig(self, state)
    self._curReconnectConfigState = state

    if state >= #ReconnectConfig then
        state = #ReconnectConfig
    end

    local config = ReconnectConfig[state]

    self._reconnectMaxTimes = config.reconnectMaxTimes
    self._waitingTimeout = config.waitingTimeout
    self._connectingTimeout = config.connectingTimeout
end

function enterRunning(self)
    if not ReconnectState.canEnter(self.state, ReconnectState.Running) then
        self:info("enterRunning", string.format("scope [%s]can not enter running from %s", self.scope, ReconnectState.toString(self.state)))
        return
    end

    self.state = ReconnectState.Running
    self.timeout = 0
    self.connectedTimes = 0

    self:setReconnectConfig(1)
    self:sendReconnectStateMessage()
end

function enterWaiting(self)
    if not ReconnectState.canEnter(self.state, ReconnectState.Waiting) then
        self:info("enterWaiting", string.format("scope [%s]can not enter waiting from %s", self.scope, ReconnectState.toString(self.state)))
        return
    end

    self.state = ReconnectState.Waiting
    self.timeout = self._waitingTimeout

    self:sendReconnectStateMessage()
end

function enterConnecting(self)
    if not ReconnectState.canEnter(self.state, ReconnectState.Connecting) then
        self:info("enterConnecting", string.format("scope [%s]can not enter connecting from %s", self.scope, ReconnectState.toString(self.state)))
        return
    end

    self.state = ReconnectState.Connecting
    self.timeout = self._connectingTimeout
    self.connectedTimes = self.connectedTimes + 1

    if self.connectedTimes > self._reconnectMaxTimes then
        self:enterFailed()
        return
    end

    NetworkExport.Reconnect2(self.scope)

    self:sendReconnectStateMessage()
end

function enterFailed(self)
    if not ReconnectState.canEnter(self.state, ReconnectState.Failed) then
        self:info("enterFailed", string.format("scope [%s]can not enter failed from %s", self.scope, ReconnectState.toString(self.state)))
        return
    end

    self.state = ReconnectState.Failed
    self.timeout = 0
    self.connectedTimes = 0

    NetworkExport.DisconnectConnector(self.scope)

    self:sendReconnectStateMessage()
end

function update(self, deltaTime)
    if self.state == ReconnectState.Waiting then
        self.timeout = self.timeout - deltaTime
        if self.timeout < 0 then
            self:enterConnecting()
        end
    elseif self.state == ReconnectState.Connecting then
        self.timeout = self.timeout - deltaTime
        if self.timeout < 0 then
            self:enterWaiting()
        end
    end
end

function onGetReconnectResult(self, result)
    if NetworkResult.Success == result then
        self:enterRunning()
    else
        self:enterConnecting()
    end
end

function onNetStateChanged(self, netState)
--    if NetworkState.NotReachable == netState then
--        if self.state == ReconnectState.Failed then
--            self:enterConnecting()
--        end
--    else
--        if self.state == ReconnectState.Running then
--            self:enterWaiting()
--        end
--    end
end

function onHeartbeatTimeout(self)
    if self.state == ReconnectState.Running then
        self:enterConnecting()
    else
        self:info("onHeartbeatTimeout", "current state is " .. ReconnectState.ToString(self.state))
    end
end

function forceReconnect(self)
    local newState = self._curReconnectConfigState + 1
    self:setReconnectConfig(newState)

    if self.state == ReconnectState.Failed then
        self:enterWaiting()
    else
        self:info("forceReconnect", "current state is " .. ReconnectState.ToString(self.state))
    end
end

function sendReconnectStateMessage(self)
    fireEvent("OnReconnectStateChanged", self.scope, self.state)
end

function isRunning(self)
    return self.state == ReconnectState.Running
end

function error(self, title, msg)
    Console.error(string.format("Reconnector.[%s]: %s", title, msg))
end

function info(self, title, msg)
    Console.info(string.format("Reconnector.[%s]: %s", title, msg))
end

function warning(self, title, msg)
    Console.warning(string.format("Reconnector.[%s]: %s", title, msg))
end
