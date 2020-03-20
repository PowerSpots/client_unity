using Gankx;
using UnityEngine;
using UnityEngine.UI;

public class UIImageGrayEffect : MonoBehaviour
{
    private const string MatPath = "ui/mat/UIGrayEffect";
    private const string Mat2Path = "ui/mat/UISoftClipGrayEffect";
    private bool _gray;

    private Material _temp;

    private MaskableGraphic _image;

    private MaskableGraphic GetImage
    {
        get
        {
            if (_image != null) return _image;
            _image = GetComponent<MaskableGraphic>();
            return _image;
        }
    }

    public bool Gray
    {
        set
        {
            var rect = GetComponentInParent<UISoftClip>();

            if (null != rect)
            {
                GetImage.material = value ? rect.MatGray : rect.MatBase;
                return;
            }

            if (!_gray && value)
            {
                if (GetImage.material.name.Contains("UISoftClip"))
                {
                    _temp = GetImage.material;
                    GetImage.material = ResourceService.Load<Material>(Mat2Path);
//                    GetImage.material.SetFloat("_UseUIGray",1f);
//                    GetImage.material.EnableKeyword("UI_SOFT_GRAY");
                }
                else
                {
                    _temp = GetImage.material;
                    GetImage.material = ResourceService.Load<Material>(MatPath);
                }


            }
            if (_gray && !value)
            {
                GetImage.material = _temp;              
            }
            _gray = value;
        }
    }
#if UNITY_EDITOR
    [ContextMenu("TestTrue")]
    public void TestTrue()
    {
        Gray = true;
    }

    [ContextMenu("TestFalse")]
    public void TestFalse()
    {
        Gray = false;
    }
#endif

}
