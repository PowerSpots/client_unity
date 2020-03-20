using System;
using UnityEngine;

namespace Flux
{
    [FEvent("Game Object/Set Active")]
	public class FSetActiveEvent : FEvent
	{
		[SerializeField]
		private bool _active = true;

		[SerializeField]
		[Tooltip("Does the event set the opposite on the last frame?")]
		private bool _setOppositeOnFinish = true;

	    [SerializeField]
	    [Tooltip("Does the event set the opposite on stop?")]
	    private bool _setOppositeOnStop = true;

        private bool _wasActive = false;

		private GameObject _childGO = null;

        [SerializeField]
	    private string _childPath = "";

	    [SerializeField]
	    private string _childTag = "";
        

        protected override void OnTrigger( float timeSinceTrigger )
        {
            // 有tag先看tag
            if (!String.IsNullOrEmpty(_childTag))
            {
                var childs = Owner.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < childs.Length; i++)
                {
                    if (childs[i].gameObject.tag == _childTag)
                    {
                        _childGO = childs[i].gameObject;
                        _wasActive = _childGO.activeSelf;
                        return;
                    }
                }

                return;
            }

            Transform childTrans = Owner.Find(_childPath);            
            if (childTrans == null)
            {
                return;                
            }

            _childGO = childTrans.gameObject;
			_wasActive = _childGO.activeSelf;
//			_ownerGO.SetActive( _active ); <- not needed since it will be handled OnUpdateEvent
		}

		protected override void OnUpdateEvent( float timeSinceTrigger )
		{
		    if (_childGO != null)
		    {
                if (_childGO.activeSelf != _active)
                    _childGO.SetActive(_active);
            }
		}

		protected override void OnFinish()
		{
			if( _setOppositeOnFinish )
                if (_childGO != null)
    				_childGO.SetActive( !_active );
		}

		protected override void OnStop ()
		{
		    if (_setOppositeOnStop)
		    {
		        if (_childGO != null)
		            _childGO.gameObject.SetActive(_wasActive);
            }            
		}
	}
}
