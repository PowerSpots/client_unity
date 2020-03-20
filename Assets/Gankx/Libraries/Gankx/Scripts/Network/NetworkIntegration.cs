using UnityEngine;

namespace Gankx
{
    public enum NetworkState
    {
        NotReachable, //none
        ReachableViaWWAN, //mobile
        ReachableViaWiFi, //wifi
        ReachableViaOthers //others
    }

    public delegate void NetworkStateChanged(NetworkState state);

    public delegate void AccountLoginHandle(NetworkResult result, NetworkPlatform platform, string accountId);

    public delegate void AccountLogoutHandle(NetworkResult result);

    [DisallowMultipleComponent]
    public abstract class NetworkIntegration : IntegrationBase<NetworkIntegration>
    {
        public event NetworkStateChanged NetworkChangedEvent;
        public event AccountLoginHandle LoginEvent;
        public event AccountLogoutHandle LogoutEvent;

        public abstract NetworkResult Initialize();

        public abstract IConnector CreateConnection(NetworkPlatform platform, string svrUrl,
            bool disconnecWhenRecvBufferIsFull = false);

        public abstract void DestroyConnector(IConnector connector);

        public abstract NetworkState GetNetworkState();

        public abstract bool IsPlatformInstalled(NetworkPlatform platform);

        protected void NotifyNetworkChangedEvent(NetworkState state)
        {
            if (NetworkChangedEvent != null)
            {
                NetworkChangedEvent(state);
            }
        }

        public abstract void Login(NetworkPlatform platform);

        protected void NotifyLoginEvent(NetworkResult result, NetworkPlatform platform, string accountId)
        {
            if (LoginEvent != null)
            {
                LoginEvent(result, platform, accountId);
            }
        }

        public abstract void Logout();

        protected void NotifyLogoutEvent(NetworkResult result)
        {
            if (LogoutEvent != null)
            {
                LogoutEvent(result);
            }
        }

        public abstract NetworkResult GetRecord(ref NetworkPlatform platform, ref string accountId);
    }
}