using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gankx
{
    public class ObjectPoolService : Singleton<ObjectPoolService>
    {
        private readonly Dictionary<int, ObjectPool> myPoolDict = new Dictionary<int, ObjectPool>();

        private readonly Dictionary<int, ObjectPool> mySpawnPoolDict = new Dictionary<int, ObjectPool>();

        private readonly List<GameObject> myUnspawnList = new List<GameObject>();

        protected override void OnInit()
        {
            StartCoroutine(UnspawnAllRoutine());
        }

        protected override void OnRelease()
        {
            StopAllCoroutines();
        }

        public void AddPool(GameObject prefab, int reserveCount = 1)
        {
            if (null == prefab)
            {
                return;
            }

            var instanceId = prefab.GetInstanceID();
            ObjectPool pool;
            if (myPoolDict.TryGetValue(instanceId, out pool))
            {
                return;
            }

            pool = new ObjectPool(prefab);
            pool.reserveCount = reserveCount;
            pool.Start();

            myPoolDict[instanceId] = pool;
        }

        public void RemovePool(GameObject prefab)
        {
            if (null == prefab)
            {
                return;
            }

            var instanceId = prefab.GetInstanceID();
            ObjectPool pool;
            if (!myPoolDict.TryGetValue(instanceId, out pool))
            {
                return;
            }

            myPoolDict.Remove(instanceId);
            // todo release spawnPoolDict
            var usedObjects = pool.GetUsedObjects();
            for (var i = 0; i < usedObjects.Count; i++)
            {
                if (null == usedObjects[i])
                {
                    continue;
                }

                instanceId = usedObjects[i].GetInstanceID();
                mySpawnPoolDict.Remove(instanceId);
            }

            pool.Stop();
        }

        public int GetUsedCount(GameObject prefab)
        {
            if (null == prefab)
            {
                return -1;
            }

            var instanceId = prefab.GetInstanceID();
            ObjectPool pool;
            if (!myPoolDict.TryGetValue(instanceId, out pool))
            {
                return -1;
            }

            if (pool.GetUsedObjects() == null)
            {
                return -1;
            }

            return pool.GetUsedObjects().Count;
        }

        public void RemoveAll()
        {
            foreach (var pair in myPoolDict)
            {
                pair.Value.Stop();
            }

            myPoolDict.Clear();
            mySpawnPoolDict.Clear();
        }

        public GameObject Spawn(GameObject prefab, Vector3 worldPosition, Quaternion worldRotation)
        {
            if (null == prefab)
            {
                return null;
            }

            var instanceId = prefab.GetInstanceID();
            ObjectPool pool;
            GameObject go;
            if (myPoolDict.TryGetValue(instanceId, out pool))
            {
                go = pool.Spawn(worldPosition, worldRotation);
                if (null != go)
                {
                    mySpawnPoolDict[go.GetInstanceID()] = pool;
                }

                return go;
            }

            go = Instantiate(prefab, worldPosition, worldRotation);
            if (null != go)
            {
                go.SetActive(true);
            }

            return go;
        }

        // Caution:
        // Unspawning go during Update phase will deactive go at the current frame
        // However, unspawning after Update will deactive go at next frame, so maybe will result in some flash bug.
        public void Unspawn(GameObject go)
        {
            myUnspawnList.Add(go);
        }

        public void UnspawnImmediate(GameObject go)
        {
            DoUnspawn(go);
        }

        private IEnumerator UnspawnAllRoutine()
        {
            while (true)
            {
                // Unspawning go during Update phase will deactive go at the current frame
                // However, unspawning after Update will deactive go at next frame, so maybe will result in some flash bug.
                yield return null;
                for (var i = 0; i < myUnspawnList.Count; i++)
                {
                    DoUnspawn(myUnspawnList[i]);
                }

                myUnspawnList.Clear();
            }
        }

        private void DoUnspawn(GameObject go)
        {
            if (null == go)
            {
                return;
            }

            var instanceId = go.GetInstanceID();
            ObjectPool pool;
            if (mySpawnPoolDict.TryGetValue(instanceId, out pool))
            {
                pool.Unspawn(go);
                return;
            }

            Destroy(go);
        }
    }
}