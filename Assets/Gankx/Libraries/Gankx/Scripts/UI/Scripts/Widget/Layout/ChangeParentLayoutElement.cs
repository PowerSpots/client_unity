using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeParentLayoutElement : MonoBehaviour
{
    public float preferredWidth = -1;
    public float preferredHeight = -1;

    private LayoutElement layoutElement;

    void CacheLayoutElement()
    {
        if (null == layoutElement)
        {
            layoutElement = GetComponentInParent<LayoutElement>();
        }        
    }

    void ApplyLayout()
    {
        if (null == layoutElement)
        {
            return;
        }

        if (preferredWidth > 0)
        {
            layoutElement.preferredWidth = preferredWidth;
        }

        if (preferredHeight > 0)
        {
            layoutElement.preferredHeight = preferredHeight;
        }
    }

    void OnEnable()
    {
        CacheLayoutElement();
        ApplyLayout();
    }

    void OnScaleVisibleChange(bool isScaleVisible)
    {
        if (isScaleVisible)
        {
            CacheLayoutElement();
            ApplyLayout();
        }
    }
}
