using Gankx;
using Gankx.UI;
using UnityEngine;

public static class CameraExport
{
    private static Camera cam = null;
    public static void SetMainCameraActive(bool active)
    {
        if (cam == null && Camera.main != null) {
            cam = Camera.main;
        }

        if (cam == null) {
            Debug.LogError("当前 Main Camera 为空！！！");
            return;
        }

        cam.gameObject.SetActive(active);
        SetWorldCanvasByCameraState(active);
    }

    public static void SetWorldCanvasByCameraState(bool active) {
        PanelLayer layer = PanelService.instance.GetLayer(PanelLayerType.World);
        if (layer != null) {
            if (active) {
                layer.Show();
            }
            else {
                layer.Hide();
            }
        }
    }

    private static Camera s_lastMainCamera;
    public static void SetLastMainCameraActiveState(bool active)
    {
        if (s_lastMainCamera == null) {
            s_lastMainCamera = Camera.main;
        }
        if (s_lastMainCamera != null) {
            s_lastMainCamera.gameObject.SetActive(active);
        }
    }
}
