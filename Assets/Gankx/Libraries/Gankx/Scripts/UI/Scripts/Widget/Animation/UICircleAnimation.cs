using UnityEngine;

[ExecuteInEditMode]
public class UICircleAnimation : MonoBehaviour
{
    [SerializeField]
    private RectTransform rt;

    [SerializeField]
    private GameObject go;

    [SerializeField]
    private float speed = 1;

    private float curAmount = 0;
    
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
    void Update ()
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

        float radius = Mathf.Min(rect.width, rect.height) / 2;
        

        go.transform.localPosition = new Vector2(radius * Mathf.Cos(amount * 2 * Mathf.PI), radius * Mathf.Sin(amount * 2 * Mathf.PI));
        //new Vector3(pos.x + rt.transform.position.x, pos.y + rt.transform.position.y, rt.transform.position.z);
    }
}
