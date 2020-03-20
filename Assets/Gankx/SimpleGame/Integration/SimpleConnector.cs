using System.Collections;
using System.Collections.Generic;
using Gankx;
using UnityEngine;

public class SimpleConnector : IConnector {

    public event ConnectEventHandler ConnectEvent;
    public event ReconnectEventHandler ReconnectEvent;
    public event DisconnectEventHandler DisconnectEvent;
    public event RecvedDataHandler RecvedDataEvent;
    public event ConnectorErrorEventHandler ErrorEvent;

    public NetworkResult Connect()
    {
        ConnectEvent(NetworkResult.Success, "simpleaccountid");
        return NetworkResult.Success;
    }

    public NetworkResult Connect(uint timeout)
    {
        ConnectEvent(NetworkResult.Success, "simpleaccountid");
        return NetworkResult.Success;
    }

    public NetworkResult Connect(uint timeout, uint totalTimeout)
    {
        ConnectEvent(NetworkResult.Success, "simpleaccountid");
        return NetworkResult.Success;
    }

    public NetworkResult Reconnect()
    {
        ReconnectEvent(NetworkResult.Success);
        return NetworkResult.Success;
    }

    public NetworkResult Reconnect(uint timeout)
    {
        ReconnectEvent(NetworkResult.Success);
        return NetworkResult.Success;
    }

    public NetworkResult Disconnect()
    {
        DisconnectEvent(NetworkResult.Success);
        return NetworkResult.Success;
    }

    public NetworkResult WriteData(byte[] data, int len = -1)
    {
        return NetworkResult.Success;
    }

    public NetworkResult ReadData(out byte[] buffer)
    {
        buffer = null;
        return NetworkResult.Success;
    }

    public NetworkResult ReadData(ref byte[] buffer, ref int realLength)
    {
        realLength = 0;
        return NetworkResult.Success;
    }
}
