using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coins : MonoBehaviour {

	public int ct = 0;
	public int target = 0;
	public void EarnCoin(int newCt){
		target += newCt;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	float t = 0;
	void Update () {
		if (ct != target) {
			t -= Time.deltaTime;
			float lerpDuration = 1.5f;
			float interval = (target - ct) / lerpDuration;
			if (t < interval) {
				t = 0;
				ct = Mathf.Min (ct + 1, target);
			}
		}
			
		GetComponent<Text> ().text = ct.ToString ();
	}
}
