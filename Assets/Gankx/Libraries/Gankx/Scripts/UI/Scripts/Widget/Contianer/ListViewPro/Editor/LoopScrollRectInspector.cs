using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(LoopScrollRect), true)]
public class LoopScrollRectInspector : Editor
{
	public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        LoopScrollRect scroll = (LoopScrollRect)target;
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Clear"))
        {
            scroll.ClearCells();
        }
        if (GUILayout.Button("Refresh"))
        {
            scroll.RefreshCells();
		}
		if(GUILayout.Button("Refill"))
		{
			scroll.RefillCells(0);
		}
		if(GUILayout.Button("RefillFromEnd"))
		{
			scroll.RefillCellsFromEnd(0);
		}
        EditorGUILayout.EndHorizontal();
	}
}