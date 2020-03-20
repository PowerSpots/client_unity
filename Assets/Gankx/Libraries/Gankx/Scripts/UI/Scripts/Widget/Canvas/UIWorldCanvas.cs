using UnityEngine;

public class UIWorldCanvas : MonoBehaviour
{
    private Canvas canvas;

    public Material uiMaterial;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();        
    }

    void Update()
    {
        if (null == canvas)
        {
            return;
        }

        if (canvas.renderMode != RenderMode.WorldSpace)
        {
            return;
        }

        canvas.worldCamera = Camera.main;        
    }
}
