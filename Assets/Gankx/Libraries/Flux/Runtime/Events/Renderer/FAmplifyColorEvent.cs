using UnityEngine;

namespace Flux
{
    // 较色效果事件
    /// <summary>
    /// TODO: 暂时Blend使用0-1的线性差值
    /// </summary>
    [FEvent("PostProcessEffect/AmplifyColor")]
    public class FAmplifyColorEvent : FEvent
    {
        [SerializeField]
        private Texture _lutTexture;
        public Texture LuaBlendTexture
        {
            get { return _lutTexture; }
            set { _lutTexture = value; }
        }

        [SerializeField]
        private Texture _lutBlendTexture;
        public Texture LutBlendTexture
        {
            get { return _lutBlendTexture; }
            set { _lutBlendTexture = value; }
        }

        protected override void OnTrigger(float timeSinceTrigger)
        {
            AmplifyColorEffect amplifyColor = Camera.main.GetComponent<AmplifyColorEffect>();
            if (amplifyColor == null)
            {
                Debug.LogError("FAmplifyColorEvent " + Camera.main + " 找不到AmplifyColorEffect！");
                return;
            }

            amplifyColor.enabled = true; // TODO: 还不确定什么时候需要enable为false，保证RenderTexture不用占太多内存，
            amplifyColor.LutTexture = _lutTexture;
            amplifyColor.LutBlendTexture = _lutBlendTexture;
            amplifyColor.BlendTo(_lutBlendTexture, this.LengthTime, null);
        }
    }
}

