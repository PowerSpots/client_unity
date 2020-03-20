using Gankx.UI;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIAnchor : MonoBehaviour {
    public RectTransform AnchorTarget
    {
        get { return anchorTarget; }
        set
        {
            anchorTarget = value;            
        }
    }
    [SerializeField]
    private RectTransform anchorTarget;
    public RectTransform.Edge leftAnchor = RectTransform.Edge.Left;
    public RectTransform.Edge rightAnchor = RectTransform.Edge.Right;
    public RectTransform.Edge topAnchor = RectTransform.Edge.Top;
    public RectTransform.Edge bottomAnchor = RectTransform.Edge.Bottom;
    
    [SerializeField]
    private float leftDelta = 0f;
    [SerializeField]
    private float rightDelta = 0f;
    [SerializeField]
    private float topDelta = 0f;
    [SerializeField]
    private float bottomDelta = 0f;

    public float LeftDelta
    {
        get { return leftDelta; }
        set { leftDelta = value; }
    }

    public float RightDelta
    {
        get { return rightDelta; }
        set { rightDelta = value; }
    }

    public float TopDelta
    {
        get { return topDelta; }
        set { topDelta = value; }
    }

    public float BottomDelta
    {
        get { return bottomDelta; }
        set { bottomDelta = value; }
    }

    private RectTransform rectTrans;
    private RectTransform parentTrans;

    private Canvas canvas;

    public Canvas GetCanvas()
    {          
        CheckCanvas();
        return canvas;
    }

    void CheckCanvas() {
        if(canvas != null) return;

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null) {
            canvas = parentCanvas.rootCanvas;
        }
    }

    void CheckRefs()
    {
        if (null == rectTrans)
        {
            rectTrans = GetComponent<RectTransform>();
        }

        if (null == parentTrans)
        {
            parentTrans = rectTrans.parent.GetComponentInParent<RectTransform>();
        }        
    }

    void Awake()
    {
        CheckRefs();
        CheckCanvas();       
    }

    void OnScaleVisibleChange(bool visible)
    {
        enabled = visible;
    }

    Vector3[] targetCorners = new Vector3[4];
    Vector3[] parentCorners = new Vector3[4];
    Vector3[] sourceCorners = new Vector3[4];

    public void DoAnchor()
    {
        if (null == anchorTarget)
        {
            return;
        }
        CheckCanvas();
        CheckRefs();

        anchorTarget.GetWorldCorners(targetCorners);
        for (int i = 0; i < targetCorners.Length; i++)
        {
            targetCorners[i] = canvas.transform.InverseTransformPoint(targetCorners[i]);
        }
        float anchoredLeft = targetCorners[0].x + leftDelta;
        if (leftAnchor != RectTransform.Edge.Left)
        {
            anchoredLeft = targetCorners[2].x + leftDelta;
        }

        float anchoredRight = targetCorners[2].x - rightDelta;
        if (rightAnchor != RectTransform.Edge.Right)
        {
            anchoredRight = targetCorners[0].x - rightDelta;
        }

        float anchoredBottom = targetCorners[0].y + bottomDelta;
        if (bottomAnchor != RectTransform.Edge.Bottom)
        {
            anchoredBottom = targetCorners[1].y + bottomDelta;
        }

        float anchoredTop = targetCorners[1].y - topDelta;
        if (topAnchor != RectTransform.Edge.Top)
        {
            anchoredTop = targetCorners[0].y - topDelta;
        }

        parentTrans.GetWorldCorners(parentCorners);
        for (int i = 0; i < parentCorners.Length; i++)
        {
            parentCorners[i] = canvas.transform.InverseTransformPoint(parentCorners[i]);
        }

        float leftRelative = anchoredLeft - parentCorners[0].x;
        float bottomRelative = anchoredBottom - parentCorners[0].y;
        float width = anchoredRight - anchoredLeft;
        float height = anchoredTop - anchoredBottom;

        // todo 不通过父节点来转换，因为anchor实际上是相对于canvas层来说的
        rectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, leftRelative, width);
        rectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, bottomRelative, height);
    }

    public void UpdateAnchorData()
    {
        if (null == anchorTarget)
        {
            return;
        }

        CheckCanvas();
        
        GetComponent<RectTransform>().GetWorldCorners(sourceCorners);
        for (int i = 0; i < sourceCorners.Length; i++)
        {
            sourceCorners[i] = canvas.transform.InverseTransformPoint(sourceCorners[i]);
        }

        anchorTarget.GetWorldCorners(targetCorners);
        for (int i = 0; i < targetCorners.Length; i++)
        {
            targetCorners[i] = canvas.transform.InverseTransformPoint(targetCorners[i]);
        }

        leftDelta = sourceCorners[0].x - targetCorners[0].x;
        if (leftAnchor != RectTransform.Edge.Left)
        {
            leftDelta = sourceCorners[0].x - targetCorners[2].x;
        }

        rightDelta = targetCorners[2].x - sourceCorners[2].x;
        if (rightAnchor != RectTransform.Edge.Right)
        {
            rightDelta = targetCorners[0].x - sourceCorners[2].x;
        }

        bottomDelta = sourceCorners[0].y - targetCorners[0].y;
        if (bottomAnchor != RectTransform.Edge.Bottom)
        {
            bottomDelta = sourceCorners[0].y - targetCorners[1].y;
        }

        topDelta = targetCorners[1].y - sourceCorners[1].y;
        if (topAnchor != RectTransform.Edge.Top)
        {
            topDelta = targetCorners[0].y - sourceCorners[1].y;
        }
    }

	void LateUpdate ()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        DoAnchor();
    }
}
