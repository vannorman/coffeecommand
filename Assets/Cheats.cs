using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour {
	bool cheatsEnabled = true;
	void Update(){
		#if UNITY_EDITOR
		if (cheatsEnabled) DetectCheatKeys();
		#endif
	}

	void DetectCheatKeys(){
		if (Input.GetKey (KeyCode.F)) {
//			Debug.Log ("fill");
			FindObjectOfType<BatteryUpload> ().SetFillAmount (FindObjectOfType<BatteryUpload> ().fill.fillAmount + 0.01f);

		}
	}
}
