using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public static class UITextExport
{
    public enum OverflowMethod
    {
        Truncate = 0,
        Overflow,
    };

    public static void SetText(uint windowId, string text, bool isRecursive)
    {
        if (isRecursive)
        {
            Text[] uiTexts = PanelService.GetWindowComponentsInChildren<Text>(windowId);
            if (null == uiTexts)
                return;

            for (int i = 0; i < uiTexts.Length; ++i)
            {
                Text t = uiTexts[i];
                if (null == t)
                {
                    return;
                }

                t.text = text;
            }
            return;
        }

        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return;
        }

        uiText.text = text;
    }

    public static string GetText(uint windowId)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return string.Empty;
        }

        return uiText.text;
    }

    public static void SetFontSize(uint windowId, int size)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return;
        }

        uiText.fontSize = size;
    }

    public static int GetFontSize(uint windowId)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return 0;
        }

        return uiText.fontSize;
    }

    public static void SetFontStyle(uint windowId, int style)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return;
        }

        uiText.fontStyle = (FontStyle) style;
    }

    public static int GetFontStyle(uint windowId)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return 0;
        }

        return (int) uiText.fontStyle;
    }

    public static void SetAlignment(uint windowId, int alignment)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return;
        }

        uiText.alignment = (TextAnchor) alignment;
    }

    public static int GetAlignment(uint windowId)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return 0;
        }

        return (int) uiText.alignment;
    }

    public static void SetMultiLine(uint windowId, bool multiLine)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return;
        }

        uiText.horizontalOverflow = multiLine ? HorizontalWrapMode.Overflow : HorizontalWrapMode.Wrap;
    }

    public static bool GetMultiLine(uint windowId)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return false;
        }

        return uiText.horizontalOverflow == HorizontalWrapMode.Overflow;
    }

    public static int GetOverflowMethod(uint windowId)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return 0;
        }

        // uGui to Gankx
        OverflowMethod method = OverflowMethod.Truncate;
        switch (uiText.verticalOverflow)
        {
            case VerticalWrapMode.Truncate:
            {
                method = OverflowMethod.Truncate;
                break;
            }
            case VerticalWrapMode.Overflow:
            {
                method = OverflowMethod.Overflow;
                break;
            }
            default:
            {
                break;
            }
        }

        return (int) method;
    }

    public static void SetOverflowMethod(uint windowId, int overflowMethod)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return;
        }

        // Gankx to uGui
        OverflowMethod method = (OverflowMethod) overflowMethod;
        switch (method)
        {
            case OverflowMethod.Truncate:
            {
                uiText.verticalOverflow = VerticalWrapMode.Truncate;
                break;
            }
            case OverflowMethod.Overflow:
            {
                uiText.verticalOverflow = VerticalWrapMode.Overflow;
                break;
            }
            default:
            {
                break;
            }
        }
    }

    public static void SetLayoutHorizontal(uint windowId)
    {
        var contentSizeFitter = PanelService.GetWindowComponent<ContentSizeFitter>(windowId);
        if (contentSizeFitter == null)
        {
            return;
        }

        contentSizeFitter.SetLayoutHorizontal();
    }

    public static void SetLayoutVertical(uint windowId)
    {
        var contentSizeFitter = PanelService.GetWindowComponent<ContentSizeFitter>(windowId);
        if (contentSizeFitter == null)
        {
            return;
        }

        contentSizeFitter.SetLayoutVertical();
    }

    public static float GetPreferredHeight(uint windowId)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return 0;
        }
        return uiText.preferredHeight;
    }

    public static float GetPreferredWidth(uint windowId)
    {
        Text uiText = PanelService.GetWindowComponent<Text>(windowId);
        if (null == uiText)
        {
            return 0;
        }
        return uiText.preferredWidth;
    }

}