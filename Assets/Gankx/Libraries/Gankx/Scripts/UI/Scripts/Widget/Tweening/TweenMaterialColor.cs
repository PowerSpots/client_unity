using UnityEngine;

public class TweenMaterialColor : TweenMaterial {

	public Color from = Color.white;
    public Color to = Color.black;

    public Color value {
        get {
            if (cachedMaterial != null && cachedMaterial.HasProperty(id)) {
                return cachedMaterial.GetColor(id);
            }
            return Color.black;
        }
        set {
            if (cachedMaterial != null && cachedMaterial.HasProperty(id)) {
                cachedMaterial.SetColor(id, value);
            }
        }
    }

    protected override void OnUpdate(float factor, bool isFinished) {
        value = Color.Lerp(from, to, factor);
    }

    public override void SetStartToCurrentValue() {
        from = value;
    }

    public override void SetEndToCurrentValue() {
        to = value;
    }
}
