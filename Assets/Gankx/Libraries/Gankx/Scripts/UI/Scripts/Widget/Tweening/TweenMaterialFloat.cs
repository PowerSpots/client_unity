using UnityEngine;

public class TweenMaterialFloat : TweenMaterial {

    public float from = 1;
    public float to = 1;

    public float value {
        get {
            if (cachedMaterial != null && cachedMaterial.HasProperty(id)) {
                return cachedMaterial.GetFloat(id);
            }
            return 0;
        }
        set {
            if (cachedMaterial != null && cachedMaterial.HasProperty(id)) {
                cachedMaterial.SetFloat(id, value);
            }
        }
    }

    protected override void OnUpdate(float factor, bool isFinished) {
        value = Mathf.Lerp(from, to, factor);
    }

    public override void SetStartToCurrentValue() {
        from = value;
    }

    public override void SetEndToCurrentValue() {
        to = value;
    }
}
