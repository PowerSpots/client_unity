using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class TextColorTagFix : MonoBehaviour {
    private Text m_Text;
    private string m_LastContent;
    private float m_LastTextAlpha = -1;

    private string COLOR_TAG = "<color=#(.*?)>";

    private Regex m_ColorTagReg;
    private Match m_ColorTagMatch;

    private bool changed = false;

    void Awake () {
	    m_Text = GetComponent<Text>();
        m_ColorTagReg = new Regex(COLOR_TAG);

        if (!m_Text.supportRichText) {
	        enabled = false;
	    }
	}

    void Update() {
        string clearContent = Regex.Replace(m_Text.text, COLOR_TAG, string.Empty);
        if (clearContent != m_LastContent) {
            m_LastContent = clearContent;
            changed = true;
        }

        if (!Mathf.Approximately(m_Text.color.a, m_LastTextAlpha)) {
            m_LastTextAlpha = m_Text.color.a;
            changed = true;
        }

        if (changed) {
            OnTextValueChanged();
            changed = false;
        }
    }
    
    void OnTextValueChanged() {
        string textContent = m_Text.text;
        MatchCollection matchs = Regex.Matches(textContent, COLOR_TAG);
        for (int index = matchs.Count - 1; index >= 0; index--) {
            Match match = matchs[index];
            if (match.Groups.Count > 1) {
                Group group = match.Groups[1];
                string value = group.Value;
                textContent = Replace(textContent, group.Index, group.Length, FormatColor(value));
            }
        }
        m_Text.text = textContent;
    }

    public static string Replace(string s, int index, int length, string replacement) {
        StringBuilder builder = new StringBuilder();
        builder.Append(s.Substring(0, index));
        builder.Append(replacement);
        builder.Append(s.Substring(index + length));
        return builder.ToString();
    }

    string FormatColor(string color) {
        if(string.IsNullOrEmpty(color) || color.Length < 6) return color;
        int colorInt = (int) (m_Text.color.a * 255);
        if (colorInt < 16) {
            colorInt = 16;
        }
        return string.Format("{0}{1:X}", color.Substring(0, 6), colorInt);
    }
}
