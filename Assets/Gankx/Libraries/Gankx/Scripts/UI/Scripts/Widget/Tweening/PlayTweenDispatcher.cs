using Gankx.UI;
using UnityEngine;

public class PlayTweenDispatcher : EventDispatcher
{
    protected override void OnInit()
    {
        PlayTween playTween = GetComponent<PlayTween>();
        if (null == playTween)
        {
            Debug.LogError("No PlayTween Component is found!!!");
            return;
        }

        playTween.onFinished.AddListener(() => { SendPanelMessage("OnPlayTweenFinish"); });
    }
}
