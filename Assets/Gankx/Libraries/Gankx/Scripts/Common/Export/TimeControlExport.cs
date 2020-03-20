using Gankx;
using UnityEngine;

public class TimeControlExport
{
    public static void SetScale(int layer, float timeScale)
    {
        TimeControl.SetScale((TimeControl.Layer) layer, timeScale);
    }

    public static float GetScale(int layer)
    {
        return TimeControl.GetScale((TimeControl.Layer) layer);
    }

    public static void ResetScale()
    {
        TimeControl.ResetScale();
    }

    public static float GetDeltaTime()
    {
        return Time.deltaTime;
    }

    public static double GetUnscaledDeltaTime()
    {
        return Time.unscaledDeltaTime;
    }
}