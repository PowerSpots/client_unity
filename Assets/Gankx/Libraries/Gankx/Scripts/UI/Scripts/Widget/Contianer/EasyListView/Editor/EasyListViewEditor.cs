using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UIWidgets;

[CanEditMultipleObjects]
[CustomEditor(typeof(EasyListView), true)]
public class EasyListViewEditor : Editor
{
    Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

    private string[] properties = new string[]
    {
        "Container",
        "DefaultItem",
        "scrollRect",
        "direction",
        "EndScrollDelay"
    };

    protected virtual void OnEnable()
    {
        Array.ForEach(properties, x =>
        {
            var p = serializedObject.FindProperty(x);
            serializedProperties.Add(x, p);
        });
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < properties.Length; i++)
        {
            string propertyName = properties[i];
            EditorGUILayout.PropertyField(serializedProperties[propertyName]);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
