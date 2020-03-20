using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Gankx.UI
{
    [CustomEditor(typeof(LookAndFeel))]
    public class LookAndFeelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(90f);

            LookAndFeel lookAndFeel = target as LookAndFeel;

            Animator animator = lookAndFeel.GetComponent<Animator>();
            if (null == animator)
            {
                return;
            }

            AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
            if (null == ac)
            {
                return;
            }

            List<string> allLookAndFeels = new List<string>();
            int currentIndex = -1;

            foreach (AnimatorControllerLayer layer in ac.layers)
            {
                if (String.CompareOrdinal(layer.name, LookAndFeel.LookAndFeelLayerName) == 0)
                {
                    AnimatorStateMachine sm = layer.stateMachine;

                    foreach (ChildAnimatorState state in sm.states)
                    {
                        if (state.state.nameHash == lookAndFeel.Current)
                        {
                            currentIndex = allLookAndFeels.Count;
                        }

                        allLookAndFeels.Add(state.state.name);
                    }

                    break;
                }
            }

            if (allLookAndFeels.Count <= 0)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            currentIndex = EditorGUILayout.Popup("", currentIndex, allLookAndFeels.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                // 不支持Undo操作
                if (currentIndex < 0 || currentIndex >= allLookAndFeels.Count)
                {
                    currentIndex = 0;
                }

                lookAndFeel.Current = Animator.StringToHash(allLookAndFeels[currentIndex]);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
