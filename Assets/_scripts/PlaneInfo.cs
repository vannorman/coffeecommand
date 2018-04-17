using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneInfo : MonoBehaviour {

	public MetalOnion plantedOnion;


	float dotsCheckTimer = 0f;
	float radius = 1f;
	float radius2 = 2f;
	public Text text;
	void Update(){
		dotsCheckTimer -= Time.deltaTime;
		if (dotsCheckTimer < 0) {
//			string s = "";
			dotsCheckTimer = Random.Range (0.4f, 0.6f);
			int insideRad1 = 0;
			int insideRad2 = 0;
			foreach (Vector3 wp in CC.featuresManager.MapPoints) {
//				Vector3 wp = CC.featuresVisualizer.mMap.transform.TransformPoint (p);
				if (Vector3.Magnitude (transform.position - wp) < radius) {
					insideRad1++;
				}
				if (Vector3.Magnitude (transform.position - wp) < radius2) {
					insideRad2++;
				}

			}
			text.text = "Rad 1:" + insideRad1.ToString () + ", rad2:" + insideRad2.ToString ();
		}
	}
}
