using System.Collections;
using Gankx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MediaPlayerCtrl))]
[RequireComponent(typeof(RawImage))]
public class CGPanel : MonoBehaviour
{
    [SerializeField]
    private MediaPlayerCtrl mpc;
    [SerializeField]
    private RawImage renderImage;

    public delegate void MediaEnd();
    public static MediaEnd OnMediaEnd;

    public delegate void VideoFirstFrameReady();
    public static VideoFirstFrameReady OnVideoFirstFrameReady;

    private string curMediaName;

    public float volumn = 0.5f;
    private bool isReady = false;

    public void OnPlayEnd()
    {
        mpc.OnEnd = null;
        gameObject.SetActive(false);
        if (null != renderImage)
        {
            renderImage.enabled = false;
        }

        if (null != mpc)
        {
            if(!string.IsNullOrEmpty(mpc.m_strFileName))
                mpc.UnLoad();

            mpc.m_bAutoPlay = false;
        }

        if (null != OnMediaEnd)
        {
            OnMediaEnd();
        }
        Debug.Log("Messenger.Broadcast('OnCgPlayEnd', curMediaName);");
        Messenger.Broadcast("OnCgPlayEnd", curMediaName);
        //Destroy(gameObject);
    }

    public void Play(string mediaName)
    {
        if (string.IsNullOrEmpty(mediaName))
        {
            Debug.LogError("CGPanel MediaName is empty");
            OnPlayEnd();
            return;
        }

        if (null == mpc)
        {
            Debug.LogError("CGPanel MediaPlayerCtrl  Component is miss");
            OnPlayEnd();
            return;
        }

        if (null == renderImage)
        {
            Debug.LogError("CGPanel renderImage  Component is miss");
            OnPlayEnd();
            return;
        }
        curMediaName = mediaName;
        gameObject.SetActive(true);
        mpc.Load(mediaName);
        mpc.m_bAutoPlay = true;
        mpc.SetVolume(volumn);
        mpc.OnVideoError = OnVideoError;
        mpc.OnEnd = OnPlayEnd;
        mpc.Pause();
        mpc.Play();
        isReady = false;
        //if first frame is ready open render image immediately
        if (mpc.IsFirstFrameReady()) {
            MPCVideoFirstFrameReady();
        }
        //if first frame is not ready then wait for its ready then open the render image
        else {
            mpc.OnVideoFirstFrameReady = MPCVideoFirstFrameReady;
        }
    }

    void MPCVideoFirstFrameReady() {
        isReady = true;
        Messenger.Broadcast("VideoFirstFrameReady");
        renderImage.enabled = true;
        if (null != OnVideoFirstFrameReady) {
            OnVideoFirstFrameReady();
        }
    }

   /// <summary>
   /// Test Code Only
   /// </summary>
    void DelayMPCVideoFirstFrameReady() {
        StartCoroutine(OnDelayMPCVideoFirstFrameReady());
    }

    IEnumerator OnDelayMPCVideoFirstFrameReady() {
        mpc.Pause();
        yield return new WaitForSeconds(3);
        mpc.Play();
        isReady = true;
        Messenger.Broadcast("VideoFirstFrameReady");
        renderImage.enabled = true;
        if (null != OnVideoFirstFrameReady) {
            OnVideoFirstFrameReady();
        }
    }

    

    public bool IsFirstFrameReady() {
        if (mpc == null) {
            return false;
        }

        return isReady;
    }

    void OnVideoError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra)
    {
        OnPlayEnd();
    }

    void OnDestroy()
    {
        OnPlayEnd();
        OnMediaEnd = null;
    }

    public string GetCurMediaName()
    {
        return curMediaName;
    }
}
