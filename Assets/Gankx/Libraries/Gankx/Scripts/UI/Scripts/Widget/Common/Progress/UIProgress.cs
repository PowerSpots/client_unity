using System;
using UnityEngine;
using UnityEngine.UI;

public class UIProgress : MonoBehaviour
{
    public enum ProgressbarDirection
    {
        Horizontal = 0,
        Vertical = 1,
    }

    public Image Current;

    public Image Target;

    public RectTransform Pivot;

    public RectTransform Node;

    public ProgressbarDirection Direction;

    [SerializeField]
    private float _currentValue;

    [SerializeField]
    private float _targetValue;

    public float CurrentValue
    {
        get
        {
            return _currentValue;
        }

        set
        {
            _currentValue = value;
            UpdateProgressBar();
        }
    }

    public float TargetValue
    {
        get
        {
            return _targetValue;
        }

        set
        {
            _targetValue = value;
            UpdateProgressBar();
        }
    }

    public void SetProgress(float progress1, float progress2)
    {
        CurrentValue = progress1;
        TargetValue = progress2;
    }

    [ContextMenu("UpdateProgressBar")]
    private void UpdateProgressBar()
    {
        var p1 = Mathf.Clamp01(CurrentValue);
        var p2 = Mathf.Clamp01(TargetValue);

        if (Current)
        {
            Current.fillAmount = p1;
        }

        if (Target)
        {
            Target.fillAmount = p2;
        }

        if (!Pivot || !Node) return;
        switch (Direction)
        {
            case ProgressbarDirection.Horizontal:
            {
                
                var local = Node.anchoredPosition;
                local.x = Pivot.rect.width * p1;
                Node.anchoredPosition = local;
            }
                break;
            case ProgressbarDirection.Vertical:
            {
                var local = Node.anchoredPosition;
                local.y = Pivot.rect.height * p1;
                Node.anchoredPosition = local;
            }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


}
