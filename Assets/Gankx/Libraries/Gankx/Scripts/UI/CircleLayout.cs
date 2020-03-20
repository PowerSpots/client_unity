

/*
Radial Layout Group by Just a Pixel (Danny Goodayle) - http://www.justapixel.co.uk
Copyright (c) 2015
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System.Collections.Generic;

/// Credit Danny Goodayle 
/// Sourced from - http://www.justapixel.co.uk/radial-layouts-nice-and-simple-in-unity3ds-ui-system/
/// Updated by ddreaper - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.
namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("Layout/Extensions/Radial Layout")]
    public class CircleLayout : LayoutGroup
    {
        [Range(0f, 360f)]
        public float MinAngle, MaxAngle, StartAngle;

        public bool SkipInactive = true;

        public bool fillClockwise = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateRadial();
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }

        public override void CalculateLayoutInputVertical()
        {
            CalculateRadial();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            CalculateRadial();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            CalculateRadial();
        }
#endif

        void CalculateRadial()
        {
            m_Tracker.Clear();

            if (transform.childCount == 0)
                return;

            List<RectTransform> childTransList = new List<RectTransform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform child = (RectTransform)transform.GetChild(i);
                if (!SkipInactive)
                {
                    childTransList.Add(child);
                }
                else if (child.gameObject.activeSelf)
                {
                    childTransList.Add(child);
                }
            }

            float fOffsetAngle = ((MaxAngle - MinAngle)) / (childTransList.Count);
            if (fillClockwise)
            {
                fOffsetAngle = -fOffsetAngle;
            }

            float fAngle = StartAngle;
            for (int i = 0; i < childTransList.Count; i++)
            {
                RectTransform child = childTransList[i];
                if (child != null)
                {
                    //Adding the elements to the tracker stops the user from modifiying their positions via the editor.
                    m_Tracker.Add(this, child,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.Rotation |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.Pivot);

                    //float fAngleWithRadian = fAngle * Mathf.Deg2Rad;
                    //Vector3 vPos = new Vector3(Mathf.Cos(fAngleWithRadian), Mathf.Sin(fAngleWithRadian), 0);
                    //child.localPosition = vPos * fDistance;
                    child.localPosition = Vector3.zero;
                    child.localEulerAngles = new Vector3(0, 0, fAngle);
                    ////Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                    child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.0f, 0.5f);

                    fAngle += fOffsetAngle;
                }
            }
        }
    }
}
