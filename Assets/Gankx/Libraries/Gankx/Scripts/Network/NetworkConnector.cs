using System;
using XLua;

namespace Gankx
{
    public class NetworkConnector
    {
        public delegate void ConnectEventHandler(int id, NetworkResult result, string openId);

        public delegate void ReconnectEventHandler(int id, NetworkResult result);

        public delegate void ErrorEventHandler(int id, NetworkResult result);

        public delegate void RecvEventHandler(int id, DataBoat databoat);

        private const int MaxDataboatBufferSize = 100 * 1024;
        private const float MinHandleLen = 200;
        private const float MaxHandleLen = 1600;

        private static DataBoat Databoat;

        private float myCurHandleInit = MinHandleLen;
        private float myCurHandleLen = MinHandleLen;

        private bool myMsgDirty;
        private bool myMsgLimited = true;

        private ConnectEventHandler myConnectEventHandler;
        private ReconnectEventHandler myReconnectEventHandler;
        private ErrorEventHandler myErrorEventHandler;
        private RecvEventHandler myRecvEventHandler;

        private IConnector myConnector;

        public int id { get; set; }

        public void SetConnectEventHandler(ConnectEventHandler handler)
        {
            myConnectEventHandler = handler;
        }

        public void SetReconnectEventHandler(ReconnectEventHandler handler)
        {
            myReconnectEventHandler = handler;
        }

        public void SetErrorEventHandler(ErrorEventHandler handler)
        {
            myErrorEventHandler = handler;
        }

        public void SetRecvEventHandler(RecvEventHandler handler)
        {
            myRecvEventHandler = handler;
        }

        public bool Connect(NetworkPlatform platform, string url)
        {
            Disconnect();

            var connector = NetworkIntegration.instance.CreateConnection(
                platform, url, true);

            if (null == connector)
            {
                return false;
            }

            connector.RecvedDataEvent += OnRecvEvent;
            connector.ConnectEvent += OnConnectEvent;
            connector.ReconnectEvent += OnReconnectEvent;
            connector.ErrorEvent += OnErrorEvent;

            if (connector.Connect(10) != NetworkResult.Success)
            {
                return false;
            }

            myConnector = connector;

            return true;
        }

        public bool Reconnect()
        {
            if (null == myConnector)
            {
                return false;
            }

            return myConnector.Reconnect(10) == NetworkResult.Success;
        }

        public bool Reconnect2()
        {
            if (null == myConnector)
            {
                return false;
            }

            myConnector.Disconnect();

            return myConnector.Connect(10) == NetworkResult.Success;
        }

        public void DisconnectConnector()
        {
            if (null == myConnector)
            {
                return;
            }

            myConnector.Disconnect();
        }

        public void Disconnect()
        {
            if (null == myConnector)
            {
                return;
            }


            var connector = myConnector;
            myConnector = null;

            connector.RecvedDataEvent -= OnRecvEvent;
            connector.ConnectEvent -= OnConnectEvent;
            connector.ReconnectEvent -= OnReconnectEvent;
            connector.ErrorEvent -= OnErrorEvent;

            connector.Disconnect();
            NetworkIntegration.instance.DestroyConnector(connector);
        }

        public NetworkResult Send(byte[] data, int usedSize)
        {
            if (null == myConnector)
            {
                return NetworkResult.Disconnected;
            }

            return myConnector.WriteData(data, usedSize);
        }

        public NetworkResult SendByMsgType(int msgType, byte[] msgData)
        {
            if (null == Databoat.data)
            {
                Databoat.data = new byte[MaxDataboatBufferSize];
            }

            var i1 = msgType / 256;
            var i0 = msgType - i1 * 256;

            var b0 = BitConverter.GetBytes(i0);
            var b1 = BitConverter.GetBytes(i1);

            Databoat.data[0] = b0[0];
            Databoat.data[1] = b1[0];

            Buffer.BlockCopy(msgData, 0, Databoat.data, 2, msgData.Length);
            Databoat.len = msgData.Length + 2;
            return Send(Databoat.data, Databoat.len);
        }

        private void OnRecvEvent()
        {
            myMsgDirty = true;
        }

        public void SetMsgLimitEnable(bool enable)
        {
            myMsgLimited = enable;
        }

        public void HandleMsgIfDirty(bool isHandleAll = false)
        {
            if (!myMsgDirty)
            {
                if (Math.Abs(myCurHandleLen - myCurHandleInit) < float.Epsilon)
                {
                    myCurHandleInit *= MaxHandleLen / (myCurHandleInit + MaxHandleLen);
                    if (myCurHandleInit < MinHandleLen)
                    {
                        myCurHandleInit = MinHandleLen;
                    }
                }

                return;
            }

            var totalLen = 0;
            do
            {
                if (null == myConnector)
                {
                    return;
                }

                if (null == Databoat.data)
                {
                    Databoat.data = new byte[MaxDataboatBufferSize];
                }

                Databoat.len = 0;

                var result = myConnector.ReadData(ref Databoat.data, ref Databoat.len);

                if (result != NetworkResult.Success)
                {
                    myMsgDirty = false;

                    if (Math.Abs(myCurHandleLen - myCurHandleInit) < float.Epsilon)
                    {
                        myCurHandleInit *= MaxHandleLen / (myCurHandleInit + MaxHandleLen);
                        if (myCurHandleInit < MinHandleLen)
                        {
                            myCurHandleInit = MinHandleLen;
                        }
                    }
                    else
                    {
                        myCurHandleInit = (myCurHandleLen + myCurHandleInit) / 2;
                        myCurHandleLen = myCurHandleInit;
                    }

                    return;
                }

                if (myRecvEventHandler != null)
                {
                    myRecvEventHandler(id, Databoat);
                }

                if (!isHandleAll && myMsgLimited)
                {
                    totalLen += Databoat.len;

                    if (totalLen > myCurHandleLen)
                    {
                        myCurHandleLen *= 1.1f;
                        if (myCurHandleLen > MaxHandleLen)
                        {
                            myCurHandleLen = MaxHandleLen;
                        }

                        return;
                    }
                }
            } while (true);
        }

        private void OnConnectEvent(NetworkResult result, string accountId)
        {
            if (myConnectEventHandler != null)
            {
                myConnectEventHandler(id, result, accountId);
            }
        }

        private void OnReconnectEvent(NetworkResult result)
        {
            if (myReconnectEventHandler != null)
            {
                myReconnectEventHandler(id, result);
            }
        }

        private void OnErrorEvent(NetworkResult result)
        {
            if (myErrorEventHandler != null)
            {
                myErrorEventHandler(id, result);
            }
        }
    }
}