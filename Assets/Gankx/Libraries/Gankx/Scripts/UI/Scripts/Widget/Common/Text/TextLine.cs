using UnityEngine;
using UnityEngine.UI;

public class TextLine : MonoBehaviour
{
    private float _oneLineHeight = 0f;
    private Text _text;
    void Awake()
    {
        _text = GetComponent<Text>();
        if (_text == null)
        {
            Debug.LogError("TextLine需要和Text同位置");
            return;
        }
        string cache = _text.text;
        _text.text = "0";
        _text.SetAllDirty();
        _oneLineHeight = _text.preferredHeight;
        _text.text = cache;
    }

    void Update()
    {
        int lineCount = (int)(_text.preferredHeight/ _oneLineHeight);
        if (lineCount == 1)
        {
            _text.alignment = TextAnchor.MiddleCenter;
        }
        else if (lineCount == 2)
        {
            _text.alignment = TextAnchor.MiddleLeft;
        }
    }
}
