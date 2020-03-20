using UnityEngine;
using UnityEditor;

namespace Gankx.UI
{
    [CustomEditor(typeof(TweenOrthoSize))]
    public class TweenOrthoSizeEditor : TweenerEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(120f);

            TweenOrthoSize tw = target as TweenOrthoSize;
            GUI.changed = false;

            float from = EditorGUILayout.FloatField("From", tw.from);
            float to = EditorGUILayout.FloatField("To", tw.to);

            if (from < 0f) from = 0f;
            if (to < 0f) to = 0f;

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