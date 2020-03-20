using UnityEngine;

[ExecuteInEditMode]
public class SetImageMaterialScalar : MonoBehaviour {
    public string vecName;
    public float value = 0;
    private Material mat;

    private void OnEnable()
    {
        mat = GetComponent<UnityEngine.UI.Graphic>().material;

    }

    private void Start()
    {
		mat.SetFloat(vecName, value);
    }

    // Update is called once per frame
    void Update ()
    {
        mat.SetFloat(vecName, value);
    }

    public void SetValue(float newValue)
    {
        value = newValue;
        if (mat != null)
            mat.SetFloat(vecName, value);
    }
}
