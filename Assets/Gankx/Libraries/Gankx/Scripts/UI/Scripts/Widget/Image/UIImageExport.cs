using Gankx;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public static class UIImageExport
{
    public static void SetAtlasPath(uint windowId, string path)
    {
        Image uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return;
        }

        DynamicImage di = uiImage.GetComponent<DynamicImage>();
        if (null == di)
        {
            di = uiImage.gameObject.AddComponent<DynamicImage>();
        }

        di.SetAtlasPath(path);
    }

    public static string GetAtlasPath(uint windowId)
    {
        Image uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return string.Empty;
        }
        
        DynamicImage di = uiImage.GetComponent<DynamicImage>();
        if (null == di)
        {
            return "";
        }
        return di.GetPath();
    }

    public static void SetPath(uint windowId, string path)
    {
        Image uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return;
        }
        
        DynamicImage di = uiImage.GetComponent<DynamicImage>();
        if (null == di)
        {
            di = uiImage.gameObject.AddComponent<DynamicImage>();
        }

        di.SetPath(path);
    }

    public static void SetPathAsync(uint windowId, string path)
    {
        Image uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return;
        }

        DynamicImage di = uiImage.GetComponent<DynamicImage>();
        if (null == di)
        {
            di = uiImage.gameObject.AddComponent<DynamicImage>();
        }

        di.SetPathAsync(path);
    }

    public static void CopyFrom(uint destId, uint srcId)
    {
        Image srcImage = PanelService.GetWindowComponent<Image>(srcId);
        if (null == srcImage)
        {
            return;
        }

        Image destImage = PanelService.GetWindowComponent<Image>(destId);
        if (null == destImage)
        {
            return;
        }

        destImage.sprite = srcImage.sprite;
    }

    public static void SetFillAmount(uint windowId, float fillAmount)
    {
        Image uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.fillAmount = fillAmount;
    }

    public static float GetFillAmount(uint windowId)
    {
        Image uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return 1;
        }

        return uiImage.fillAmount;
    }

    #region CircleImage
    public static void SetCircleImageBeginFillAmount(uint windowId, float fillAmount)
    {
        CircleImage uiImage = PanelService.GetWindowComponent<CircleImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.beginFillPercent = fillAmount;
    }

    public static void SetCircleImageEndFillAmount(uint windowId, float fillAmount)
    {
        CircleImage uiImage = PanelService.GetWindowComponent<CircleImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.endFillPercent = fillAmount;
    }

    public static void SetCircleImageSegements(uint windowId, int segements)
    {
        CircleImage uiImage = PanelService.GetWindowComponent<CircleImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.segements = segements;
    }

    public static void SetCircleImageStartAngle(uint windowId, float startAngle)
    {
        CircleImage uiImage = PanelService.GetWindowComponent<CircleImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.startAngle = startAngle;
    }

    public static void SetCircleImageUVOffset(uint windowId, float uvOffsetX, float uvOffsetY)
    {
        CircleImage uiImage = PanelService.GetWindowComponent<CircleImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.SetUVOffset(new Vector2(uvOffsetX, uvOffsetY));
    }

    public static void SetCircleImageVerticesDirty(uint windowId)
    {
        CircleImage uiImage = PanelService.GetWindowComponent<CircleImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.SetVerticesDirty();
    }
    #endregion

    public static void SetNativeSize(uint windowId)
    {
        AtlasImage uiImage = PanelService.GetWindowComponent<AtlasImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.SetNativeSize();
    }

    public static void SetGray(uint windowId, bool state)
    {
        Image uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return;
        }

        var grayEffect = uiImage.GetOrAddComponent<UIImageGrayEffect>();
        grayEffect.Gray = state;
    }


    public static void LoadLocalImage(uint windowId, string path)
    {
        RawImage uiImage = PanelService.GetWindowComponent<RawImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        RawImageLocalLoader jpgloader = uiImage.GetComponent<RawImageLocalLoader>();
        if (null == jpgloader)
        {
            jpgloader = uiImage.gameObject.AddComponent<RawImageLocalLoader>();
        }

        jpgloader.LoadJpg(path);
    }

    public static void ClearLocalImage(uint windowId)
    {
        RawImage uiImage = PanelService.GetWindowComponent<RawImage>(windowId);
        if (null == uiImage)
        {
            return;
        }
        uiImage.texture = null;
    }

    public static void SetEnabled(uint windowId, bool enabled)
    {
        Image uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.enabled = enabled;
    }

    public static float GetImageWidth(uint windowId)
    {
        var uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return 0f;
        }
        if (null == uiImage.overrideSprite)
        {
            return 0f;
        }

        return uiImage.overrideSprite.rect.width / uiImage.pixelsPerUnit;
    }

    public static float GetImageHeight(uint windowId)
    {
        var uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return 0f;
        }

        if (null == uiImage.overrideSprite)
        {
            return 0f;
        }

        return uiImage.overrideSprite.rect.height / uiImage.pixelsPerUnit;
    }

    public static void SetMaterialFloat(uint windowId, string Propety, float Value)
    {
        Image uiImage = PanelService.GetWindowComponent<Image>(windowId);
        RawImage rawImage = PanelService.GetWindowComponent<RawImage>(windowId);
        if (null == uiImage && null == rawImage)
        {
            return;
        }

        var mat = (null != uiImage) ? uiImage.material : rawImage.material;

        if (mat != null)
        {
            mat.SetFloat(Propety, Value);
        }
    }

    public static void SetColor(uint windowId, float r, float g, float b, float a)
    {
        Image uiImage = PanelService.GetWindowComponent<Image>(windowId);
        if (null == uiImage)
        {
            return;
        }
        uiImage.color = new Color(r, g, b, a);
    }

    public static void UnloadUnusedAssets()
    {
        IconService.instance.UnloadUnusedAssets();
    }

}