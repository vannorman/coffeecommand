using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class CCGameManager : MonoBehaviour {

	void Start(){
//		UnityARSessionRunOption options = new UnityARSessionRunOption();
//		options = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;
//		m_session.RunWithConfigAndOptions(config, options);
	}

	public void Reset(){
		
		Application.LoadLevel(Application.loadedLevel);
	}
}
