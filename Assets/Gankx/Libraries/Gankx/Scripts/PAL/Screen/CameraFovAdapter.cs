using UnityEngine;

public class CameraFovAdapter : MonoBehaviour {
    private  float fovFactor = 1.0f;
    private Camera mycamera;
    private bool needAdapt = false;
    const float standardAspect = 16f / 9f;
    public bool onlyAdaptNarrowScreen = false;

    private bool started = false;
    // Use this for initialization
    public void Start () {
        if (started)
        {
            return;
        }

        started = true;

        mycamera = GetComponent<Camera>();
        float aspect = (float)Screen.width / (float)Screen.height;
        if (!onlyAdaptNarrowScreen && Mathf.Abs(aspect - standardAspect) > 0.001f)
        {
            needAdapt = true;
        }
        else if (onlyAdaptNarrowScreen && aspect < (standardAspect - 0.001f))
        {
            needAdapt = true;
        }

        if (needAdapt)
        {            
            fovFactor = standardAspect / aspect;
            if (mycamera.orthographic)
            {
                mycamera.orthographicSize = mycamera.orthographicSize*fovFactor;
            }
            else
            {
                float fov = AdaptFov(mycamera.fieldOfView);
                mycamera.fieldOfView = fov;
            }
        }
	}

    public float AdaptFov(float fov)
    {
        if (!needAdapt)
            return fov;
        float tan = Mathf.Tan((fov / 2f)*Mathf.Deg2Rad) * fovFactor;
        return Mathf.Atan(tan)*2f*Mathf.Rad2Deg;
    }

    public float InverseAdaptFov(float fov)
    {
        if (!needAdapt)
            return fov;
        float tan = Mathf.Tan((fov / 2f) * Mathf.Deg2Rad) / fovFactor;
        return Mathf.Atan(tan) * 2f * Mathf.Rad2Deg;
    }

    /*void OnPreRender()
    {
        origFov = mycamera.fieldOfView;
        AdaptFov();
    }

    void OnPostRender()
    {
        mycamera.fieldOfView = origFov;
    }*/
}
