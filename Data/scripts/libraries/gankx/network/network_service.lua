local NetworkExport = CS.NetworkExport
local NetworkScope = NetworkScope
local NetworkSendType = NetworkSendType
local NetworkResult = NetworkResult

module("Gankx", package.seeall)

local ReenterMsgMap = {
}

service("NetworkService")

MAX_RESEND_NUM = 200

function startup(self)
    NetworkExport.Initialize()

    self._talkers = {}
    self._msgDefines = {}
    self._recvCaches = {}
    self._sendCaches = {}
    self._resendList = {}

    self:listenEvent("OnNetRecv", _onNetRecv)
end

function shutdown(self)
    NetworkExport.Release()
end

function _addTalker(self, scope, msgType, msgDefine, talker)
    if nil == scope or nil == msgType or nil == talker then
        self:error("_addTalker", debug.traceback("Invalid parameters!"))
        return
    end

    local msgDefines = self._msgDefines
    if msgDefines[msgType] == nil then
        msgDefines[msgType] = msgDefine()
    end

    local scopeTalkers = self._talkers[scope]
    if nil == scopeTalkers then
        scopeTalkers = {}
        self._talkers[scope] = scopeTalkers
    end

    local talkers = scopeTalkers[msgType]
    if nil == talkers then
        talkers = {}
        scopeTalkers[msgType] = talkers
    end

    Array.add(talkers, talker)
end

function _removeTalker(self, scope, msgType, talker)
    if nil == scope or nil == msgType then
        self:error("_removeTalker", debug.traceback("Invalid parameters!"))
        return
    end

    if nil == talker then
        self:error("_removeTalker", debug.traceback("Invalid parameters!"))
        return
    end

    local scopeTalkers = self._talkers[scope]
    if nil == scopeTalkers then
        return
    end

    local talkers = scopeTalkers[msgType]
    if nil == talkers then
        return
    end

    Array.remove(talkers, talker)
end

function _removeAllTalkers(self, scope)
    if nil == scope then
        self:error("_removeAllTalkers", debug.traceback("Invalid parameters!"))
        return
    end

    local scopeTalkers = self._talkers[scope]
    if nil == scopeTalkers then
        return
    end

    self._talkers[scope] = {}
end

function _onNetRecv(self, scope,msgType, msgData)
    local seq = 1
    fireEvent("_OnRecv", scope, msgType, msgData)

    self:_onRawRecv(scope,msgType,msgData,seq)
end

function _onRawRecv(self,scope,msgType,msgData,seq)
    if self._isRecvCaching and not ReenterMsgMap[cache.msgType] then
        self:_addRecvCache(scope, msgType, msgData, seq)
        return
    end

    self:_onRecv(scope, msgType, msgData, seq)
end

function _isRecvReenter(self, cache)
    local msgObject = ReenterMsgMap[cache.msgType]
    if nil == msgObject then
        return false
    end

    local msgData = cache.msgData

    local bOk, errMsg = pcall(msgObject.Parse, msgObject, msgData)
    if not bOk then
        self:error("_isRecvReenter", "Parse message(" .. tostring(cache.msgType) .. ") data occured error: " .. tostring(errMsg))
        return false
    end

    return false
end

function startRecvCache(self)
    self._isRecvCaching = true
end

function stopRecvCaching(self)
    self._isRecvCaching = false
end

function handleRecvCachesUntilReenter(self)
    self:handleRecvCaches()
end

function handleFirstRecvCache(self)
    local cache = self._recvCaches[1]
    if not self:_isRecvReenter(cache) then
        self:error("handleFirstRecvCache", "This function can only be called on reenter cases!!!")
        return
    end

    Array.removeAt(self._recvCaches, 1)
    self:_onRecv(cache.scope, cache.msgType, cache.msgData, cache.seq)
end

function handleRecvCaches(self)
    while #self._recvCaches > 0 do
        local cache = self._recvCaches[1]

        if self:_isRecvReenter(cache) then
            return true
        end

        Array.removeAt(self._recvCaches, 1)
        self:_onRecv(cache.scope, cache.msgType, cache.msgData, cache.seq)
    end

    return false
end

function stopRecvCache(self)
    self._isRecvCaching = false

    self:handleRecvCaches()

    if #self._recvCaches > 0 then
        local cache = self._recvCaches[1]

        if self:_isRecvReenter(cache) then
            Array.removeAt(self._recvCaches, 1)
            self:_onRecv(cache.scope, cache.msgType, cache.msgData, cache.seq)
        end
    end
end

function _addRecvCache(self, scope, msgType, msgData, seq)
    local cachedMsg = {}
    cachedMsg.scope = scope
    cachedMsg.msgType = msgType
    cachedMsg.msgData = msgData
    cachedMsg.seq = seq

    Array.add(self._recvCaches, cachedMsg)
end

function clearRecvCache(self)
    self._recvCaches = {}
end

function _onRecv(self, scope, msgType, msgData, seq)
    if nil == msgType or nil == msgData then
        self:error("_onRecv", "msgType or msgData is invalid")
        return
    end

    local talkers = self._talkers[scope][msgType]
    if nil == talkers then
        Console.warning("recv message(" .. tostring(msgType) .. ") on scope(" .. NetworkScope.ToString(scope) .. ") occurred error: cannot find talker")
        return
    end

    local msgObject = self._msgDefines[msgType]
    local bOk, errMsg = pcall(msgObject.Parse, msgObject, msgData)
    if not bOk then
        Console.error("_onRecv:" .. "Parse message(" .. tostring(msgType) .. ") data occured error: " .. tostring(errMsg))
        return
    end

    for _, talker in ipairs(talkers) do
        talker:_onRecv(msgType, msgObject)
    end
end

function _onRecvLocal(self, scope, msgType, msgData)
    if nil == msgType or nil == msgData then
        self:error("_onRecvLocal", "msgType or msgData is invalid!")
        return
    end

    local talkers = self._talkers[scope][msgType]
    if nil == talkers then
        Console.warning("recv local message(" .. tostring(msgType) .. ") on scope(" .. NetworkScope.ToString(scope) .. ") occurred error: cannot find talker!")
        return
    end

    for _, talker in ipairs(talkers) do
        talker:_onRecvLocal(msgType, msgData)
    end
end

function startSendCache(self)
    self._isSendCaching = true
end

function stopSendCache(self)
    self._isSendCaching = false
end

function _addSendCache(self, scope, msgType, msgData, sendType)
    local cachedMsg = {}
    cachedMsg.scope = scope
    cachedMsg.msgType = msgType
    cachedMsg.msgData = msgData
    cachedMsg.sendType = sendType

    Array.add(self._sendCaches, cachedMsg)
end

function clearSendCache(self)
    self._sendCaches = {}
end

function setSendCacheWhiteList(self, whiteList)
    self._sendCacheWhiteList = whiteList
end

function handleSendCaches(self)
    for i=1,#self._sendCaches do
        local cachedMsg = self._sendCaches[i]
        self:_send(cachedMsg.scope,cachedMsg.msgType,cachedMsg.msgData,cachedMsg.sendType)
    end

    self:clearSendCache()
end

function _addResendList(self, scope, msgType, msgData, sendType)
    local cachedMsg = {}
    cachedMsg.scope = scope
    cachedMsg.msgType = msgType
    cachedMsg.msgData = msgData
    cachedMsg.sendType = sendType

    if #self._resendList >= MAX_RESEND_NUM then
        table.remove(self._resendList,1)
    end

    table.insert(self._resendList, cachedMsg)
end

function clearResendList(self)
    self._resendList = {}
    self._sendSeq = 0
end

function executeResend(self, sendSeq)
    if nil == sendSeq then
        Console.error("FATAL ERROR IN ExecuteResend c2g_proto_seq is nil !!")
        return
    end

    local offset = self._sendSeq - sendSeq

    if offset < 0 then
        Console.error("FATAL ERROR IN ExecuteResend Offset is negative !!")
        EnterSceneSystem.instance:LogOut()
        return
    end

    local serverReceivedRange = #self._resendList - offset
    for i = 1,serverReceivedRange do
        table.remove(self._resendList,1)
    end

    for i = 1,#self._resendList  do
        local msg = self._resendList[i]
        self:_rawsend(msg.scope, msg.msgType, msg.msgData, msg.sendType)
    end
end

function _rawsend(self, scope, msgType, msgData, sendType)
    if not NetworkScope.isValid(scope) then
        self:error("_rawSend", debug.traceback("invalid scope!"))
        return false
    end

    if sendType == NetworkSendType.Blocking then
        fireEvent("OnNetBlockingSend",msgType)
    end

    local result = NetworkExport.SendByMsgType(scope,msgType,msgData)

    return true
end

function send(self, scope, msgType, msgData, sendType)
    if self._isSendCaching and Array.contains(self._sendCacheWhiteList, msgType) then
        self:_addSendCache(scope, msgType, msgData,sendType)
        return
    end

    self:_send(scope, msgType, msgData, sendType)
end

function _send(self, scope, msgType, msgData, sendType)
    if not self:_rawsend(scope, msgType, msgData, sendType) then
        return
    end

    if nil == self._sendCacheWhiteList then
        return
    end

    if not Array.contains(self._sendCacheWhiteList, msgType) then
        return
    end

    self._sendSeq = (self._sendSeq or 0) + 1
    self:_addResendList(scope, msgType, msgData, sendType)
end

function _resend(self, scope, msgType, msgData, seq)
    if not NetworkScope.isValid(scope) then
        self:error("_resend", debug.traceback("Invalid scope!"))
        return NetworkResult.Error
    end

    return NetworkExport.SendByMsgType(scope, msgType,msgData)
end

function _connect(self, scope, platform, url)
    if not NetworkScope.isValid(scope) then
        self:error("_connect", debug.traceback("Invalid scope!"))
        return false
    end

    return NetworkExport.Connect(scope, platform, url)
end

function _disconnect(self, scope)
    if not NetworkScope.isValid(scope) then
        self:error("_disconnect", debug.traceback("Invalid scope!"))
        return
    end

    fireEvent("OnDisconnectNet", scope)
    NetworkExport.Disconnect(scope)
end

function _reconnect(self, scope)
    if not NetworkScope.isValid(scope) then
        self:error("_reconnect", debug.traceback("Invalid scope!"))
        return false
    end

    return NetworkExport.Reconnect(scope)
end

function _reconnect2(self, scope)
    if not NetworkScope.isValid(scope) then
        self:error("_reconnect2", debug.traceback("Invalid scope!"))
        return false
    end

    return NetworkExport.Reconnect2(scope)
end

function _setMsgLimitEnable(self,scope,enable)
    if not NetworkScope.isValid(scope) then
        self:error("_setMsgLimitEnable", debug.traceback("Invalid scope!"))
        return false
    end

    return NetworkExport.SetMsgLimitEnable(scope,enable)
end
