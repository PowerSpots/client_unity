using System;

namespace Gankx
{
    public static class NetworkAccount
    {
        private enum LoginType
        {
            None,

            Login
        }

        private static LoginType MyLoginType = LoginType.None;

        public static string accountId { get; set; }

        public static NetworkPlatform platform { get; set; }

        public static void Initialize()
        {
            accountId = string.Empty;
            platform = NetworkPlatform.None;

            var accountService = NetworkIntegration.instance;
            if (accountService != null)
            {
                accountService.LoginEvent += OnLogin;
                accountService.LogoutEvent += OnLogout;
            }
        }

        public static void Release()
        {
            var accountService = NetworkIntegration.instance;
            if (accountService != null)
            {
                accountService.LoginEvent -= OnLogin;
                accountService.LogoutEvent -= OnLogout;
            }
        }

        public static bool IsPlatformInstalled(NetworkPlatform platformParam)
        {
            var accountService = NetworkIntegration.instance;
            if (null == accountService)
            {
                return false;
            }

            return accountService.IsPlatformInstalled(platformParam);
        }

        public static NetworkResult Login(NetworkPlatform platformParam)
        {
            var accountService = NetworkIntegration.instance;

            if (null == accountService)
            {
                return NetworkResult.Error;
            }

            if (MyLoginType != LoginType.None)
            {
                return NetworkResult.Error;
            }

            MyLoginType = LoginType.Login;
            accountService.Login(platformParam);

            return NetworkResult.Success;
        }

        public static NetworkResult GetRecord()
        {
            var accountService = NetworkIntegration.instance;
            if (null == accountService)
            {
                return NetworkResult.Error;
            }

            var recordPlatform = NetworkPlatform.None;
            var recordAccountId = string.Empty;
            var result = accountService.GetRecord(ref recordPlatform, ref recordAccountId);

            if (result != NetworkResult.Success)
            {
                return result;
            }

            accountId = recordAccountId;
            platform = recordPlatform;

            if (LuaService.instance != null)
            {
                LuaService.instance.FireEvent("OnNetGetRecord", (int) recordPlatform, recordAccountId);
            }

            return NetworkResult.Success;
        }

        private static void OnLogin(NetworkResult result, NetworkPlatform platformParam, string accountIdParam)
        {
            if (MyLoginType == LoginType.Login)
            {
                accountId = accountIdParam;
                platform = platformParam;

                if (LuaService.instance != null)
                {
                    LuaService.instance.FireEvent("OnNetLogin", (int) result, (int) platformParam, accountIdParam);
                }
            }

            MyLoginType = LoginType.None;
        }

        private static void OnLogout(NetworkResult result)
        {
        }

        public static void Logout()
        {
            accountId = string.Empty;
            platform = NetworkPlatform.None;

            var accountService = NetworkIntegration.instance;
            if (null == accountService)
            {
                return;
            }

            accountService.Logout();
        }

        private static string GetTimeStamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (Convert.ToInt64(ts.TotalMilliseconds) / 1000).ToString();
        }
    }
}