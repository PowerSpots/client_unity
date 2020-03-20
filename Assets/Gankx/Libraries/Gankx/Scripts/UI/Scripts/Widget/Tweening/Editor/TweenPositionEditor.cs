using UnityEngine;
using UnityEditor;

namespace Gankx.UI
{
    [CustomEditor(typeof(TweenPosition))]
    public class TweenPositionEditor : TweenerEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(120f);

            TweenPosition tw = target as TweenPosition;
            GUI.changed = false;

            Vector3 from = EditorGUILayout.Vector3Field("From", tw.from);
            Vector3 to = EditorGUILayout.Vector3Field("To", tw.to);

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