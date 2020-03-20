using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class UITextDynamicSetAlignment : MonoBehaviour {

    public TextAnchor m_DefaultAnchor = TextAnchor.MiddleCenter;
    public TextAnchor m_ChangedAnchor = TextAnchor.MiddleLeft;

    public bool m_Debug = false;

    private Text mText;

    void Awake () {
        mText = GetComponent<Text>();

        mText.RegisterDirtyVerticesCallback(DynamicChangeAlignment);
    }

    void OnEnable() {
        DynamicChangeAlignment();
    }
	
	void OnDestroy () {
	    mText.UnregisterDirtyVerticesCallback(DynamicChangeAlignment);
	}

    void DynamicChangeAlignment() {
        Canvas.ForceUpdateCanvases();
        mText.alignment = mText.cachedTextGenerator.lineCount > 1 ? m_ChangedAnchor : m_DefaultAnchor;
        if (m_Debug)
            Debug.Log("Current Line count is " + mText.cachedTextGenerator.lineCount);
    }
}
