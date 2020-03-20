using UnityEngine;

namespace Gankx
{
    public class SettingService : Singleton<SettingService>
    {
        public T GetSetting<T>(string assetPath) where T : ScriptableObject
        {
            return ResourceService.Load<T>(assetPath);
        }
    }
}