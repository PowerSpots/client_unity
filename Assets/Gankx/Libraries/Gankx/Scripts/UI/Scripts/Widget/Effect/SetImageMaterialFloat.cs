using UnityEngine;

[ExecuteInEditMode]
public class SetImageMaterialFloat : MonoBehaviour {
    public string vecName;
    public Vector4 value = Vector4.zero;
    private Material mat = null;

    private void Start()
    {
        if (mat == null)
        {
            mat = GetComponent<UnityEngine.UI.Graphic>().material;
        }

        if (mat != null)
        {
            mat.SetVector(vecName, value);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (mat != null)
        {
            mat.SetVector(vecName, value);
        }
    }
}
