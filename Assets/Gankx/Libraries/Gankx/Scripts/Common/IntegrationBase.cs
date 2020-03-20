using UnityEngine;

namespace Gankx
{
    public class IntegrationBase<T> : MonoBehaviour where T : IntegrationBase<T>
    {
        private static T MyInstance;

        public static T instance
        {
            get
            {
                if (MyInstance == null)
                {
                    MyInstance = IntegrationService.instance.GetIntegration<T>();
                }

                if (MyInstance == null)
                {
                    Debug.LogError("GetIntegration Error");
                }

                return MyInstance;
            }
        }
    }
}