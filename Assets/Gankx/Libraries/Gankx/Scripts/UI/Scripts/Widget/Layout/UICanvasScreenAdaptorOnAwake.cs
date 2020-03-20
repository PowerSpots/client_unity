using UnityEngine;

class UICanvasScreenAdaptorOnAwake:MonoBehaviour
{
    void Awake()
    {
        UIScreenAdaptor.SetCanvasScaler(gameObject);
    }
}
