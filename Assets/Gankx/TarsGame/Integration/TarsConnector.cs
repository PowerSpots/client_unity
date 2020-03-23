using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

namespace Gankx.GalaxyGame
{
    public class TarsConnector : IConnector
    {
        public event ConnectEventHandler ConnectEvent;
        public event ReconnectEventHandler ReconnectEvent;
        public event DisconnectEventHandler DisconnectEvent;
        public event RecvedDataHandler RecvedDataEvent;
        public event ConnectorErrorEventHandler ErrorEvent;

        private Socket socketSend;

        // 123.206.99.53:19836
        public NetworkResult Connect()
        {
            int _port = 19836;
            string _ip = "123.206.99.53";
            try
            {
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(_ip);
                IPEndPoint point = new IPEndPoint(ip, _port);

                socketSend.Connect(point);
                Debug.Log("连接成功!");
                //开启新的线程，不停的接收服务器发来的消息
                //Thread c_thread = new Thread(Received);
                //c_thread.IsBackground = true;
                //c_thread.Start();
            }
            catch(Exception e)
            {
                Debug.LogError("TarsConnector Exception:" + e);
            }

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
}
