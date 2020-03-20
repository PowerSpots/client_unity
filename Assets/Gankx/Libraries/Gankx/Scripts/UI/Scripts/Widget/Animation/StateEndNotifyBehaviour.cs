using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gankx;
using Gankx.UI;

public class StateEndNotifyBehaviour : StateMachineBehaviour {
	bool isFireEvent = false;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		isFireEvent = false;
    }

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if(stateInfo.normalizedTime > 0.99f && !isFireEvent)
		{
			isFireEvent = true;
			LuaService.instance.FireEvent("AnimatorStateEnd");
		}
	}
}
