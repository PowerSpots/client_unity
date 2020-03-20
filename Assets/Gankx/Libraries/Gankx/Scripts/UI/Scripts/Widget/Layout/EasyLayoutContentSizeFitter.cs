using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EasyLayoutContentSizeFitter : MonoBehaviour
{
    private ContentSizeFitter contentSizeFitter;
    private EasyLayout.EasyLayout easyLayout;
    private void Awake()
    {
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        if (null == contentSizeFitter)
        {
            Debug.LogError("Cannot Find Content Size Fitter", gameObject);
        }
        easyLayout = GetComponent<EasyLayout.EasyLayout>();
        if (null == easyLayout)
        {
            Debug.LogError("Cannot Find EasyLayout", gameObject);
        }
    }

    public void UpdateAll()
    {
        if (null == contentSizeFitter || null == easyLayout)
        {
            return;
        }

        easyLayout.UpdateLayout();
        contentSizeFitter.SetLayoutHorizontal();
        contentSizeFitter.SetLayoutVertical();
    }

    private void LateUpdate()
    {
        UpdateAll();
    }   
}
