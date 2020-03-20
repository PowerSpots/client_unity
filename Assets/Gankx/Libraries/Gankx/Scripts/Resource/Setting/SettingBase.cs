using UnityEngine;

namespace Gankx
{
    public class SettingBase<T> : ScriptableObject where T : SettingBase<T>
    {
        protected virtual void OnInit()
        {
        }

        public void Initialize()
        {
            // dummy
        }
    }
}