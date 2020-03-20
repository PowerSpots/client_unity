using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenAdapterCom : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		UIScreenAdaptor.SetCanvasScaler(gameObject);
	}
}
