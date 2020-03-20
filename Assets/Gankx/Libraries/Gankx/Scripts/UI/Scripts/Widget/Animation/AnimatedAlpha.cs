using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    [ExecuteInEditMode]
    public class AnimatedAlpha : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float alpha = 1f;

        MaskableGraphic mMaskableGraphic;

        void OnEnable()
        {
            mMaskableGraphic = GetComponent<MaskableGraphic>();
            LateUpdate();
        }

        void LateUpdate()
        {
            if (mMaskableGraphic != null)
            {
                Color color = mMaskableGraphic.color;
                color.a = alpha;
                mMaskableGraphic.color = color;
            }
        }
    }
}