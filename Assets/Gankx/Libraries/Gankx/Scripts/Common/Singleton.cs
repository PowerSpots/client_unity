using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gankx
{
    public static class DontDestroyOnLoadManager
    {
        private static readonly List<GameObject> DdolObjects = new List<GameObject>();

        public static void DontDestroyOnLoad(this GameObject go)
        {
            Object.DontDestroyOnLoad(go);
            DdolObjects.Add(go);
        }

        public static void ClearDontDestroyOnLoad(this GameObject go)
        {
            if (DdolObjects != null)
            {
                DdolObjects.Remove(go);
            }
        }

        public static void DestroyAll()
        {
            for (var i = 0; i < DdolObjects.Count; i++)
            {
                var go = DdolObjects[i];
                if (go != null)
                {
                    Object.Destroy(go);
                }
            }

            DdolObjects.Clear();
        }
    }

    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T MyInstance;
        private static bool IsQuiting;

        public static T instance
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    IsQuiting = false;
                }
#endif

                if (MyInstance == null && !IsQuiting)
                {
#if UNITY_EDITOR
                    MyInstance = FindObjectOfType<T>();
#endif
                    if (MyInstance == null)
                    {
                        MyInstance = new GameObject("Singleton-" + typeof(T).Name).AddComponent<T>();
                        MyInstance.gameObject.tag = "Singleton";
                    }
                }

                return MyInstance;
            }
        }

        public static bool ContainsInstance()
        {
            return MyInstance != null;
        }

        private void Awake()
        {
            var thisInstance = this as T;

            if (MyInstance != null && MyInstance != thisInstance)
            {
                DestroyImmediate(gameObject);
                return;
            }

            gameObject.DontDestroyOnLoad();
            MyInstance = thisInstance;
            OnInit();
        }

        private void OnDestroy()
        {
            gameObject.ClearDontDestroyOnLoad();

            OnRelease();

            //MyInstance = null;
        }

        private void OnApplicationQuit()
        {
            IsQuiting = true;
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnRelease()
        {
        }
    }
}