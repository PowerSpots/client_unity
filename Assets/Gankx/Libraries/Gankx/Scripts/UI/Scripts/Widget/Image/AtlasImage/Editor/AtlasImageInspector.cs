using Gankx.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Gankx.UIEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AtlasImage), true)]
    public class AtlasImageInspector : ImageEditor
    {
        private Sprite myOldsprite;
        private AtlasImage myImage;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                return;
            }

            if (null == myImage)
            {
                myImage = target as AtlasImage;
            }

            if (null == myImage)
            {
                return;
            }

            var sprite = myImage.sprite;
            if (null != sprite && myOldsprite != sprite)
            {
                myOldsprite = sprite;

                var path = AssetDatabase.GetAssetPath(sprite);

                if (path.StartsWith(UIConfig.IconAssetResourcePath))
                {
                    myImage.GetOrAddComponent<DynamicImage>();
                }
            }
        }
    }
}