using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Gankx
{
    public static class UITools
    {
        public static string GetHierarchy(GameObject obj)
        {
            if (obj == null)
            {
                return "";
            }

            var path = obj.name;

            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "\\" + path;
            }

            return path;
        }

        public static void SetDirty(Object obj)
        {
#if UNITY_EDITOR
            if (obj)
            {
                EditorUtility.SetDirty(obj);
            }
#endif
        }

        public static GameObject AddChild(GameObject parent, bool undo = true)
        {
            var go = new GameObject();
#if UNITY_EDITOR
            if (undo)
            {
                Undo.RegisterCreatedObjectUndo(go, "Create Object");
            }
#endif
            if (parent != null)
            {
                var rt = go.AddComponent<RectTransform>();
                rt.SetParent(parent.transform, false);
                rt.SetParent(parent.transform);
                rt.localPosition = Vector3.zero;
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                go.layer = parent.layer;
            }

            return go;
        }

        public static GameObject AddChild(GameObject parent, GameObject prefab)
        {
            var go = Object.Instantiate(prefab);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (go != null && parent != null)
            {
                var t = go.transform;
                t.SetParent(parent.transform, false);
                go.layer = parent.layer;
            }

            return go;
        }

        private static void Activate(Transform t, bool compatibilityMode = false)
        {
            SetActiveSelf(t.gameObject, true);

            if (compatibilityMode)
            {
                for (int i = 0, imax = t.childCount; i < imax; ++i)
                {
                    var child = t.GetChild(i);
                    if (child.gameObject.activeSelf)
                    {
                        return;
                    }
                }

                for (int i = 0, imax = t.childCount; i < imax; ++i)
                {
                    var child = t.GetChild(i);
                    Activate(child, true);
                }
            }
        }

        private static void Deactivate(Transform t)
        {
            SetActiveSelf(t.gameObject, false);
        }

        public static void SetActive(GameObject go, bool state, bool compatibilityMode = true)
        {
            if (go)
            {
                if (state)
                {
                    Activate(go.transform, compatibilityMode);
                }
                else
                {
                    Deactivate(go.transform);
                }
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        public static bool GetActive(Behaviour mb)
        {
            return mb && mb.enabled && mb.gameObject.activeInHierarchy;
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        public static bool GetActive(GameObject go)
        {
            return go && go.activeInHierarchy;
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void SetActiveSelf(GameObject go, bool state)
        {
            go.SetActive(state);
        }
    }
}