using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPlayerPrefsOnClick : MonoBehaviour {

	public void ClearNow(){
		PlayerPrefs.DeleteAll ();
	}
}
