using Gankx.UI;
using UnityEngine;

public class TweenerDispatcher : EventDispatcher
{
    protected override void OnInit()
    {
        Tweener tweener = GetComponent<Tweener>();
        if (null == tweener)
        {
            Debug.LogError("No Tweener Component is found!!!");
            return;
        }

        tweener.onFinished.AddListener(() => { SendPanelMessage("OnTweenerFinish"); });
    }
}
