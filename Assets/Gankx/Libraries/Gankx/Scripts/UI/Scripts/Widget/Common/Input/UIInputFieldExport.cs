using System.Collections.Generic;
using System.Globalization;
using Gankx.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIInputFieldExport
{
    public static void SetInteractable(uint windowId, bool isInteractable)
    {
        InputField inputField = PanelService.GetWindowComponent<InputField>(windowId);
        if (null == inputField)
        {
            return;
        }

        inputField.interactable = isInteractable;
    }

    public static bool GetInteractable(uint windowId)
    {
        InputField inputField = PanelService.GetWindowComponent<InputField>(windowId);
        if (null == inputField)
        {
            return false;
        }

        return inputField.interactable;
    }

    public static string GetText(uint windowId)
    {
        InputField inputField = PanelService.GetWindowComponent<InputField>(windowId);
        if (null == inputField)
        {
            return "";
        }
        return inputField.text;
    }

    public static void SetText(uint windowId, string str)
    {
        InputField inputField = PanelService.GetWindowComponent<InputField>(windowId);
        if (null == inputField)
        {
            return;
        }
        inputField.text = str;
    }
    public static void SetSafeText(uint windowId, string str)
    {
        var safeStr = new List<char>();
        foreach (var c in str)
        {
            if (char.GetUnicodeCategory(c) == UnicodeCategory.Surrogate)
            {
                continue;
            }
            
            safeStr.Add(c);
        }
        SetText(windowId, new string(safeStr.ToArray()));
    }

    public static void SetCharacterLimit(uint windowId, int num)
    {
        InputField inputField = PanelService.GetWindowComponent<InputField>(windowId);
        if (null == inputField)
        {
            return;
        }

        inputField.characterLimit = num;
    }

    public static void SetCaretPos(uint windowId, int pos)
    {
        InputField inputField = PanelService.GetWindowComponent<InputField>(windowId);
        if (null == inputField)
        {
            return;
        }

        InputFieldCaretController ctrl = inputField.GetOrAddComponent<InputFieldCaretController>();
        ctrl.SetCaretPos(pos);
    }

    public static void MoveCaretToEnd(uint windowId)
    {
        InputField inputField = PanelService.GetWindowComponent<InputField>(windowId);
        if (null == inputField)
        {
            return;
        }

        InputFieldCaretController ctrl = inputField.GetOrAddComponent<InputFieldCaretController>();
        ctrl.MoveCaretToEnd();
    }

    public static void ForceOn(uint windowId)
    {
        InputField inputField = PanelService.GetWindowComponent<InputField>(windowId);
        if (null == inputField)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }

    public static void SetPlaceHolderText(uint windowId,string text)
    {
        InputField inputField = PanelService.GetWindowComponent<InputField>(windowId);
        if (null == inputField)
        {
            return;
        }
        inputField.placeholder.GetComponent<Text>().text = text;
    }

    public static int GetUnicodeCategory(string text)
    {
        var c = text[0];
        return (int)char.GetUnicodeCategory(c);
    }
    
}