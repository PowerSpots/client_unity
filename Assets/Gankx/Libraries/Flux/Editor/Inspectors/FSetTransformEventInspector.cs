using UnityEngine;
using UnityEditor;
using Flux;

namespace FluxEditor
{
	[CustomEditor(typeof(FSetTransformEvent))]
	public class FSetTransformEventInspector : FEventInspector
	{
		private FSetTransformEvent _event = null;
        protected override void OnEnable()
		{
			base.OnEnable();

			if( target == null )
			{
				DestroyImmediate( this );
				return;

			}
        }

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI();

		    if (GUILayout.Button("Paste owner transform"))
		    {
                _event = (FSetTransformEvent)target;
                _event.localPosition = _event.Owner.transform.localPosition;
                _event.localRotation = _event.Owner.transform.localRotation.eulerAngles;
                _event.localScale = _event.Owner.transform.localScale;
            }
			serializedObject.Update();

			serializedObject.ApplyModifiedProperties();
		}
	}
}