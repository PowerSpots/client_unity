using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITweenRotateByShader : MonoBehaviour
{
    public float speed = 1f;

    private void Start()
    {
        Image image = GetComponent<Image>();
        if (null == image)
        {
            return;
        }

        image.material.SetFloat("_RotateSpeed", speed);
    }    
}
