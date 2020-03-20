using System;
using UnityEngine;

namespace Flux
{
    [Serializable]
    public class FTweenFloatCurved : FTween<float>
    {
        public AnimationCurve EasingCurve;

        public FTweenFloatCurved(float from, float to)
        {
            _from = from;
            _to = to;
        }

        public override float GetValue(float t)
        {
            return Mathf.Lerp(_from, _to, EasingCurve.Evaluate(t));
        }
    }

    [FEvent("Script/Tween Float with Curve")]
    public class FTweenFloatCurvedEvent : FTweenVariableEvent<FTweenFloatCurved>
    {
        protected override object GetValueAt(float t)
        {
            return _tween.GetValue(t);
        }
    }
}
