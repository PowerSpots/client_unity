using UnityEngine;
using UnityEditor;

namespace Gankx.UI
{
    [CustomEditor(typeof(TweenWidth))]
    public class TweenWidthEditor : TweenerEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(120f);

            TweenWidth tw = target as TweenWidth;
            GUI.changed = false;

            float from = EditorGUILayout.FloatField("From", tw.from);
            float to = EditorGUILayout.FloatField("To", tw.to);

            if (from < 0) from = 0;
            if (to < 0) to = 0;

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