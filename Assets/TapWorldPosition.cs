using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class TapWorldPosition : MonoBehaviour {

	UnityEngine.UI.Text db;
	// Use this for initialization
	void Start () {
		db = GameObject.Find ("DebugText").GetComponent<UnityEngine.UI.Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0) {
			Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch(0).position);
			foreach (RaycastHit hit in Physics.RaycastAll(ray)) {
				db.text = "hit:" + hit.collider.name + "\n";
			}

		}
	}
}
