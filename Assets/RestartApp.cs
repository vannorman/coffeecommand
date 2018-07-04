using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartApp : MonoBehaviour {

	public void Restart(){
		LibPlacenote.Instance.StopSession ();
		Application.LoadLevel (Application.loadedLevel);

	}
}

