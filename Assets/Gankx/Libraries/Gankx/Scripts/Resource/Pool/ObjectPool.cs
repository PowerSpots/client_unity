using System.Collections.Generic;
using UnityEngine;

namespace Gankx
{
    public class ObjectPool
    {
        public int reserveCount = 1;

        private List<GameObject> myCachedObjects = new List<GameObject>();
        private List<GameObject> myUsedObjects = new List<GameObject>();

        private Transform myParentRoot;

        public ObjectPool(GameObject prefab)
        {
            started = false;
            template = prefab;
        }

        public bool started { get; private set; }

        public GameObject template { get; private set; }

        public void Reserve(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var newObj = Object.Instantiate(template, myParentRoot);
                if (myParentRoot == null)
                {
                    myParentRoot = ObjectPoolService.instance.transform;
                }

                newObj.SetActive(false);

                myCachedObjects.Add(newObj);

                var poolables = newObj.GetComponentsInChildren<IPoolable>(true);
                for (var j = 0; j < poolables.Length; j++)
                {
                    poolables[j].OnInstantiate();
                }
            }
        }

        public void SetParent(Transform rootTransform)
        {
            if (rootTransform == null)
            {
                myParentRoot = ObjectPoolService.instance.transform;
            }
            else
            {
                myParentRoot = rootTransform;
            }
        }

        public void Start()
        {
            if (started)
            {
                return;
            }

            started = true;

            Reserve(reserveCount);
        }

        public void Stop()
        {
            for (var i = 0; i < myCachedObjects.Count; i++)
            {
                if (myCachedObjects[i] != null)
                {
                    Object.Destroy(myCachedObjects[i]);
                }
            }

            myCachedObjects.Clear();
            myUsedObjects.Clear();
            template = null;
        }

        private bool IsNull(GameObject go)
        {
            return go == null;
        }

        private void UpdateCachedObjects()
        {
            var count = myCachedObjects.Count;
            var hasEmpty = false;
            for (var i = count - 1; i >= 0; i--)
            {
                if (myCachedObjects[i] == null)
                {
                    hasEmpty = true;
                    break;
                }
            }

            if (hasEmpty)
            {
                myCachedObjects.RemoveAll(IsNull);
            }
        }

        public GameObject Spawn(Transform newParent = null)
        {
            UpdateCachedObjects();

            if (myCachedObjects.Count <= 0)
            {
                Reserve(reserveCount);
            }

            var go = myCachedObjects[myCachedObjects.Count - 1];
            myCachedObjects.RemoveAt(myCachedObjects.Count - 1);
            myUsedObjects.Add(go);

            go.transform.SetParent(newParent);

            var poolables = go.GetComponentsInChildren<IPoolable>(true);
            for (var i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnSpawn();
            }

            //TODO 存在的坑：如果在脚本Enable中逻辑依赖于父节点的话，会存在逻辑错误
            go.SetActive(true);

            return go;
        }

        public GameObject Spawn(Vector3 worldPosition, Quaternion worldRotation, Transform newParent = null)
        {
            UpdateCachedObjects();

            if (myCachedObjects.Count <= 0)
            {
                Reserve(reserveCount);
            }

            var go = myCachedObjects[myCachedObjects.Count - 1];
            myCachedObjects.RemoveAt(myCachedObjects.Count - 1);
            myUsedObjects.Add(go);

            go.transform.SetParent(newParent);
            go.transform.position = worldPosition;
            go.transform.rotation = worldRotation;

            var poolables = go.GetComponentsInChildren<IPoolable>(true);
            for (var i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnSpawn();
            }

            //TODO 存在的坑：如果在脚本Enable中逻辑依赖于父节点的话，会存在逻辑错误
            go.SetActive(true);

            return go;
        }

        public void Unspawn(GameObject go)
        {
            go.SetActive(false);

            var poolables = go.GetComponentsInChildren<IPoolable>(true);
            for (var i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnUnspawn();
            }

            myCachedObjects.Add(go);
            myUsedObjects.Remove(go);

            go.transform.SetParent(myParentRoot);
        }

        public GameObject SpawnkeepActive(Transform newParent = null)
        {
            UpdateCachedObjects();

            if (myCachedObjects.Count <= 0)
            {
                Reserve(reserveCount);
            }

            var go = myCachedObjects[myCachedObjects.Count - 1];
            myCachedObjects.RemoveAt(myCachedObjects.Count - 1);
            myUsedObjects.Add(go);

            go.transform.SetParent(newParent);

            var poolables = go.GetComponentsInChildren<IPoolable>(true);
            for (var i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnSpawn();
            }

            return go;
        }

        public void UnspawnkeepActive(GameObject go)
        {
            var poolables = go.GetComponentsInChildren<IPoolable>(true);
            for (var i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnUnspawn();
            }

            myCachedObjects.Add(go);
            myUsedObjects.Remove(go);

            go.transform.SetParent(myParentRoot);
        }

        public void UnspawnAll()
        {
            if (myUsedObjects.Count == 0)
            {
                return;
            }

            UpdateCachedObjects();
            var list = new List<GameObject>(myUsedObjects);
            for (var i = 0; i < list.Count; i++)
            {
                Unspawn(list[i]);
            }
        }

        public List<GameObject> GetUsedObjects()
        {
            return myUsedObjects;
        }

        public List<GameObject> GetCacheObjects()
        {
            return myCachedObjects;
        }

        public void ReleaseUnusedCacheCount(int count)
        {
            var curCount = myCachedObjects.Count;
            if (curCount > count)
            {
                for (var i = curCount; i < curCount; i++)
                {
                    if (myCachedObjects[i] != null)
                    {
                        Object.Destroy(myCachedObjects[i]);
                    }
                }

                myCachedObjects.RemoveRange(count, curCount - count);
            }
        }
    }
}