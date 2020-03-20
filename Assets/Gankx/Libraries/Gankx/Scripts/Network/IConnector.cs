namespace Gankx
{
    public delegate void ConnectEventHandler(NetworkResult result , string accountId);

    public delegate void ReconnectEventHandler(NetworkResult result);

    public delegate void DisconnectEventHandler(NetworkResult result);

    public delegate void ConnectorErrorEventHandler(NetworkResult result);

    public delegate void RecvedDataHandler();

    public delegate void RecvedUdpDataHandler();

    public interface IConnector
    {
        event ConnectEventHandler ConnectEvent;
        event ReconnectEventHandler ReconnectEvent;
        event DisconnectEventHandler DisconnectEvent;
        event RecvedDataHandler RecvedDataEvent;
        event ConnectorErrorEventHandler ErrorEvent;

        NetworkResult Connect();
        NetworkResult Connect(uint timeout);
        NetworkResult Connect(uint timeout, uint totalTimeout);

        NetworkResult Reconnect();
        NetworkResult Reconnect(uint timeout);

        NetworkResult Disconnect();

        NetworkResult WriteData(byte[] data, int len = -1);

        NetworkResult ReadData(out byte[] buffer);
        NetworkResult ReadData(ref byte[] buffer, ref int realLength);

    }
}