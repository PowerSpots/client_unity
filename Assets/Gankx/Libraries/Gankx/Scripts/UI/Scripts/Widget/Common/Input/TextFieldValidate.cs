using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TextFieldValidate : MonoBehaviour
{
    private int mLimitLength = 100;

    // Use this for initialization
    void Start()
    {
        InputField inputField = GetComponentInChildren<InputField>();
        if (null != inputField)
        {
            inputField.onValidateInput = ValidateInput;
        }
    }

    public char ValidateInput(string text, int charIndex, char addedChar)
    {
        // Emoji表情
        if (char.GetUnicodeCategory(addedChar) == UnicodeCategory.Surrogate)
        {
            return '\0';
        }

        // 字数限制
        int count = UITextFieldValidateExport.GetStringLength(text);
        if (count >= mLimitLength)
        {
            return '\0';
        }
        return addedChar;
    }

    public void SetLimitLength(int length)
    {
        mLimitLength = length;
    }

    public int GetLimitLength()
    {
        return mLimitLength;
    }
}
