using UnityEngine;

namespace Gankx
{
    public static class GameObjectUtility
    {
        /// <summary>
        /// 设置指定GameObject的Layer，并递归设置所有子GameObject
        /// </summary>
        /// <param name="go">指定的GameObject</param>
        /// <param name="layer">指定的Layer</param>
        public static void SetLayerRecursively(GameObject go, int layer , bool idForce = false)
        {
            if (null == go)
            {
                return;
            }

            if (idForce == false && go.layer == layer)
            {
                return;
            }

            go.layer = layer;

            for (int index = 0; index < go.transform.childCount; ++index)
            {
                Transform childTrans = go.transform.GetChild(index);

                SetLayerRecursively(childTrans.gameObject, layer , idForce);
            }
        }

        /// <summary>
        /// 设置指定GameObject的所有子GameObject的Layer，不包括指定GameObject自身
        /// </summary>
        /// <param name="go">指定的GameObject</param>
        /// <param name="layer">指定的Layer</param>
        public static void SetChildrenLayerRecursively(GameObject go, int layer)
        {
            if (null == go)
            {
                return;
            }

            for (int index = 0; index < go.transform.childCount; ++index)
            {
                Transform childTrans = go.transform.GetChild(index);

                SetLayerRecursively(childTrans.gameObject, layer);
            }
        }

       /// <summary>
       /// 遍历的方法
       /// </summary>
       /// <returns>是否继续遍历</returns>
        public delegate bool TraversalFunc(GameObject go);

        /// <summary>
        /// 深度遍历指定GameObject中所有子GameObject
        /// </summary>
        /// <param name="go">指定的GameObject</param>
        /// <param name="func">指定的遍历方法</param>
        public static void TraversalRecursively(GameObject go, TraversalFunc func)
        {
            if (null == go || null == func)
            {
                return;
            }

            // 遍历方法返回false表达停止遍历
            if (!func(go))
            {
                return;
            }

            for (int index = 0; index < go.transform.childCount; ++index)
            {
                Transform childTrans = go.transform.GetChild(index);

                TraversalRecursively(childTrans.gameObject, func);
            }
        }

        public static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
    }
}
