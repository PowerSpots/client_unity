namespace Flux
{

    [FEvent("Text/Gradient Offset")]
    public class FTweenGradientOffsetEvent : FTweenEvent<FTweenFloat> {
        private TextGradient _gradient;

        protected override void OnInit() {
            _gradient = Owner.GetComponent<TextGradient>();
        }

        protected override void SetDefaultValues() {
            _tween = new FTweenFloat(-1, 1);
        }

        protected override void OnTrigger(float timeSinceTrigger) {
            if (_gradient == null)
                return;
            
            _gradient.gameObject.SetActive(true);
            OnUpdateEvent(timeSinceTrigger);
        }

        protected override void ApplyProperty(float t) {
            _gradient.m_gradientOffsetVertical = _tween.GetValue(t);
            _gradient.SetDirty();
        }
    }
}

