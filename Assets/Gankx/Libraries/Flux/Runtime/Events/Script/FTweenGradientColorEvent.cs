using UnityEngine;

namespace Flux
{

    [FEvent("Text/Gradient Color")]
    public class FTweenGradientColorEvent : FTweenEvent<FTweenColor> {
        private TextGradient _gradient;

        protected override void OnInit() {
            _gradient = Owner.GetComponent<TextGradient>();
        }

        protected override void SetDefaultValues() {
            _tween = new FTweenColor(Color.white, Color.black);
        }

        protected override void OnTrigger(float timeSinceTrigger) {
            if (_gradient == null)
                return;

            _gradient.gameObject.SetActive(true);
            OnUpdateEvent(timeSinceTrigger);
        }

        protected override void ApplyProperty(float t) {
            _gradient.TopColor = _tween.GetValue(t);
            _gradient.SetDirty();
        }

        protected override void OnStop() {
            if (_gradient == null)
                return;

            _gradient.gameObject.SetActive(false);
        }
    }
}
