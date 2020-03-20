//using System.Linq;
using Gankx.UI;
using UnityEngine;

public class UIAnimatorExport
{
    public static void SetEnabled(uint windowId, bool isEnabled)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (null == window)
        {
            return ;
        }

        Animator animator = ((Component) window).GetComponent<Animator>();
        if (null == animator)
        {
            return;
        }

        animator.enabled = isEnabled;
    }

    public static bool CurrentStateIsName(uint windowId, int index, string name)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (null == window)
        {
            return false;
        }

        Animator animator = ((Component) window).GetComponent<Animator>();
        if (null == animator)
        {
            return false;
        }
        return animator.GetCurrentAnimatorStateInfo(index).IsName(name);
    }

    public static void SetBool(uint windowId, string triggerName, bool state, bool isRecursive)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (null == window)
        {
            return;
        }

        if (isRecursive)
        {
            Animator[] animators = window.GetComponentsInChildren<Animator>();
            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].SetBool(triggerName, state);
            }
        }
        else
        {
            Animator animator = ((Component) window).GetComponent<Animator>();
            if (null == animator)
            {
                return;
            }
            animator.SetBool(triggerName, state);
        }
    }
    public static void SetTrigger(uint windowId, string triggerName, bool isRecursive)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (null == window)
        {
            return;
        }

        if (isRecursive)
        {
            Animator[] animators = window.GetComponentsInChildren<Animator>();
            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].SetTrigger(triggerName);
            }
        }
        else
        {
            Animator animator = ((Component) window).GetComponent<Animator>();
            if (null == animator)
            {
                return;
            }
            animator.SetTrigger(triggerName);
        }
    }

    public static void ResetTrigger(uint windowId, string triggerName, bool isRecursive)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (null == window)
        {
            return;
        }

        if (isRecursive)
        {
            Animator[] animators = window.GetComponentsInChildren<Animator>();
            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].ResetTrigger(triggerName);
            }
        }
        else
        {
            Animator animator = ((Component) window).GetComponent<Animator>();
            if (null == animator)
            {
                return;
            }
            animator.ResetTrigger(triggerName);
        }
    }

    public static void ResetAllTrigger(uint windowId, bool isRecursive)
    {
        var window = PanelService.instance.GetWindow(windowId);
        if (null == window)
        {
            return;
        }

        if (isRecursive)
        {
            var animators = window.GetComponentsInChildren<Animator>();
            foreach (var t in animators)
            {
                foreach (var param in t.parameters)
                {
                    if (param.type == AnimatorControllerParameterType.Trigger)
                    {
                        t.ResetTrigger(param.name);
                    }
                }
            }
        }
        else
        {
            var animator = ((Component) window).GetComponent<Animator>();
            if (null == animator)
            {
                return;
            }
            foreach (var param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.ResetTrigger(param.name);
                }
            }
        }
    }


    public static void SetInteger(uint windowId, string name, int value, bool isRecursive)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (null == window)
        {
            return;
        }

        if (isRecursive)
        {
            Animator[] animators = window.GetComponentsInChildren<Animator>();
            for (int i = 0; i < animators.Length; i++)
            {

                animators[i].SetInteger(name, value);
            }
        }
        else
        {
            Animator animator = ((Component) window).GetComponent<Animator>();
            if (null == animator)
            {
                return;
            }
            animator.SetInteger(name, value);
        }
    }

    public static bool IsInRunning(uint windowId, int layerIndex, string animName)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (window == null)
        {
            return false;
        }

        Animator[] animators = window.GetComponentsInChildren<Animator>();
        if (animators == null || animators.Length == 0 || animators[0] == null)
        {
            return false;
        }

        return animators[0].GetCurrentAnimatorStateInfo(layerIndex).IsName(animName);
    }
    public static void Speed(uint windowId, float deltaTime)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (window == null)
        {
            return;
        }

        Animator[] animators = window.GetComponentsInChildren<Animator>();
        if (animators == null || animators[0] == null)
        {
            return;
        }

        animators[0].speed = deltaTime;
    }

    public static string GetCurAnimName(uint windowId)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (window == null)
        {
            return string.Empty;
        }

        Animator animator = ((Component) window).GetComponent<Animator>();
        if (animator == null)
        {
            return string.Empty;
        }

        AnimatorClipInfo[] animClipInfos = animator.GetCurrentAnimatorClipInfo(0);
        if (animClipInfos.Length == 0)
        {
            return string.Empty;
        }

        return animClipInfos[0].clip.name;
    }

    public static void SetAnimFinished(uint windowId, string animName)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (window == null)
        {
            return;
        }

        Animator animator = ((Component) window).GetComponent<Animator>();
        if (animator == null)
        {
            return;
        }

        animator.Play(animName, 0, 1.0f);
    }

    public static void Update(uint windowId, int layerIndex)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (window == null)
        {
            return;
        }

        Animator[] animators = window.GetComponentsInChildren<Animator>();
        if (animators == null || animators[0] == null)
        {
            return;
        }


        AnimatorClipInfo[] clips = animators[0].GetCurrentAnimatorClipInfo(layerIndex);
        if (clips == null || clips.Length == 0)
        {
            return;
        }

        if (clips[0].clip != null)
        {
            animators[0].Update(clips[0].clip.length);
        }
    }
}
