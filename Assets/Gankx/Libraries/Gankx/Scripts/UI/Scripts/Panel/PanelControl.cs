using UnityEngine;
using UnityEngine.Serialization;

namespace Gankx.UI
{
    public class PanelControl : MonoBehaviour
    {
        public int sortOrder = 0;
        public int depth = 0;
        public PanelLayerType layer = PanelLayerType.Foreground;

        private Canvas myCanvas;

        void OnEnable()
        {
            if (sortOrder == 0)
            {
                return;
            }

            if (myCanvas == null)
            {
                myCanvas = GetComponent<Canvas>();
            }

            if (myCanvas != null)
            {
                myCanvas.overrideSorting = true;
            }
        }
    }
}