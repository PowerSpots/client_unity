using UnityEngine;

namespace PerfAssist.Editor.Treemap
{
    interface ITreemapRenderable
    {
        Color GetColor();
        Rect GetPosition();
        string GetLabel();
    }
}
