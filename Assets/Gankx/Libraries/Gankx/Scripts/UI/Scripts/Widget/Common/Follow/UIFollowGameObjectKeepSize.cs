using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowGameObjectKeepSize : MonoBehaviour {

    public Transform m_target;
    public float sizeOnScreen = 1f;
    public float m_fixSize = 1f;

    public float m_RectHeight;
	
	public void UpdateLoacalScale (Transform target) {
	    m_target = target;

        if (m_target == null || Camera.main == null) return;
	    Camera mainCamera = Camera.main;
        // TODO SceneCameraService
//        if (SceneCameraService.ContainsInstance() && SceneCameraService.Instance.SceneCamera != null &&
//            SceneCameraService.Instance.SceneCamera.enabled) {
//            mainCamera = SceneCameraService.Instance.SceneCamera;
//        }
        Vector3 dir = m_target.position - mainCamera.transform.position;
        float distance = Vector3.Dot(dir, mainCamera.transform.forward);

        transform.localScale = Vector3.one * distance * 0.10f;
        transform.rotation = mainCamera.transform.rotation;

        /*sizeOnScreen = ScreenRTMgr.Instance.GetHeight() * 1.0f / 1080 * m_RectHeight * m_fixSize;
        Vector3 a = mainCamera.WorldToScreenPoint(m_target.position);
	    Vector3 b = new Vector3(a.x, a.y + sizeOnScreen, a.z);

	    Vector3 aa = mainCamera.ScreenToWorldPoint(a);
	    Vector3 bb = mainCamera.ScreenToWorldPoint(b);

	    transform.localScale = Vector3.one * (aa - bb).magnitude;
	    transform.rotation = mainCamera.transform.rotation;*/
    }
}
