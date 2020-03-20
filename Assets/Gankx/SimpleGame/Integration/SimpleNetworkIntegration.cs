using System.Collections.Generic;
using Gankx;

public class SimpleNetworkIntegration : NetworkIntegration
{
    private List<IConnector> mySimpleConnectors;

    public override NetworkResult Initialize()
    {
        mySimpleConnectors = new List<IConnector>();
        return NetworkResult.Success;
    }

    public override IConnector CreateConnection(NetworkPlatform platform, string svrUrl,
        bool disconnecWhenRecvBufferIsFull = false)
    {
        var connector = new SimpleConnector();
        mySimpleConnectors.Add(connector);
        return connector;
    }

    public override void DestroyConnector(IConnector connector)
    {
        if (mySimpleConnectors.Remove(connector))
        {
        }
    }

    public override NetworkState GetNetworkState()
    {
        return NetworkState.NotReachable;
    }

    public override bool IsPlatformInstalled(NetworkPlatform platform)
    {
        return false;
    }

    public override void Login(NetworkPlatform platform)
    {
        NotifyLoginEvent(NetworkResult.Success, NetworkPlatform.None, "simpleaccountid");
    }


    public override void Logout()
    {
        NotifyLogoutEvent(NetworkResult.Success);
    }

    public override NetworkResult GetRecord(ref NetworkPlatform platform, ref string accountId)
    {
        return NetworkResult.NotInitialized;
    }
}