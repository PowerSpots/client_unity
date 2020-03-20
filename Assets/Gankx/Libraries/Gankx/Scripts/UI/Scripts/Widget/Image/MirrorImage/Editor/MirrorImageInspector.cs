using Gankx.UI;
using UnityEditor;
using UnityEngine;

namespace Gankx.UIEditor
{
    [CustomEditor(typeof(MirrorImage), true)]
    [CanEditMultipleObjects]
    public class MirrorImageInspector : Editor
    {
        private SerializedProperty myMirrorType;
        private GUIContent myCorrectButtonContent;
        private GUIContent myMirrorTypeContent;

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnEnable()
        {
            myCorrectButtonContent = new GUIContent("Set Native Size", "Sets the size to match the content.");
            myMirrorTypeContent = new GUIContent("Mirror Type");
            myMirrorType = serializedObject.FindProperty("myMirrorType");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(myMirrorType, myMirrorTypeContent);

            if (GUILayout.Button(myCorrectButtonContent, EditorStyles.miniButton))
            {
                var len = targets.Length;

                for (var i = 0; i < len; i++)
                {
                    var mirrorImage = targets[i] as MirrorImage;
                    if (mirrorImage != null)
                    {
                        Undo.RecordObject(mirrorImage.rectTransform, "Set Native Size");
                        mirrorImage.SetNativeSize();
                        EditorUtility.SetDirty(mirrorImage);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}