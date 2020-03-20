﻿using UnityEngine;
using UnityEditor;

namespace Gankx.UI
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(Tweener), true)]
    public class TweenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(110f);
            base.OnInspectorGUI();
            DrawCommonProperties();
        }

        protected void DrawCommonProperties()
        {
            Tweener tw = target as Tweener;

            if (UIEditorTools.DrawHeader("Tweener"))
            {
                UIEditorTools.BeginContents();
                UIEditorTools.SetLabelWidth(110f);

                GUI.changed = false;

                Tweener.Style style = (Tweener.Style) EditorGUILayout.EnumPopup("Play Style", tw.style);
                AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.animationCurve,
                    GUILayout.Width(170f), GUILayout.Height(62f));
                //UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);

                GUILayout.BeginHorizontal();
                float dur = EditorGUILayout.FloatField("Duration", tw.duration, GUILayout.Width(170f));
                GUILayout.Label("seconds");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(170f));
                GUILayout.Label("seconds");
                GUILayout.EndHorizontal();

                int tg = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(170f));
                bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);

                if (GUI.changed)
                {
                    UIEditorTools.RegisterUndo("Tween Change", tw);
                    tw.animationCurve = curve;
                    //tw.method = method;
                    tw.style = style;
                    tw.ignoreTimeScale = ts;
                    tw.tweenGroup = tg;
                    tw.duration = dur;
                    tw.delay = del;
                    UITools.SetDirty(tw);
                }
                UIEditorTools.EndContents();
            }

            UIEditorTools.SetLabelWidth(80f);
#if COMPATIBILITY_NGUI
        UIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);
#endif
        }
    }
}