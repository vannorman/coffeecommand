using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneDataComm : MonoBehaviour {

	public Camera cam;
	public WrldMap wrldMap;
	public Transform wrldMapParent;
	bool waitingForLocation = true;
	public Text db;
	void Start () {

		Input.location.Start ();
	}

	void OnLocationRunning(){
		waitingForLocation = false;
		float lat = 0;
		float lng = 0;
		float rot = 0;
		if (Input.location.status != LocationServiceStatus.Running) {
//			ToastManager.ShowToast ("Location services not enabled!");
			lat = 37.7749f;
			lng = -122.4194f;
			rot = -45;
		} else {
			lat = Input.location.lastData.latitude;
			lng = Input.location.lastData.longitude;
			rot = Input.compass.trueHeading;
		}


		wrldMap.SetLatLng ((double)lat, (double)lng);



		cam.transform.Rotate (Vector3.up, rot);
		wrldMapParent.gameObject.SetActive (true);

	}


	
	// Update is called once per frame
	void Update () {
		db.text = "Waiting for location:" + waitingForLocation + "\n" +
			"Location Service status:" +Input.location.status +"\n" +
//			"Location Service status failed:" + LocationServiceStatus.Failed.ToString() + "\n" +
//			"Location Service status stopped:" + LocationServiceStatus.Stopped.ToString() + "\n" +
//			"Location Service status running:" + LocationServiceStatus.Running.ToString() + "\n" +
		"";
		if (waitingForLocation) {
			if (Input.location.status == LocationServiceStatus.Running) {
				OnLocationRunning ();
			}
		}
			
	}
}
