using System.Collections;
using UnityEngine;

namespace Gankx
{
    public class TransformDirtyTracker : MonoBehaviour
    {
        private int myOldLayer;

        public bool hasChanged { get; private set; }

        private void LateUpdate()
        {
            var layer = gameObject.layer;
            hasChanged = transform.hasChanged || layer != myOldLayer;
            transform.hasChanged = false;

            myOldLayer = layer;
        }

        private void OnEnable()
        {
            hasChanged = true;
            transform.hasChanged = false;

            if (gameObject.activeSelf)
            {
                StartCoroutine(DelayDirty());
            }
        }

        public void SetDirty()
        {
            if (gameObject.activeSelf == false || enabled == false)
            {
                // do nothing
            }
            else
            {
                StartCoroutine(DelayDirty());
            }
        }

        public IEnumerator DelayDirty()
        {
            yield return null;
            hasChanged = true;
            transform.hasChanged = true;
        }
    }
}