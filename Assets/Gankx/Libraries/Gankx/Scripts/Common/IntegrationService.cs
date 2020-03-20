using UnityEngine;

namespace Gankx
{
    public class IntegrationService : Singleton<IntegrationService>
    {
        public T GetIntegration<T>() where T : MonoBehaviour
        {
            return gameObject.GetComponent<T>();
        }
    }
}