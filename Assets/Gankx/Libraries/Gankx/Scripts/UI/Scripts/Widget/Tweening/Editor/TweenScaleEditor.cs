using UnityEngine;
using UnityEditor;

namespace Gankx.UI
{
    [CustomEditor(typeof(TweenScale))]
    public class TweenScaleEditor : TweenerEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(120f);

            TweenScale tw = target as TweenScale;
            GUI.changed = false;

            Vector3 from = EditorGUILayout.Vector3Field("From", tw.from);
            Vector3 to = EditorGUILayout.Vector3Field("To", tw.to);
            bool table = EditorGUILayout.Toggle("Update Table", tw.updateTable);

            if (GUI.changed)
            {
                UIEditorTools.RegisterUndo("Tween Change", tw);
                tw.from = from;
                tw.to = to;
                tw.updateTable = table;
                UITools.SetDirty(tw);
            }

            DrawCommonProperties();
        }
    }
}