
namespace Gankx
{
    public enum NetworkResult
    {
        Success = 0,
        Error,
        NetworkException,
        Timeout,

        PlatformNotInstall = 10,

        InnerError = 100,
        NotInitialized,
        NoConnection,
        ConnectFailed,

        GcpError = 120,
        PeerCloseConnection,
        PeerStopSession,
        PkgNotCompleted,
        SendFailed,
        StayInQueue,
        SvrIsFull,
        TokenSvrError,

        Others = 10000,
        Disconnected,
        NotCreated,
    }
}