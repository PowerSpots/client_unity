using UnityEngine;

/// <summary>
/// Fade组件，该组件的使用者可以在调用Fade函数后，在自身的LateUpdate去调用GetFadeValue来获取Fade的值
/// </summary>
public class FadeComponent : MonoBehaviour
{
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;

    /// <summary>
    /// 当前的fade状态
    /// </summary>
    protected enum FadeStatus
    {
        None = 0,
        FadeIn = 1,
        FadeOut = 2,
    }

    private float fadeValue = 0;
    private float FadeValue
    {
        get { return fadeValue; }
        set { fadeValue = Mathf.Max(Mathf.Min(1f, value), 0f); }
    }

    /// <summary>
    /// 当前fade的状态
    /// </summary>
    protected FadeStatus status = FadeStatus.None;
    private System.Action fadeInEndCallback;
    private System.Action fadeOutEndCallback;
    private System.Action<float> fadeValueCallback;

    public void FadeOut(System.Action fadeEndCallback = null, System.Action<float> fadeCallback = null)
    {
        if (status == FadeStatus.FadeOut)
        {
            Debug.LogWarning("FadeComponent.FadeOut occured warning. The last fadeout process has not finished.");
        }

        // 在fade过程中调用FadeOut不改变fadeValue
        if (status == FadeStatus.None)
        {
            FadeValue = 1;
        }

        status = FadeStatus.FadeOut;
        fadeInEndCallback = null;
        fadeOutEndCallback = fadeEndCallback;
        fadeValueCallback = fadeCallback;
    }

    public void FadeIn(System.Action fadeEndCallback = null, System.Action<float> fadeCallback = null)
    {
        if (status == FadeStatus.FadeIn)
        {
            Debug.LogWarning("FadeComponent.FadeIn occured warning. The last fadein process has not finished.");            
        }

        // 在fade过程中调用FadeIn不改变fadeValue
        if (status == FadeStatus.None)
        {
            FadeValue = 0;
        }

        status = FadeStatus.FadeIn;
        fadeOutEndCallback = null;
        fadeInEndCallback = fadeEndCallback;
        fadeValueCallback = fadeCallback;
    }

    /// <summary>
    /// 获取fade的值，fadeIn的起始值是0，fadeOut的起始值是1
    /// </summary>
    /// <returns></returns>
    public float GetFadeValue()
    {
        return FadeValue;
    }

    /// <summary>
    /// 获取是否正在fading过程中
    /// </summary>
    /// <returns></returns>
    public bool IsFading()
    {
        return status != FadeStatus.None;
    }

    void Update()
    {
        if (status == FadeStatus.None)
        {
            return;
        }

        float totalTime = fadeInDuration;
        if (status == FadeStatus.FadeOut)
        {
            totalTime = -fadeOutDuration;
        }

        float deltaValue = Time.deltaTime / totalTime;
        FadeValue = FadeValue + deltaValue;

        if (null != fadeValueCallback)
        {
            fadeValueCallback(FadeValue);
        }

        if (status == FadeStatus.FadeIn && FadeValue >= 1f ||
            status == FadeStatus.FadeOut && FadeValue <= 0f)
        {
            OnFadeEnd();

            return;
        }
    }

    /// <summary>
    /// fade结束调用的子函数，子类可以重载，默认行为是开启和关闭target组件
    /// </summary>
    private void OnFadeEnd()
    {
        status = FadeStatus.None;

        if (null != fadeInEndCallback)
        {
            fadeInEndCallback();
            fadeInEndCallback = null;
        }

        if (null != fadeOutEndCallback)
        {
            fadeOutEndCallback();
            fadeOutEndCallback = null;
        }
    }

    public void SetFadeOutEndCallback(System.Action fadeEndCallback) {
        fadeOutEndCallback = fadeEndCallback;
    }
}
