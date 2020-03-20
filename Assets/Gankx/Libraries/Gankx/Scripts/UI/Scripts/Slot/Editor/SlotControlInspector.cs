using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Gankx.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SlotControl), true)]
    public class SlotControlInspector : Editor
    {
        public const string AssetRootPath = "Assets/Resources/" + PanelService.PanelResourcePath;

        private static void DrawLookAndFeel(SlotControl uc)
        {
            if (null == uc.cachedControlTransform)
            {
                return;
            }

            var animator = uc.cachedControlTransform.GetComponent<Animator>();
            if (null == animator)
            {
                return;
            }

            var ac = animator.runtimeAnimatorController as AnimatorController;
            if (null == ac)
            {
                return;
            }

            var allLookAndFeels = new List<string>();
            var lookAndFeelIndex = -1;

            foreach (var layer in ac.layers)
            {
                if (string.CompareOrdinal(layer.name, SlotControl.LookAndFeelLayerName) == 0)
                {
                    var sm = layer.stateMachine;

                    foreach (var state in sm.states)
                    {
                        if (state.state.nameHash == uc.lookAndFeel)
                        {
                            lookAndFeelIndex = allLookAndFeels.Count;
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

            lookAndFeelIndex = EditorGUILayout.Popup("Look And Feel", lookAndFeelIndex, allLookAndFeels.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                if (lookAndFeelIndex < 0 || lookAndFeelIndex >= allLookAndFeels.Count)
                {
                    lookAndFeelIndex = 0;
                }

                uc.lookAndFeel = Animator.StringToHash(allLookAndFeels[lookAndFeelIndex]);
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawControlPrefab(SlotControl uc)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Where");
            var prefabPath = uc.prefabPath;
            if (null == prefabPath)
            {
                prefabPath = string.Empty;
            }

            EditorGUILayout.LabelField(prefabPath);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(" ");

            var controlPrefab =
                EditorGUILayout.ObjectField(uc.controlPrefab, typeof(GameObject), true) as GameObject;

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uc, "Set ControlPrefab");
                uc.controlPrefab = controlPrefab;
                uc.DeletePreview();
                uc.prefabPath = GetControlPath(controlPrefab);
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(90f);

            var uc = target as SlotControl;

            if (null == uc)
            {
                return;
            }

            if (UIEditorTools.DrawHeader("Control"))
            {
                UIEditorTools.BeginContents();

                DrawLookAndFeel(uc);

                DrawControlPrefab(uc);

                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel(" ");

                    if (GUILayout.Button("Set Native Size"))
                    {
                        Undo.RecordObject(uc.transform, "Set Native Size");

                        uc.SetNativeSize();
                    }


                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.PrefixLabel(" ");

                if (GUILayout.Button("Save Prefab"))
                {
                    Undo.RecordObject(uc.controlPrefab, "Save Prefab");
                    uc.SavePrefab();
                }

                UIEditorTools.EndContents();
            }
        }

        public static string GetControlPath(GameObject controlPrefab)
        {
            if (null == controlPrefab)
            {
                return null;
            }

            var controlPrefabPath = AssetDatabase.GetAssetPath(controlPrefab);
            if (!controlPrefabPath.StartsWith(AssetRootPath))
            {
                Debug.LogWarning(string.Format("Control prefab ({0}) must from ({1})", controlPrefabPath,
                    AssetRootPath));
                return null;
            }

            int length;

            if ((length = controlPrefabPath.LastIndexOf('.')) == -1)
            {
                return controlPrefabPath.Substring(AssetRootPath.Length);
            }

            return controlPrefabPath.Substring(AssetRootPath.Length,
                length - AssetRootPath.Length);
        }
    }
}