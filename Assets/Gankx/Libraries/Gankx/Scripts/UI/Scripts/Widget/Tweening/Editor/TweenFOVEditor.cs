using UnityEngine;
using UnityEditor;

namespace Gankx.UI
{
    [CustomEditor(typeof(TweenFOV))]
    public class TweenFOVEditor : TweenerEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(120f);

            TweenFOV tw = target as TweenFOV;
            GUI.changed = false;

            float from = EditorGUILayout.Slider("From", tw.from, 1f, 180f);
            float to = EditorGUILayout.Slider("To", tw.to, 1f, 180f);

            if (GUI.changed)
            {
                UIEditorTools.RegisterUndo("Tween Change", tw);
                tw.from = from;
                tw.to = to;
                UITools.SetDirty(tw);
            }

            DrawCommonProperties();
        }
    }
}