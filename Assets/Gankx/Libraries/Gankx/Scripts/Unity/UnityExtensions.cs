using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XLua;
using Object = UnityEngine.Object;

/// <summary>
/// Unity Extensions 扩展功能
/// </summary>
public static class UnityExtensions
{

    public static void SetGlobalScale(this Transform transform, Vector3 globalScale) {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }

    /// <summary>
    /// 根据名字在子对象中查找组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindComponent<T>(this Transform trans, string name, bool reportError = true) where T : Component
    {
        Transform target = trans.Find(name);
        if (target == null)
        {
            if (reportError)
            {
                Debug.LogError("Transform is null, name = " + name);
            }

            return null;
        }

        T component = target.GetComponent<T>();
        if (component == null)
        {
            if (reportError)
            {
                Debug.LogError("Component is null, type = " + typeof(T).Name);
            }

            return null;
        }

        return component;
    }

    /// <summary>
    /// Gets or add a component. Usage example:
    /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
    /// </summary>
    public static T GetOrAddComponent<T>(this Component child) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }

    public static T GetOrAddComponent<T>(this GameObject child) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.AddComponent<T>();
        }
        return result;
    }

    /// <summary>
    /// 可以递归地查找所有子节点的某个T类型的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <param name="recursive"></param>
    /// <param name="includeInactive"></param>
    /// <param name="reportError"></param>
    /// <returns></returns>
    public static T[] FindComponentsInChildren<T>(this Transform transform, bool recursive = true, bool includeInactive = true, bool reportError = true) where T : Component
    {

        if (recursive)
        {
            var list = new System.Collections.Generic.List<T>();
            if (reportError && list == null)
            {
                Debug.LogError("Create List error!");
            }
            GetChildren(transform, includeInactive, ref list);
            return list.ToArray();
        }
        else
        {
            return transform.GetComponentsInChildren<T>(includeInactive);
        }
    }

    public static T GetComponentsInParent<T>(this Transform transform) where T : Component
    {
        if (transform == null) {
            return null;
        }
        if (transform.GetComponent<T>() != null) {
            return transform.GetComponent<T>();
        }
        if (transform.parent != null) {
            return transform.parent.GetComponentInParent<T>();
        }
        return null;
    }

    public static T GetComponentInParentIgnoreState<T>(this Transform transform) where T : Component {
        if (transform == null) {
            return null;
        }

        T current = null;
        current = transform.GetComponent<T>();
        if (current != null) {
            return current;
        }
        if (transform.parent != null) {
            return transform.parent.GetComponentInParentIgnoreState<T>();
        }
        return null;
    }

    private static void GetChildren<T>(Transform t, bool includeInactive, ref System.Collections.Generic.List<T> list)
    {
        if (includeInactive || t.gameObject.activeSelf)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                if (t.GetChild(i) != null)
                {
                    GetChildren(t.GetChild(i), includeInactive, ref list);
                }
            }

            var comp = t.GetComponent<T>();
            if (comp != null && comp.ToString() != "null")
            {
                list.Add(comp);
            }
        }

    }

    public static Transform GetChildByName(this Transform transform,string name, bool recursive = true, bool includeInactive = true)
    {
        Transform target;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null && (includeInactive || transform.gameObject.activeSelf))
            {
                if (child.name == name)
                {
                    return child;
                }
                else
                {
                    target = child.GetChildByName(name);
                    if (target != null) return target;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 修改layer  和 child layer
    /// </summary>
    /// <param name="trans">父</param>
    /// <param name="name">layer name</param>
    public static void ChangeLayersRecursively(this Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        for (int i = 0; i < trans.childCount; i++)
        {
            ChangeLayersRecursively(trans.GetChild(i), name);
        }
    }

    public static void ChangeLayersRecursively(this Transform trans, int layer)
    {
        trans.gameObject.layer = layer;
        for (var i = 0; i < trans.childCount; i++)
        {
            ChangeLayersRecursively(trans.GetChild(i), layer);
        }
    }

    /// <summary>
    ///  修改layer  和 child layer
    /// </summary>
    /// <param name="o">父</param>
    /// <param name="layer">layer</param>
    public static void SetLayerRecursively(this GameObject o, int layer)
    {
        ChangeLayersRecursively(o.transform, layer);
    }


    /// <summary>
    /// 递归修改GameObject Active
    /// </summary>
    /// <param name="go"></param>
    /// <param name="state"></param>
    public static void SetActivateForChildren(this GameObject go, bool state)
    {
        ActivateChildren(go, state);
    }

    public static void ActivateChildren(GameObject go, bool state)
    {
        go.SetActive(state);

        foreach (Transform child in go.transform)
        {
            ActivateChildren(child.gameObject, state);
        }
    }

//     /// <summary>
//     /// 记录当前的trans信息，包括父节点
//     /// </summary>
//     /// <param name="trans"></param>
//     public static int RecordTrans(this Transform trans)
//     {
//         RecordableTrans recordableTrans = trans.GetOrAddComponent<RecordableTrans>();
//         return recordableTrans.RecordTrans();
//     }
// 
//     /// <summary>
//     /// 恢复记录的状态
//     /// </summary>
//     /// <param name="trans"></param>
//     public static void RecoverTrans(this Transform trans,int index = 1)
//     {
//         RecordableTrans recordableTrans = trans.GetComponent<RecordableTrans>();
//         if (null != recordableTrans)
//             recordableTrans.RecoverTrans(index);
//     }

    /// <summary>
    /// 16进制转Color
    /// </summary>
    /// <param name="HexVal"></param>
    /// <returns></returns>
    public static Color32 HexToColor32(this int HexVal)
    {
        if (HexVal > 0xFFFFFF || HexVal < 0x000000) return Color.white;

        byte R = (byte)((HexVal >> 16) & 0xFF);
        byte G = (byte)((HexVal >> 8) & 0xFF);
        byte B = (byte)((HexVal) & 0xFF);
        return new Color32(R, G, B, 255);
    }

    /// <summary>
    /// 截取字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="length">长度:汉字算2个</param>
    /// <returns></returns>
    public static string CutString(this string str, int length)
    {
        string result = "";
        int count = 0;
        for (int i = 0; i < str.Length; i++)
        {
            char ch = str[i];
            count += System.Text.Encoding.Default.GetByteCount(ch.ToString());
            if (count <= length)
            {
                result += ch;
            }
            else
                break;
        }
        return result;
    }

    /// <summary>
    /// 返回字符串的字符长度，汉字算2个，英文算1个.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetStringCount(this string str)
    {
        return System.Text.Encoding.Default.GetByteCount(str);
    }

    /// <summary> 
    /// 截取文本，区分中英文字符，中文算两个长度，英文算一个长度
    /// </summary>
    /// <param name="str">待截取的字符串</param>
    /// <param name="length">需计算长度的字符串</param>
    /// <returns>string</returns>
    public static string GetSubString(this string str, int length)
    {
        string temp = str;
        int j = 0;
        int k = 0;
        for (int i = 0; i < temp.Length; i++)
        {
            if (Regex.IsMatch(temp.Substring(i, 1), @"[\u4e00-\u9fa5]+"))
            {
                j += 2;
            }
            else
            {
                j += 1;
            }
            if (j <= length)
            {
                k += 1;
            }
            if (j > length)
            {
                return temp.Substring(0, k);
            }
        }
        return temp;
    }

    public static string ShowCntStr(this int cnt)
    {
        if (cnt > 99999)
        {
            int cnt0 = cnt / 10000;
            return cnt0.ToString()+"万";
        }
        return cnt.ToString();
    }

    /// <summary>
    /// 添加按钮事件
    /// </summary>
    /// <param name="button"></param>
    /// <param name="listener"></param>
    public static void ButtonClickAddListener(this Button button, UnityAction listener)
    {
        if (button == null)
        {
            Debug.LogError("Button is null");
            return;
        }

        button.onClick.AddListener(listener);
    }

    /// <summary>
    /// 移除按钮事件
    /// </summary>
    /// <param name="button"></param>
    /// <param name="listener"></param>
    public static void ButtonClickRemoveListener(this Button button, UnityAction listener)
    {
        if (button == null)
        {
            Debug.LogError("Button is null");
            return;
        }

        button.onClick.RemoveListener(listener);
    }

    #region 时间转换

    public static DateTime GetDateTime(this int timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        TimeSpan toNow = new TimeSpan((long)timeStamp * 10000000);
        return dtStart.Add(toNow);
    }

    public static DateTime GetDateTime(this long uiTimestamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        TimeSpan toNow = new TimeSpan(uiTimestamp * 10000000);
        return dtStart.Add(toNow);
    }
    #endregion

    public static void ShowRender(this Transform trans,bool isShow)
    {
        Renderer[] renders = trans.FindComponentsInChildren<Renderer>();
        foreach (var render in renders)
        {
            if(render != null)
                render.enabled = isShow;
        }
    }
	
	public static bool CustomEndsWith(this string a, string b) {
        if (a == null || b == null) return false;
        int ap = a.Length - 1;
        int bp = b.Length - 1;

        while (ap >= 0 && bp >= 0 && a[ap] == b[bp]) {
            ap--;
            bp--;
        }
	    return (bp < 0 && a.Length >= b.Length) ||
	           (ap < 0 && b.Length >= a.Length);
	}

    public static bool CustomStartsWith(this string a, string b) {
        if (a == null || b == null) return false;
        int aLen = a.Length;
        int bLen = b.Length;
        int ap = 0; int bp = 0;

        while (ap < aLen && bp < bLen && a[ap] == b[bp]) {
            ap++;
            bp++;
        }
        
        return (bp == bLen && aLen >= bLen) ||
               (ap == aLen && bLen >= aLen);
    }

    public static void SafeClearCameraTarget(this Camera camera) {
        if(camera == null) return;
        CameraResolutionAdapter adapter = camera.GetComponent<CameraResolutionAdapter>();
        if (adapter != null) {
            adapter.Refresh();
        }
        else
            camera.targetTexture = null;
    }

    public static Transform Clear(this Transform transform)
    {
        foreach (Transform child in transform) {
            Object.Destroy(child.gameObject);
        }
        return transform;
    }
}

[ReflectionUse]
public static class UnityEngineObjectExtention
{
    public static bool IsNull(this UnityEngine.Object o) // 或者名字叫IsDestroyed等等
    {
        return o == null;
    }
}