using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Gankx.GalaxyGame
{
    public class TarsNetworkIntegration : NetworkIntegration
    {
        private List<IConnector> myConnectors;

        public override NetworkResult Initialize()
        {
            Debug.Log("TarsNetworkIntegration Initialize");
            myConnectors = new List<IConnector>();
            var con = new TarsConnector();
            con.ConnectEvent += Con_ConnectEvent;
            con.Connect();
            myConnectors.Add(con);
            return NetworkResult.Success;
        }

        private void Con_ConnectEvent(NetworkResult result, string accountId)
        {
            Debug.Log("Con_ConnectEvent Callback");
        //    throw new NotImplementedException();
        }

        public override IConnector CreateConnection(NetworkPlatform platform, string svrUrl,
            bool disconnecWhenRecvBufferIsFull = false)
        {
            Debug.Log("TarsNetworkIntegration CreateConnection");
            var connector = new TarsConnector();
            myConnectors.Add(connector);
            return connector;
        }

        public override void DestroyConnector(IConnector connector)
        {
            Debug.Log("TarsNetworkIntegration DestroyConnector");
            if (myConnectors.Remove(connector))
            {
            }
        }

        public override NetworkState GetNetworkState()
        {
            Debug.Log("TarsNetworkIntegration GetNetworkState");
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
}
