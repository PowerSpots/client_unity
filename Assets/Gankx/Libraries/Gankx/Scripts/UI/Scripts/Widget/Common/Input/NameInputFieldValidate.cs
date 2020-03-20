using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class NameInputFieldValidate : MonoBehaviour
{
    public int MAX_STRING_LENGTH = 12;

    Font font;
	// Use this for initialization
	void Start ()
	{
	    InputField inputField = GetComponentInChildren<InputField>();
	    if (null != inputField)
        {
            inputField.onValidateInput = ValidateInput;
            font = inputField.textComponent.font;
        }
	}

    public char ValidateInput(string text, int charIndex, char addedChar)
    {
        // Emoji表情
        if (char.GetUnicodeCategory(addedChar) == UnicodeCategory.Surrogate)
        {
            return '\0';
        }

        if (char.GetUnicodeCategory(addedChar) == UnicodeCategory.OtherSymbol)
        {
            return '\0';
        }
        
        if(!font.HasCharacter(addedChar))
        {
            return '\0';
        }
        
        string fullText = text + addedChar;
        // 字数限制
        int count = UITextFieldValidateExport.GetStringLength(fullText);
        if (count > MAX_STRING_LENGTH)
        {
            return '\0';
        }
        return addedChar;
    }
}
