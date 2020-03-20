using System;
using System.Collections.Generic;
using XLua;

namespace Gankx
{
    public class NetworkService : Singleton<NetworkService>
    {
        private bool myInitialized;

        private Dictionary<int, NetworkConnector> myConnectors;

        private List<NetworkConnector> myConnectorsList;

        private bool myOnNetworkStateChanged;

        protected override void OnInit()
        {
        }

        protected override void OnRelease()
        {
            Release();
        }

        public NetworkResult Initialize()
        {
            if (myInitialized)
            {
                return NetworkResult.Success;
            }


            var result = NetworkIntegration.instance.Initialize();
            if (result != NetworkResult.Success)
            {
                return result;
            }

            myConnectors = new Dictionary<int, NetworkConnector>();
            myConnectorsList = new List<NetworkConnector>();

            NetworkAccount.Initialize();

            NetworkIntegration.instance.NetworkChangedEvent += OnNetworkStateChanged;

            myInitialized = true;

            return result;
        }

        public void Release()
        {
            if (!myInitialized)
            {
                return;
            }

            myInitialized = false;

            if (NetworkIntegration.instance != null)
            {
                NetworkIntegration.instance.NetworkChangedEvent -= OnNetworkStateChanged;
            }

            NetworkAccount.Release();

            if (myConnectors != null)
            {
                foreach (var pair in myConnectors)
                {
                    pair.Value.Disconnect();

                    RemoveConnector(pair.Value);
                }

                myConnectors.Clear();
                myConnectorsList.Clear();
            }
        }

        public int GetNetworkState()
        {
            return (int) NetworkIntegration.instance.GetNetworkState();
        }

        private void OnNetworkStateChanged(NetworkState state)
        {
            myOnNetworkStateChanged = true;
        }

        private void Update()
        {
            if (myOnNetworkStateChanged)
            {
                myOnNetworkStateChanged = false;

                if (LuaService.instance != null)
                {
                    LuaService.instance.FireEvent("OnNetStateChanged", GetNetworkState());
                }
            }

            if (null == myConnectorsList)
            {
                return;
            }

            for (var i = myConnectorsList.Count - 1; i >= 0; --i)
            {
                myConnectorsList[i].HandleMsgIfDirty();
            }
        }

        private NetworkConnector AddConnector(int id)
        {
            var connector = new NetworkConnector();
            connector.id = id;

            connector.SetRecvEventHandler(OnRecv);
            connector.SetConnectEventHandler(OnConnect);
            connector.SetReconnectEventHandler(OnReconnect);
            connector.SetErrorEventHandler(OnError);

            return connector;
        }

        private void RemoveConnector(NetworkConnector connector)
        {
            connector.id = -1;

            connector.SetRecvEventHandler(null);
            connector.SetConnectEventHandler(null);
            connector.SetReconnectEventHandler(null);
            connector.SetErrorEventHandler(null);
        }

        public bool Connect(int id, NetworkPlatform platform, string url)
        {
            if (null == myConnectors)
            {
                return false;
            }

            NetworkConnector connector;
            if (!myConnectors.TryGetValue(id, out connector))
            {
                connector = AddConnector(id);
                myConnectors[id] = connector;
                if (!myConnectorsList.Contains(connector))
                {
                    myConnectorsList.Add(connector);
                }
            }

            return connector.Connect(platform, url);
        }

        public bool SetMsgLimitEnable(int id, bool enable)
        {
            if (null == myConnectors)
            {
                return false;
            }

            NetworkConnector connector;
            if (!myConnectors.TryGetValue(id, out connector))
            {
                return false;
            }

            connector.SetMsgLimitEnable(enable);

            return true;
        }

        public bool Reconnect(int id)
        {
            if (null == myConnectors)
            {
                return false;
            }

            NetworkConnector connector;
            if (!myConnectors.TryGetValue(id, out connector))
            {
                return false;
            }

            return connector.Reconnect();
        }

        public bool Reconnect2(int id)
        {
            if (null == myConnectors)
            {
                return false;
            }

            NetworkConnector connector;
            if (!myConnectors.TryGetValue(id, out connector))
            {
                return false;
            }

            connector.HandleMsgIfDirty(true);
            return connector.Reconnect2();
        }

        public bool ForceHandleMsg(int id)
        {
            if (null == myConnectors)
            {
                return false;
            }

            NetworkConnector connector;
            if (!myConnectors.TryGetValue(id, out connector))
            {
                return false;
            }

            connector.HandleMsgIfDirty(true);
            return true;
        }

        public void Disconnect(int id)
        {
            if (null == myConnectors)
            {
                return;
            }

            NetworkConnector connector;
            if (!myConnectors.TryGetValue(id, out connector))
            {
                return;
            }

            connector.Disconnect();

            RemoveConnector(connector);
            myConnectors.Remove(id);
            myConnectorsList.Remove(connector);
        }

        public void DisconnectConnector(int id)
        {
            if (null == myConnectors)
            {
                return;
            }

            NetworkConnector connector;
            if (!myConnectors.TryGetValue(id, out connector))
            {
                return;
            }

            connector.DisconnectConnector();
        }

        public NetworkResult Send(int id, byte[] data, int usedSize)
        {
            if (null == myConnectors)
            {
                return NetworkResult.NotCreated;
            }

            NetworkConnector connector;
            if (myConnectors.TryGetValue(id, out connector))
            {
                return connector.Send(data, usedSize);
            }

            return NetworkResult.NotCreated;
        }

        public NetworkResult SendByMsgType(int id, int msgType, byte[] msgData)
        {
            if (null == myConnectors)
            {
                return NetworkResult.NotCreated;
            }

            NetworkConnector connector;
            if (myConnectors.TryGetValue(id, out connector))
            {
                return connector.SendByMsgType(msgType, msgData);
            }

            return NetworkResult.NotCreated;
        }


        private void OnRecv(int id, DataBoat databoat)
        {
            if (databoat.len < 2)
            {
                return;
            }

            var msgType = databoat.data[0] + databoat.data[1] * 256;

            Buffer.BlockCopy(databoat.data, 2, databoat.data, 0, databoat.len - 2);
            databoat.len = databoat.len - 2;

            if (LuaService.instance != null)
            {
                LuaService.instance.FireDataEvent("OnNetRecv", id, msgType, databoat);
            }
        }

        private void OnConnect(int id, NetworkResult result, string openId)
        {
            if (LuaService.instance != null)
            {
                ForceHandleMsg(id);
                LuaService.instance.FireEvent("OnNetConnect", id, (int) result);
            }
        }

        private void OnReconnect(int id, NetworkResult result)
        {
            if (LuaService.instance != null)
            {
                ForceHandleMsg(id);
                LuaService.instance.FireEvent("OnNetReconnect", id, (int) result);
            }
        }

        private void OnError(int id, NetworkResult result)
        {
            if (LuaService.instance != null)
            {
                ForceHandleMsg(id);
                LuaService.instance.FireEvent("OnNetError", id, (int) result);
            }
        }
    }
}