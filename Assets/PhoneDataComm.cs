using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wrld;
using Wrld.Space;
using System.Linq;

public class PhoneDataComm : MonoBehaviour {

	public Camera cam;
	public WrldMap wrldMap;
	public Transform wrldMapParent;
	bool waitingForLocation = true;
	public Text db;
	public GameObject locationPrefab;
	Text pnt; 
	void Start () {
//		LibPlacenote.in
		pnt = GameObject.Find("Placenote track").GetComponent<Text>();

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
			Debug.Log ("lat lng set to SF.");
		} else {
			lat = Input.location.lastData.latitude;
			lng = Input.location.lastData.longitude;
			rot = Input.compass.trueHeading;
		}


		wrldMap.SetLatLng ((double)lat, (double)lng);



		cam.transform.Rotate (Vector3.up, rot);
		wrldMapParent.gameObject.SetActive (true);

		if (LibPlacenote.Instance.Initialized ()) {
			ListNearbyMaps ();
		} else {
			onInitializedDelegate += OnPlacenoteSdkInitialized;
		}


	}

	public delegate void OnInitializedDelegate();
	public OnInitializedDelegate onInitializedDelegate;
	bool initializeFinished = false;

	void OnPlacenoteSdkInitialized(){
		onInitializedDelegate -= OnPlacenoteSdkInitialized;
		ListNearbyMaps ();
	}

	void ListNearbyMaps(){
		pnt.text += "Listing nearby maps.\n";
		if (!LibPlacenote.Instance.Initialized()) {
			pnt.text += "Oops sdk not ready..\n";
//			ToastManager.ShowToast ("SDK not yet initialized", 2f);
			return;
		}
			

		float radius = 5;
		LibPlacenote.Instance.SearchMaps (Input.location.lastData.latitude, Input.location.lastData.longitude, radius, (mapList) => {
			foreach (LibPlacenote.MapInfo mapId in mapList) {
				pnt.text += "map! " +mapId.placeId+ " .\n";		
				if (mapId.metadata.userdata != null) {
					
					pnt.text += "making prefab. \n";
					float lat = mapId.metadata.userdata ["location"] ["latitude"].ToObject<float> ();
					float lng = mapId.metadata.userdata ["location"] ["longitude"].ToObject<float> ();


					GeographicTransform coordinateFrame = (GeographicTransform)Instantiate(locationPrefab.GetComponent<GeographicTransform>());
					Api.Instance.GeographicApi.RegisterGeographicTransform(coordinateFrame);
					LatLong pointA = LatLong.FromDegrees(lat,lng);
					coordinateFrame.SetPosition(pointA);
					pnt.text += "prefab placed. \n";
					coordinateFrame.GetComponent<MapMarkerInfo>().people.text = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoJson.CCO>(Newtonsoft.Json.JsonConvert.SerializeObject(mapId.metadata.userdata["CoffeeCommandObject"])).mine.coins.count.ToString();
					coordinateFrame.GetComponent<MapMarkerInfo>().coins.text = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoJson.CCO>(Newtonsoft.Json.JsonConvert.SerializeObject(mapId.metadata.userdata["CoffeeCommandObject"])).visitors.Select(e => e.user).Distinct().ToList().Count.ToString();
					coordinateFrame.GetComponent<MapMarkerInfo>().mapInfo = mapId;
					//var uniqueCerts = cco.visitors.Select(e => e.user).Distinct().ToList(); //  SelectMany(e => e.user).Distinct().ToList();
				} else {

					pnt.text += "... but null. \n";
				}
			}

		});

//		LibPlacenote.Instance.ListMaps ((mapList) => {
			// render the map list!
//			foreach (LibPlacenote.MapInfo mapId in mapList) {
//				pnt.text += "map! " +mapId.placeId+ " .\n";		
//				if (mapId.userData == null) {
//					pnt.text += "... but null. \n";
////					Debug.LogError (mapId.userData.ToString (Formatting.None));
//				} else {
////					Debug.Log("mapid:"+mapId.placeId);
//					pnt.text += "making prefab. \n";
//					float lat = mapId.userData ["location"] ["latitude"].ToObject<float> ();
//					float lng = mapId.userData ["location"] ["longitude"].ToObject<float> ();
//					var distance = MapInfoElement.Calc (Input.location.lastData.latitude, Input.location.lastData.longitude,
//						lat,
//						lng);
//					if (distance < radius){
//						GeographicTransform coordinateFrame = (GeographicTransform)Instantiate(locationPrefab.GetComponent<GeographicTransform>());
//						Api.Instance.GeographicApi.RegisterGeographicTransform(coordinateFrame);
//						LatLong pointA = LatLong.FromDegrees(lat,lng);
//						coordinateFrame.SetPosition(pointA);
//						pnt.text += "prefab placed. \n";
//					} else {
//						pnt.text += "too far: "+distance + "\n";
//					}
//				}

//			}
//		});
	}


	
	// Update is called once per frame
	void Update () {

		// so awkward. I couldn't figure out how to hook into Placenote's initialized callback.
		if (!initializeFinished){
			if (LibPlacenote.Instance.Initialized ()) {
				initializeFinished = true;
				if (onInitializedDelegate != null) {
					onInitializedDelegate();
				}
			}
		}
		
//		db.text = "Waiting for location:" + waitingForLocation + "\n" +
//			"Location Service status:" +Input.location.status +"\n" +
//			"";

//		pnt.text = LibPlacenote.Instance.Initialized().ToString();
		if (waitingForLocation) {
			if (Input.location.status == LocationServiceStatus.Running) {
				OnLocationRunning ();
			}
			#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.L)){
				OnLocationRunning();
			}
			#endif
		}

			
	}
}
