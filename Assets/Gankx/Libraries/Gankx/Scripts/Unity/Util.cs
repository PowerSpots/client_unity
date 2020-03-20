using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gankx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public static class Util
{
    public static void CopyTransformR(GameObject dst, GameObject src)
    {
        Debug.Assert(dst.transform.childCount == src.transform.childCount);
        for (int i = 0; i < dst.transform.childCount; i++)
        {
            Transform dstTransform = dst.transform.GetChild(i);
            Transform srcTransform = src.transform.GetChild(i);
            CopyTransform(dstTransform, srcTransform);
            CopyTransformR(dstTransform.gameObject, srcTransform.gameObject);
        }
    }

    //gameobj related
    public static void CopyTransform(Transform dst, Transform src)
    {
        dst.position = src.position;
        dst.rotation = src.rotation;
        dst.localPosition = src.localPosition;
        dst.localRotation = src.localRotation;
        dst.localScale = src.localScale;
    }

    //render related
    public static void InitShader(ref Shader shader, ref Material mat, string shaderPath, string resPath)
    {
        if (shader == null)
        {
            shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                shader = Resources.Load<Shader>(resPath);  //TODO
            }

            mat = new Material(shader);
        }
    }

    public static void InitGameObject(GameObject obj)
    {
        var t = obj.transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    public static Transform CreateEmpty(Transform parent, string name)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent);
        InitGameObject(obj);
        return obj.transform;
    }

    public static T Load<T>(Transform t, string path) where T : Component
    {
        var oobj = ResourceService.Load<GameObject>(path);
        if (null == oobj)
        {
            Debug.LogError("Util Load Error: model(" + path + ") Load Fail");
            return null;
        }
        var obj = Object.Instantiate(oobj, Vector3.zero, Quaternion.identity, t) as GameObject;
        return obj != null ? obj.GetComponent<T>() : null;
    }

    public static GameObject Load(Transform t, string path)
    {
        var oobj = ResourceService.Load<GameObject>(path);
        if (null == oobj)
        {
            Debug.LogError("Util Load Error: model(" + path + ") Load Fail");
            return null;
        }
        var obj = Object.Instantiate(oobj, Vector3.zero, Quaternion.identity, t) as GameObject;
        return obj;
    }

    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindDeepChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }

    public static bool CheckInCameraView(Camera camera, Vector3 p) {
        if (camera == null) return false;
        var po = camera.WorldToViewportPoint(p);
        return po.x >= 0 && po.x <= 1 && po.y >= 0 && po.y <= 1 && po.z >= 0;
    }


    public static bool CheckInCameraFrustum(Camera cam, Bounds bound)
    {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), bound);
    }

    public static bool CheckInCameraViewGO(GameObject go) {
        if (Camera.main == null || go == null) {
            return false;
        }

        var po = Camera.main.WorldToViewportPoint(go.transform.position);
        return po.x >= 0 && po.x <= 1 && po.y >= 0 && po.y <= 1 && po.z >= 0;
    }

    public static float GetGODistance(GameObject go1, GameObject go2) {
        if (go1 == null || go2 == null) return float.MaxValue;

        return Vector3.Distance(go1.transform.position, go2.transform.position);
    }

    public static float GetGODistanceSqr(GameObject go1, GameObject go2)
    {
        if (go1 == null || go2 == null) return float.MaxValue;

        Vector3 delta = go1.transform.position - go2.transform.position;

        return Vector3.Dot(delta, delta);
    }

    public static float GetGODistanceIgnoreY(GameObject go1, GameObject go2) {
        if (go1 == null || go2 == null) return float.MaxValue;

        Vector3 dis = go1.transform.position - go2.transform.position;
        dis.y = 0;
        return dis.magnitude;
    }

    //    //Depth-first search
    //    public static Transform FindDeepChild(this Transform aParent, string aName)
    //    {
    //        foreach(Transform child in aParent)
    //        {
    //            if(child.name == aName )
    //                return child;
    //            var result = child.FindDeepChild(aName);
    //            if (result != null)
    //                return result;
    //        }
    //        return null;
    //    }
    public static bool CheckInCameraView(Vector3 p)
    {
        if (Camera.main == null)
        {
            return false;
        }

        var po = Camera.main.WorldToViewportPoint(p);
        return po.x >= 0 && po.x <= 1 && po.y >= 0 && po.y <= 1 && po.z >= 0;
    }

    // TODO Bloom
//    public static void HideCameraBloom()
//    {
//        var target = Camera.main;
//        if (target == null)
//        {
//            return;
//        }
//
//        var bloom = target.GetComponent<Bloom>();
//        if (bloom == null)
//        {
//            return;
//        }
//
//        bloom.enabled = false;
//    }

    public static string MainCameraTag = "MainCamera";
    public static Camera GetActiveMainCamera()
    {
        if (Camera.main && Camera.main.gameObject.activeInHierarchy)
        {
            return Camera.main;            
        }
        return Camera.allCameras.FirstOrDefault(cam => cam.tag == MainCameraTag && cam.gameObject.activeInHierarchy);
    }


    public static void ResetInputModule()
    {
        if (!EventSystem.current || !EventSystem.current.currentInputModule) return;
        var inputModule = EventSystem.current.currentInputModule;
        inputModule.DeactivateModule();
        inputModule.ActivateModule();
        inputModule.Process();
    }

    public static void WarmUpEffect(string path)
    {
        ResourceService.LoadAsync<GameObject>(path);
    }

    public static string DebugTraceback()
    {
        string info = null;
        StackTrace st = new StackTrace(true);
        StackFrame[] sf = st.GetFrames();
        for (int i = 0; i < sf.Length; ++i)
        {
            info = info + "\r\n" + " FileName=" + sf[i].GetFileName() + " fullname=" + sf[i].GetMethod().DeclaringType.FullName + " function=" + sf[i].GetMethod().Name + " FileLineNumber=" + sf[i].GetFileLineNumber();
        }
        return info;
    }

    public static void ResetRectTransform(RectTransform t)
    {
        if (null == t)
        {
            return;
        }
        t.anchoredPosition = Vector2.zero;
        t.sizeDelta = Vector2.zero;
    }

    public static void ResetRectTransform(Transform t)
    {
        ResetRectTransform(t.GetComponent<RectTransform>());
    }

    public static void ResetRectTransform(GameObject t)
    {
        ResetRectTransform(t.GetComponent<RectTransform>());
    }

    public static void SetObjectEnable(Object o, bool state)
    {
        var behaiour = o as Behaviour;
        if (behaiour != null)
        {
            behaiour.enabled = state;
        }
    }

    public static void ChangeAndPlayAnimationClip(Animator animator, AnimationClip animationClip, string clipName, string stateName)
    {
        if (animator == null || animationClip == null || string.IsNullOrEmpty(clipName) ||string.IsNullOrEmpty(stateName))
        {
            Debug.LogError("ChangeAndPlayAnimationClip input param is invalid");
            return;
        }

        AnimatorOverrideController aoc = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (aoc == null)
        {
            Debug.LogError("ChangeAndPlayAnimationClip Error: AnimatorOverrideController component is miss");
            return;
        }

        aoc[clipName] = animationClip;
        animator.Update(0);
        animator.speed = 1.0f;
        animator.Play(stateName);
    }
    public static int GetCanvasRenderOrder(this GraphicRaycaster graphicRaycaster)
    {
        return graphicRaycaster.GetComponent<Canvas>().renderOrder;
    }

    public static void FindAllNonAlloc<T>(this List<T> aList, Predicate<T> match,ref List<T> objList)
    {
        objList.Clear();
        foreach (var t in aList)
        {
            if (!match(t)) continue;

            objList.Add(t);
        }
    }
}

public class TimeUtils {
    private static readonly DateTime Jan1st1970 = new DateTime
        (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long CurrentTimeMillis()
    {
        return (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }
}