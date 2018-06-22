using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class TapWorldPosition : MonoBehaviour {

	public CoffeeCommandView coffeeCommandView;
	UnityEngine.UI.Text db;
	public GameObject loadButton;
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
				MapMarkerInfo mmi = hit.collider.GetComponent<MapMarkerInfo> ();
				if (mmi) {
					foreach (MapMarkerInfo mmi2 in FindObjectsOfType<MapMarkerInfo>()) {
						if (mmi != mmi2) {
							mmi2.fx.SetActive (false);
						}
						else {
							mmi2.fx.SetActive (true);
						}
					}
					loadButton.SetActive (true);

				}
			}

		}
	}
}
