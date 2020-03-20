using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MaskableGraphic))]
    public class AnimatedColor : MonoBehaviour
    {
        public Color color = Color.white;

        MaskableGraphic mMaskableGraphic;

        void OnEnable()
        {
            mMaskableGraphic = GetComponent<MaskableGraphic>();
            LateUpdate();
        }

        void LateUpdate()
        {
            mMaskableGraphic.color = color;
        }
    }
}