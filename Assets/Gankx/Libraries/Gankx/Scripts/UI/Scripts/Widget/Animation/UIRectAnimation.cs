using UnityEngine;

[ExecuteInEditMode]
public class UIRectAnimation : MonoBehaviour
{
    [SerializeField]
    private RectTransform rt;

    [SerializeField]
    private GameObject go;

    [SerializeField]
    private float speed = 1;

    private float curAmount = 0;

    private Vector2[] corners = new Vector2[4];
    
    void Awake()
    {
        curAmount = 0;
    }

    void OnEnable()
    {
        go.SetActive(false);
        UpdatePos(curAmount);
        go.SetActive(true);
    }

	// Update is called once per frame
	void Update()
	{
        float delta = Time.deltaTime;
        if (delta > 0.05f)
            delta = 0.05f;

        curAmount = (curAmount + speed* delta) % 1;
        UpdatePos(curAmount);
	}

    //amount 0 - 1
    void UpdatePos(float amount)
    {
        if (amount < 0 || amount > 1)
            return;

        if (null == rt || null == go)
            return;

        Rect rect = rt.rect;

        float totalLength = (rect.width + rect.height)*2;

        int index = 0;
        float amount1 = rect.height;
        float amount2 = (rect.height + rect.width);
        float amount3 = (rect.height + rect.width + rect.height);

        float t = 0;
        if (amount * totalLength <= amount1)
        {
            index = 0;
            t = (amount1 - amount * totalLength) / rect.height;
        }
        else if (amount * totalLength <= amount2)
        {
            index = 1;
            t = (amount2 - amount * totalLength) / rect.width;
        }
        else if (amount * totalLength <= amount3)
        {
            index = 2;
            t = (amount3 - amount * totalLength) / rect.height;
        }
        else
        {
            index = 3;
            t = (totalLength - amount * totalLength) / rect.width;
        }

        int start = index;
        int end = (index + 1)%4;

        corners[0] = new Vector2(rect.xMin, rect.yMin);
        corners[1] = new Vector2(rect.xMin, rect.yMax);
        corners[2] = new Vector2(rect.xMax, rect.yMax);
        corners[3] = new Vector2(rect.xMax, rect.yMin);

        Vector2 pos = corners[start]*t + corners[end]*(1 - t);
        go.transform.localPosition = pos;
        //new Vector3(pos.x + rt.transform.position.x, pos.y + rt.transform.position.y, rt.transform.position.z);
    }
}
