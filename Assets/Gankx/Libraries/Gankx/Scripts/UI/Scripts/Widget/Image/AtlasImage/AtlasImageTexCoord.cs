using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    [ExecuteInEditMode]
    public class AtlasImageTexCoord : MonoBehaviour
    {
        private Image myAtlasImage;

        private void Start()
        {
            myAtlasImage = GetComponent<AtlasImage>();
        }

        private void Update()
        {
            if (myAtlasImage && myAtlasImage.overrideSprite != null)
            {
                var pos = myAtlasImage.canvas.transform.worldToLocalMatrix.MultiplyPoint(
                    myAtlasImage.transform.position);

                myAtlasImage.material.SetVector("_Offset", new Vector4(myAtlasImage.rectTransform.rect.width,
                    myAtlasImage.rectTransform.rect.height, pos.x, pos.y));
            }
        }
    }
}