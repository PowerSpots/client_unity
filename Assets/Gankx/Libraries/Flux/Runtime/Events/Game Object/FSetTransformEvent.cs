using UnityEngine;

namespace Flux
{
    [FEvent("Game Object/Set Transform")]
	public class FSetTransformEvent : FEvent
	{
		[SerializeField]
		public Vector3 localPosition;
        [SerializeField]
        public Vector3 localRotation;
        [SerializeField]
        public Vector3 localScale = Vector3.one;

        public bool isWorld = false;

        [SerializeField]
		[Tooltip("Does the event recover transform on the last frame?")]
		private bool _recoverOnFinish = true;

		private Vector3 oldLocalPosition;
        private Quaternion oldLocalRotation;
        private Vector3 oldLocalScale;

        private GameObject _ownerGO = null;

		protected override void OnTrigger( float timeSinceTrigger )
		{
			_ownerGO = Owner.gameObject;

		    if (!isWorld)
		    {
		        oldLocalPosition = _ownerGO.transform.localPosition;
		        oldLocalRotation = _ownerGO.transform.localRotation;
		        oldLocalScale = _ownerGO.transform.localScale;

		        _ownerGO.transform.localPosition = localPosition;
		        _ownerGO.transform.localRotation = Quaternion.Euler(localRotation);
		        _ownerGO.transform.localScale = localScale;
		    }
		    else
		    {
		        oldLocalPosition = _ownerGO.transform.position;
		        oldLocalRotation = _ownerGO.transform.rotation;
		        oldLocalScale = _ownerGO.transform.lossyScale;

		        _ownerGO.transform.position = localPosition;
		        _ownerGO.transform.rotation = Quaternion.Euler(localRotation);
		        _ownerGO.transform.SetGlobalScale(localScale);
            }
        }

		protected override void OnUpdateEvent( float timeSinceTrigger )
		{

		}

		protected override void OnFinish()
		{
			if(_recoverOnFinish)
            {
                Recover();
            }
		}

		protected override void OnStop ()
		{
            Recover();
        }

        private void Recover()
        {
            if (!isWorld)
            {
                _ownerGO.transform.localPosition = oldLocalPosition;
                _ownerGO.transform.localRotation = oldLocalRotation;
                _ownerGO.transform.localScale = oldLocalScale;
            }
            else
            {
                _ownerGO.transform.position = oldLocalPosition;
                _ownerGO.transform.rotation = oldLocalRotation;
                _ownerGO.transform.SetGlobalScale(oldLocalScale);
            }
        }
	}
}
