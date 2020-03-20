using Gankx.UI;
using UnityEditor;
using UnityEngine;

namespace Gankx.UIEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DynamicImage), true)]
    public class DynamicImageInspector : Editor
    {
        private Sprite myOldsprite;

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }

            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(90f);

            var di = target as DynamicImage;

            UIEditorTools.BeginContents();

            var image = di.GetComponent<AtlasImage>();
            if (null == image)
            {
                return;
            }

            var sprite = (Sprite) EditorGUILayout.ObjectField("Sprite", image.sprite, typeof(Sprite));
            if (myOldsprite != sprite)
            {
                myOldsprite = sprite;
                if (null == sprite)
                {
                    di.path = null;
                    DestroyImmediate(di);
                    return;
                }

                var path = AssetDatabase.GetAssetPath(sprite);
                path = path.Substring(0, path.LastIndexOf('.'));
                if (path.StartsWith(UIConfig.IconAssetResourcePath))
                {
                    var relativePath = path.Substring(17);
                    di.path = relativePath;
                }
                else
                {
                    di.path = null;
                    DestroyImmediate(di);
                    return;
                }

                di.Reload();
            }

            di.async = EditorGUILayout.Toggle("Load Async", di.async);

            UIEditorTools.EndContents();
        }
    }
}