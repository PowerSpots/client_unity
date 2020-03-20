local NetworkScope = NetworkScope
local Reconnector = Gankx.Reconnector
local NetworkResult = Gankx.NetworkResult

module("Gankx", package.seeall)

service("ReconnectService")

function startup(self)
    self._reconnectors = {}
    self:listenEvent("OnNetConnect", onGetReconnectResult)
    self:listenEvent("OnNetError", onNetError)
    self:listenEvent("OnNetStateChanged", onNetStateChanged)
end

function shutdown(self)
    self:unlistenAll()
    self._reconnectors = {}
end

function update(self, deltaTime)
    for _, reconnector in pairs(self._reconnectors) do
        reconnector:update(deltaTime)
    end
end

function openReconnector(self, scope, url)
    if not NetworkScope.isValid(scope) then
        self:error("openReconnector", debug.traceback("invalid scope! " .. tostring(scope)))
        return
    end

    if nil == self._reconnectors[scope] then
        self._reconnectors[scope] = Reconnector:new(scope, url)
    end
end

function closeReconnector(self, scope)
    if not NetworkScope.isValid(scope) then
        self:error("closeReconnector", debug.traceback("invalid scope! " .. tostring(scope)))
        return
    end

    if nil ~= self._reconnectors[scope] then
        self._reconnectors[scope] = nil
    end
end

function hasReconnector(self, scope)
    if not NetworkScope.isValid(scope) then
        self:error("hasReconnector", debug.traceback("invalid scope! " .. tostring(scope)))
        return false
    end

    return (self._reconnectors[scope] ~= nil)
end

function onGetReconnectResult(self, scope, result)
    if not self:hasReconnector(scope) then
        return
    end

    if result == NetworkResult.SvrIsFull then
        self:closeReconnector(scope)
    else
        self._reconnectors[scope]:onGetReconnectResult(result)
    end
end

function onNetError(self, scope, result)
    if not self:hasReconnector(scope) then
        return
    end

    self._reconnectors[scope]:onGetReconnectResult(result)
end

function onNetStateChanged(self, netState)
    for _, reconnector in pairs(self._reconnectors) do
        reconnector:onNetStateChanged(netState)
    end
end

function onHeartbeatTimeout(self, scope)
    if not self:hasReconnector(scope) then
        return
    end

    self._reconnectors[scope]:onHeartbeatTimeout()
end

function forceReconnect(self, scope)
    if not self:hasReconnector(scope) then
        return
    end

    self._reconnectors[scope]:forceReconnect()
end

function reconnectEnterFailed(self, scope)
    if not self:hasReconnector(scope) then
        return
    end

    self._reconnectors[scope]:enterFailed()
end

function isConnectionRunning(self, scope)
    return self:hasReconnector(scope) and self._reconnectors[scope]:isRunning()
end