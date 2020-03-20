using UnityEngine;

namespace Gankx.UI
{
    public class EventDispatcher : MonoBehaviour
    {
        protected Window window;

        private void Awake()
        {
            window = Window.GetWindowInParent(gameObject);

            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        protected void SendPanelMessage(string message, object param1 = null, object param2 = null)
        {
            if (LuaService.instance != null && window != null)
            {
                LuaService.instance.FireEvent(
                    "OnPanelMessage", window.panelId, window.id, message, param1, param2);
            }
        }

        protected new T GetComponent<T>()
        {
            var control = base.GetComponent<SlotControl>();
            if (null != control)
            {
                return control.cachedControlTransform.GetComponent<T>();
            }

            return base.GetComponent<T>();
        }

        protected T AddComponent<T>() where T : Component
        {
            var control = base.GetComponent<SlotControl>();
            if (null != control)
            {
                return control.cachedControlTransform.gameObject.AddComponent<T>();
            }

            return gameObject.AddComponent<T>();
        }
    }
}