using UnityEngine;
using System.Collections;
using Gankx;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource
    {
        private ObjectPool mPool; 

        // 初始化元素
        public GameObject InitGameObject;

        // 模板元素
        private GameObject mTemplateGameObjectObj ;
        private bool inited = false;
        public virtual GameObject GetObject(Transform parent)
        {
            InitPool();
            GameObject obj = mPool.Spawn(parent);
            return obj;
        }

        public virtual void DelObject(GameObject obj)
        {
            InitPool();
            mPool.Unspawn(obj);
        }

        public void InitPool()
        {
            if (!inited)
            {
                ResetUISoftClipNewObject(InitGameObject);
                GameObject rootObJ = new GameObject("ItemPool");
                rootObJ.transform.SetParent(InitGameObject.transform.parent.parent);
                rootObJ.SetActive(false);
                
                //mTemplateGameObjectObj = GameObject.Instantiate(InitGameObject) as GameObject;
                InitGameObject.name = "Defaultem";
                InitGameObject.transform.SetParent(rootObJ.transform);
                //InitGameObject.SetActive(false);

                mPool = new ObjectPool(InitGameObject);
                mPool.SetParent(rootObJ.transform);
                mPool.Start();

                inited = true;
            }
        }

        protected void ResetUISoftClipNewObject(GameObject newitem)
        {
            if (null == newitem)
            {
                return;
            }

            var icomponents = newitem.GetComponentsInParent<IUISoftNewObjectHandler>(true);
            foreach (var c in icomponents)
            {
                c.OnNewObjectLoaded(newitem);
            }
        }
    }
}
