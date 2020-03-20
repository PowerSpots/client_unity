using UnityEngine;
using UnityEditor;
using System.Collections;
using Gankx;

[CustomEditor(typeof(UIAnchor))]
public class UIAnchorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            return;
        }

        GUILayout.Space(6f);        

        UIAnchor uiAnchor = target as UIAnchor;
        GUI.changed = false;

        EditorGUILayout.BeginHorizontal();
        uiAnchor.AnchorTarget = (RectTransform)EditorGUILayout.ObjectField("Target", uiAnchor.AnchorTarget, typeof(RectTransform));
        EditorGUILayout.EndHorizontal();
        if (GUI.changed)
        {
            uiAnchor.UpdateAnchorData();
        }

        GUI.changed = false;
        EditorGUILayout.BeginHorizontal();
        var leftAnchor = (RectTransform.Edge)EditorGUILayout.EnumPopup("Left Anchor", uiAnchor.leftAnchor);        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var rightAnchor = (RectTransform.Edge)EditorGUILayout.EnumPopup("Right Anchor", uiAnchor.rightAnchor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var bottomAnchor = (RectTransform.Edge)EditorGUILayout.EnumPopup("Bottom Anchor", uiAnchor.bottomAnchor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var topAnchor = (RectTransform.Edge)EditorGUILayout.EnumPopup("Top Anchor", uiAnchor.topAnchor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var leftDelta = EditorGUILayout.FloatField("Left Delta", uiAnchor.LeftDelta);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var rightDelta = EditorGUILayout.FloatField("Right Delta", uiAnchor.RightDelta);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var topDelta = EditorGUILayout.FloatField("Top Delta", uiAnchor.TopDelta);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var bottomDelta = EditorGUILayout.FloatField("Bottom Delta", uiAnchor.BottomDelta);
        EditorGUILayout.EndHorizontal();

        

        if (GUI.changed)
        {
            uiAnchor.leftAnchor = leftAnchor;
            uiAnchor.rightAnchor = rightAnchor;
            uiAnchor.topAnchor = topAnchor;
            uiAnchor.bottomAnchor = bottomAnchor;
            uiAnchor.LeftDelta = leftDelta;
            uiAnchor.RightDelta = rightDelta;
            uiAnchor.TopDelta = topDelta;
            uiAnchor.BottomDelta = bottomDelta;
        }
        else if (uiAnchor.transform.hasChanged)
        {
            // 如果是自身的transform改变，那么根据当前跟target的相对位置关系来更新anchored data
            uiAnchor.UpdateAnchorData();
            serializedObject.ApplyModifiedProperties();
            uiAnchor.transform.hasChanged = false;
        }
        else if(uiAnchor.AnchorTarget != null && uiAnchor.AnchorTarget.hasChanged)
        {                        
            // 否则根据当前的anchor data去做一次anchor
            uiAnchor.DoAnchor();
            serializedObject.ApplyModifiedProperties();
            uiAnchor.AnchorTarget.hasChanged = false;
        }        
    }    
}