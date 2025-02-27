﻿namespace Gankx.UI.AnimationOrTween
{
    public enum Trigger
    {
        OnClick,
        OnHover,
        OnPress,
        OnHoverTrue,
        OnHoverFalse,
        OnPressTrue,
        OnPressFalse,
        OnActivate,
        OnActivateTrue,
        OnActivateFalse,
        OnDoubleClick,
        OnSelect,
        OnSelectTrue,
        OnSelectFalse
    }

    public enum Direction
    {
        Reverse = -1,
        Toggle = 0,
        Forward = 1
    }

    public enum EnableCondition
    {
        DoNothing = 0,
        EnableThenPlay,
        IgnoreDisabledState
    }

    public enum DisableCondition
    {
        DisableAfterReverse = -1,
        DoNotDisable = 0,
        DisableAfterForward = 1
    }
}