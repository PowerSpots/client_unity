using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System;

// ReSharper disable PossibleNullReferenceException
namespace Gankx.UI
{
    public class SlotControlTool
    {
        public const string PanelAssetPath = "Assets/Resources/" + UIConfig.PanelSimplePath;
        public const string SlotControlAssetPath = PanelAssetPath + "control/";

        private static string SyncSlotControlPath = "";

        [MenuItem("Tools/Slot/SyncAllSlotControlsInAllPanels")]
        public static void SyncAllSlotControlsInAllPanels()
        {
            Debug.Log("SyncAllSlotControlsInAllPanels Begin");
            var start = DateTime.Now;

            {
                SetSyncSlotControlPath(null);
                TraverseAllPanelsAndSyncSlotControls();
            }

            Debug.Log("SyncAllSlotControlsInAllPanels End! Elapsed-Time: " + (DateTime.Now - start).TotalMilliseconds +
                      " ms");
        }


        [MenuItem("Tools/Slot/SyncCurrentSlotControlInAllPanels")]
        public static void SyncCurrentSlotControlsInAllPanels()
        {
            Debug.Log("SyncCurrentSlotControlInAllPanels Begin");
            var start = DateTime.Now;

            {
                var selectedObj = Selection.activeObject;
                if (selectedObj)
                {
                    Debug.Log("selected object : " + selectedObj.name);

                    var prefab = PrefabUtility.GetPrefabParent(selectedObj);
                    var assetPath = AssetDatabase.GetAssetPath(prefab);
                    Debug.Log("selected object AssetPath : " + assetPath);
                    SetSyncSlotControlPath(assetPath);
                    TraverseAllPanelsAndSyncSlotControls();
                }
                else
                {
                    Debug.LogWarning("no selected object");
                }
            }

            Debug.Log("SyncCurrentSlotControlInAllPanels End! Elapsed-Time: " +
                      (DateTime.Now - start).TotalMilliseconds +
                      " ms");
        }

        [MenuItem("Tools/Slot/SyncAllSlotControlsInCurrentPanel")]
        public static void SyncAllSlotControlsInCurrentPanel()
        {
            Debug.Log("SyncAllSlotControlsInCurrentPanel Begin");
            var start = DateTime.Now;

            {
                var selectedObj = Selection.activeObject;

                if (selectedObj)
                {
                    Debug.Log("selected object : " + selectedObj.name);
                    var prefab = PrefabUtility.GetPrefabParent(selectedObj);
                    var assetPath = AssetDatabase.GetAssetPath(prefab);
                    Debug.Log("selected object AssetPath : " + assetPath);
                    SetSyncSlotControlPath(null);
                    TraverseOnePanelsAndSyncSlotControls(assetPath);
                }
                else
                {
                    Debug.LogWarning("no selected object");
                }
            }

            Debug.Log("SyncAllSlotControlsInCurrentPanel End! Elapsed-Time: " +
                      (DateTime.Now - start).TotalMilliseconds +
                      " ms");
        }

        [MenuItem("Tools/Slot/DeleteAllSlotControlObjectInAllPanels")]
        public static void DeleteAllSlotControlsObjectInAllPanels()
        {
            Debug.Log("DeleteAllSlotControlObjectInAllPanels Begin");
            var start = DateTime.Now;

            {
                SetSyncSlotControlPath(null);
                TraverseAllPanelsAndDeleteSlotControlObjects();
            }

            Debug.Log("DeleteAllSlotControlObjectInAllPanels End! Elapsed-Time: " +
                      (DateTime.Now - start).TotalMilliseconds + " ms");
        }

        [MenuItem("Tools/Slot/DeleteAllSlotControlObjectsInCurrentPanel")]
        public static void DeleteAllSlotControlObjectsInCurrentPanel()
        {
            Debug.Log("DeleteAllSlotControlObjectsInCurrentPanel Begin");
            var start = DateTime.Now;

            {
                var selectedObj = Selection.activeObject;
                if (selectedObj)
                {
                    Debug.Log("selected object : " + selectedObj.name);

                    var prefab = PrefabUtility.GetPrefabParent(selectedObj);
                    var assetPath = AssetDatabase.GetAssetPath(prefab);
                    Debug.Log("selected object AssetPath : " + assetPath);
                    SetSyncSlotControlPath(null);
                    TraverseOnePanelsAndDeleteSlotControlObjects(assetPath);
                }
                else
                {
                    Debug.LogWarning("no selected object");
                }
            }

            Debug.Log("DeleteAllSlotControlObjectsInCurrentPanel End! Elapsed-Time: " +
                      (DateTime.Now - start).TotalMilliseconds + " ms");
        }

        public static void TraverseAllPanelsAndSyncSlotControls()
        {
            {
                var fileNameList = Directory.GetFiles(SlotControlAssetPath, "*.prefab", SearchOption.AllDirectories);
                foreach (var path in fileNameList)
                {
                    if (Path.GetExtension(path).ToLower() == ".prefab")
                    {
                        TraverseOnePanelsAndSyncSlotControls(path);
                    }
                }
            }

            {
                var fileNameList = Directory.GetFiles(PanelAssetPath, "*.prefab", SearchOption.AllDirectories);
                foreach (var path in fileNameList)
                {
                    if (Path.GetExtension(path).ToLower() == ".prefab" && !path.Contains(SlotControlAssetPath))
                    {
                        TraverseOnePanelsAndSyncSlotControls(path);
                    }
                }
            }
        }

        public static void TraverseOnePanelsAndSyncSlotControls(string assetPath)
        {
            Debug.Log("TraverseOnePanelsAndSyncSlotControls,  assetPath = " + assetPath);

            if (!assetPath.Contains(PanelAssetPath))
            {
                Debug.LogWarning("selected object assetPath is not a panel,  assetPath = " + assetPath);
                return;
            }

            var isSlotControlPanel = assetPath.Contains(SlotControlAssetPath);
            var isSkipWhenInvisible = !isSlotControlPanel;

            var panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (null == panelPrefab)
            {
                Debug.LogWarning("LoadAssetAtPath failed,  assetPath = " + assetPath);
                return;
            }

            var panelObj = UnityEngine.Object.Instantiate(panelPrefab);

            var needReplace = RecursiveTransformAndSyncSlotControls(panelObj.transform, isSkipWhenInvisible);

            if (needReplace)
            {
                Debug.Log("ReplacePrefab Done");
                PrefabUtility.ReplacePrefab(panelObj, panelPrefab,
                    ReplacePrefabOptions.ConnectToPrefab);
            }

            UnityEngine.Object.DestroyImmediate(panelObj);
        }

        public static bool RecursiveTransformAndSyncSlotControls(Transform transform, bool isSkipWhenInvisible)
        {
            var needReplace = false;
            for (var i = 0; i < transform.childCount; i++)
            {
                var childTrans = transform.GetChild(i);
                if (childTrans != null)
                {
                    var isSkipWhenInvisibleInChild = isSkipWhenInvisible;

                    var slotControl = childTrans.GetComponent<SlotControl>();

                    if (SyncSlotControl(slotControl))
                    {
                        needReplace = true;

                        isSkipWhenInvisibleInChild = false;
                    }

                    if (RecursiveTransformAndSyncSlotControls(childTrans, isSkipWhenInvisibleInChild))
                    {
                        needReplace = true;
                    }
                }
            }

            return needReplace;
        }

        public static void TraverseAllPanelsAndDeleteSlotControlObjects()
        {
            var fileNameList = Directory.GetFiles(PanelAssetPath, "*.prefab", SearchOption.AllDirectories);
            foreach (var path in fileNameList)
            {
                if (Path.GetExtension(path).ToLower() == ".prefab")
                {
                    TraverseOnePanelsAndDeleteSlotControlObjects(path);
                }
            }

            AssetDatabase.SaveAssets();
        }

        public static void TraverseOnePanelsAndDeleteSlotControlObjects(string assetPath)
        {
            Debug.Log("TraverseOnePanelsAndDeleteSlotControlObjects,  assetPath = " + assetPath);

            if (!assetPath.Contains(PanelAssetPath))
            {
                Debug.LogWarning("selected object assetPath is not a panel,  assetPath = " + assetPath);
                return;
            }

            var panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (null == panelPrefab)
            {
                Debug.LogWarning("LoadAssetAtPath failed,  assetPath = " + assetPath);
                return;
            }

            RecursiveTransformAndDeleteSlotControlObjects(panelPrefab.transform);
        }

        public static bool RecursiveTransformAndDeleteSlotControlObjects(Transform transform)
        {
            var needReplace = false;
            for (var i = 0; i < transform.childCount; i++)
            {
                var childTrans = transform.GetChild(i);
                if (childTrans != null)
                {
                    var slotControl = childTrans.GetComponent<SlotControl>();

                    if (DestroySlotControlObject(slotControl))
                    {
                        needReplace = true;
                    }

                    if (RecursiveTransformAndDeleteSlotControlObjects(childTrans))
                    {
                        needReplace = true;
                    }
                }
            }

            return needReplace;
        }

        public static void SetSyncSlotControlPath(string assetPath = "")
        {
            SyncSlotControlPath = assetPath;
        }

        public static bool SyncSlotControl(SlotControl slotControl)
        {
            if (slotControl == null)
            {
                return false;
            }


            var needReplace = false;

            if (!string.IsNullOrEmpty(SyncSlotControlPath))
            {
                if (SyncSlotControlPath.Contains(slotControl.prefabPath))
                {
                    needReplace = true;
                }
            }
            else
            {
                needReplace = true;
            }

            DestroySlotControlObject(slotControl);

            Debug.Log("SlotControlTool - LoadControl , name=" + slotControl.gameObject.name);

            slotControl.LoadControl();

            return needReplace;
        }

        public static IEnumerable<Transform> GetChildren(Transform tr)
        {
            var children = new List<Transform>();
            foreach (Transform child in tr)
            {
                children.Add(child);
            }

            return children;
        }

        public static bool DestroySlotControlObject(SlotControl slotControl)
        {
            if (slotControl == null)
            {
                return false;
            }

            var needReplace = false;

            var childs = GetChildren(slotControl.transform);
            foreach (var child in childs)
            {
                var slotControlObject = child.GetComponent<SlotControlObject>();
                if (slotControlObject != null)
                {
                    Debug.Log("DestroySlotControlObject , name=" + slotControlObject.gameObject.name);

                    UnityEngine.Object.DestroyImmediate(slotControlObject.gameObject, true);
                    needReplace = true;
                }
            }

            slotControl.ClearControl();
            return needReplace;
        }
    }
}

#endif