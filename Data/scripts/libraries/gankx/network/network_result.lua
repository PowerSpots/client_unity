module("Gankx")

NetworkResult =
{
    Success = 0,
    Error = 1,
    NetworkException = 2,
    Timeout = 3,

    PlatformNotInstall = 10,

    InnerError = 100,
    NotInitialized = 101,
    NoConnection = 102,
    ConnectFailed = 103,

    GcpError = 120,
    PeerCloseConnection = 121,
    PeerStopSession = 122,
    PkgNotCompleted = 123,
    SendFailed = 124,
    StayInQueue = 125,
    SvrIsFull = 126,
    TokenSvrError = 127,

    -- Others
    Disconnected = 10001,
    NotCreated = 10002,
}

NetworkResultDes =
{
    [NetworkResult.Timeout] = "连接超时",
    [NetworkResult.PeerStopSession] = "服务器断开连接",
    [NetworkResult.PeerCloseConnection] = "服务器关闭连接",

    [NetworkResult.ConnectFailed] = "连接服务器失败",
}

