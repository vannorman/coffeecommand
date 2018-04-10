using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryUpload : MonoBehaviour {

	public UnityEngine.UI.Image fill;
	void Start(){
		uploadButton.SetActive (false);
	}

	public GameObject uploadButton;
	public void SetFillAmount(float f){
//		Debug.Log ("Fill...:" + f);
		fill.fillAmount = f;
		if (f >= 1) {
			uploadButton.SetActive (true);	
		}
	}
}
