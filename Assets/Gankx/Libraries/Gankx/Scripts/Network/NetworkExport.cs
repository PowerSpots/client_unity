using Gankx;

public class NetworkExport
{
    public static void Initialize()
    {
        NetworkService.instance.Initialize();
    }

    public static void Release()
    {
        if (!NetworkService.ContainsInstance())
        {
            return;
        }

        NetworkService.instance.Release();
    }

    public static string GetAccountId()
    {
        return NetworkAccount.accountId;
    }

    public static bool IsPlatformInstalled(int platform)
    {
        return NetworkAccount.IsPlatformInstalled((NetworkPlatform) platform);
    }

    public static int Login(int platform)
    {
        return (int) NetworkAccount.Login((NetworkPlatform) platform);
    }

    public static int GetRecord()
    {
        return (int) NetworkAccount.GetRecord();
    }

    public static void Logout()
    {
        NetworkAccount.Logout();
    }

    public static bool Connect(int id, int platform, string url)
    {
        return NetworkService.instance.Connect(id, (NetworkPlatform) platform, url);
    }

    public static bool Reconnect(int id)
    {
        return NetworkService.instance.Reconnect(id);
    }

    public static bool Reconnect2(int id)
    {
        return NetworkService.instance.Reconnect2(id);
    }

    public static bool ForceHandleMsg(int id)
    {
        return NetworkService.instance.ForceHandleMsg(id);
    }

    public static bool SetMsgLimitEnable(int id, bool enable)
    {
        return NetworkService.instance.SetMsgLimitEnable(id, enable);
    }

    public static void Disconnect(int id)
    {
        NetworkService.instance.Disconnect(id);
    }

    public static void DisconnectConnector(int id)
    {
        NetworkService.instance.DisconnectConnector(id);
    }

    public static int Send(int id, byte[] data)
    {
        return (int) NetworkService.instance.Send(id, data, data.Length);
    }

    public static int SendByMsgType(int id, int msgType, byte[] data)
    {
        return (int) NetworkService.instance.SendByMsgType(id, msgType, data);
    }

    public static int GetNetworkState()
    {
        return NetworkService.instance.GetNetworkState();
    }
}