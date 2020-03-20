using System;
using UnityEngine;

public class UITextureExport
{
    public static void SetUnrefMaxSize(uint number)
    {
        TextureManager.instance.SetUnrefMaxSize(number);
    }

    public static void UnloadAllAssets()
    {
        TextureManager.instance.ClearTextureAll();
    }

    public static void RemoveAllLoadTasks()
    {
        TextureManager.instance.RemoveAllLoadTasks();
    }
}
