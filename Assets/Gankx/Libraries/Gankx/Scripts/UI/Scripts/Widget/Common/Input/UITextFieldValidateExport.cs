using Gankx;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public static class UITextFieldValidateExport 
{
    public static void SetLimitLength(uint windowId, int length)
    {
        GameObject windowObject = PanelService.GetWindowObject(windowId);
        if (windowObject == null)
        {
            Debug.LogError("UITextFieldValidateExport.SetLimitLength Error: Window Miss");
            return;
        }

        TextFieldValidate validate = windowObject.GetOrAddComponent<TextFieldValidate>();
        validate.SetLimitLength(length);
    }

    public static int GetLimitLength(uint windowId)
    {
        GameObject windowObject = PanelService.GetWindowObject(windowId);
        if (windowObject == null)
        {
            Debug.LogError("UITextFieldValidateExport.GetLimitLength Error: Window Miss");
            return 0;
        }

        TextFieldValidate validate = windowObject.GetComponent<TextFieldValidate>();
        if (validate != null)
        {
            return validate.GetLimitLength(); 
        }

        return 0;
    }


    public static int GetStringLength(string text)
    {
        // 字数限制
        int count = 0;
        char[] c = text.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if ((int)c[i] > 127)
                count += 2;
            else
                count++;
        }

        return count;
    }
}
