using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gankx.UI
{
    public class DynamicImageSpriteLoadedDispatcher : EventDispatcher
    {       
        protected override void OnInit()
        {
            DynamicImage di = gameObject.GetOrAddComponent<DynamicImage>();
            di.onLoaded.AddListener(OnSpriteLoaded);
        }

        public void OnSpriteLoaded(bool isAsync)
        {
            SendPanelMessage("OnSpriteLoaded", isAsync);
        }
    }
}
