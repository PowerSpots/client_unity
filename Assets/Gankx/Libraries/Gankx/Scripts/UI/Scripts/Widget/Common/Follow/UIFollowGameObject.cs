using UnityEngine;

/// <summary>
/// 2D UI 跟随3D的物体
/// </summary>
public class UIFollowGameObject : MonoBehaviour
{
    public Transform Target = null;

    public string Path;

    public bool Once;


    private bool _init;

    protected void Update()
    {

        FindTarget();
        UpdateUIPos();
    }

    public void FindTarget()
    {
        if (Target != null)
        {
            return;
        }

        if (string.IsNullOrEmpty(Path))
        {
            return;
        }

        var go = GameObject.Find(Path);
        if (go != null)
        {
            Target = go.transform;
        }
        
    }

    // ReSharper disable once InconsistentNaming
    public void UpdateUIPos()
    {
        if (Target == null)
        {
            return;
        }

        if (_init && Once)
        {
            return;
        }

        var canvas = gameObject.GetComponentInParent<Canvas>();
        var screenCamera = canvas.worldCamera;

        var screenPosition = Camera.main.WorldToScreenPoint(Target.position);

        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, screenCamera, out pos))
        {
            transform.localPosition = pos;
        }

        _init = true;
    }
}
