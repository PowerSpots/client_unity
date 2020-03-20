local Protobuf = Gankx.Protobuf
local ProtoFilePath = Gankx.Config.Protobuf.filepath

NetworkProtocol = Protobuf.loadAll(ProtoFilePath)

local C2GMsgIds = (NetworkProtocol["external_message"] and NetworkProtocol["external_message"].C2G_MESSAGE_ID) or {}
local G2CMsgIds = (NetworkProtocol["external_message"] and NetworkProtocol["external_message"].G2C_MESSAGE_ID) or {}

ProtoMsgIds = setmetatable({},{
    __index = function(table,key)
        if C2GMsgIds[key] then return C2GMsgIds[key] end
        if G2CMsgIds[key] then return G2CMsgIds[key] end
        Console.error("Get ProtoMsgId Failed : " .. tostring(key))
    end
})