using System.Collections;
using System.Collections.Generic;
using Gankx;
using UnityEngine;


namespace Flux
{
    [FEvent("Time/Timescale")]
	public class FTimescaleEvent : FEvent {

		[SerializeField]
		private AnimationCurve _curve;
		public AnimationCurve Curve { get { return _curve; } set { _curve = value; } }

		[SerializeField]
		[Tooltip("Set Time.timescale back to 1 at the end?")]
		private bool _clearOnFinish = true;
		public bool ClearOnFinish { get { return _clearOnFinish; } set { _clearOnFinish = value; } }

        private float curT = 0f;

		protected override void SetDefaultValues ()
		{
			_curve = new AnimationCurve( new Keyframe[]{ new Keyframe(0, 1) } );
		}

		protected override void OnFrameRangeChanged( FrameRange oldFrameRange )
		{
            // 强制刷到0-1
            FUtility.ResizeAnimationCurve(_curve, 1);
        }

        IEnumerator ScaleTimeRoutine()
        {
            while (true)
            {
                curT = (Time.unscaledDeltaTime / LengthTime) + curT;
                TimeControl.SetScale(TimeControl.Layer.Sequence, Mathf.Clamp(_curve.Evaluate(curT), 0, 100));                

                if (curT < 1)
                {
                    yield return null;
                }
                else
                {
                    if (ClearOnFinish)
                    {
                        TimeControl.SetScale(TimeControl.Layer.Sequence, 1);
                        StopAllCoroutines();
                    }
                    yield break;
                }
            }            
        }

		protected override void OnTrigger( float timeSinceTrigger )
		{            
		    curT = 0f;
		    StartCoroutine(ScaleTimeRoutine());
		}        

		protected override void OnStop()
		{
		    TimeControl.SetScale(TimeControl.Layer.Sequence, 1);
            StopAllCoroutines();
		}
	}
}