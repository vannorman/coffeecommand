using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wrld;
using Wrld.Space;
using System.Linq;

namespace CoffeeCommand {
	public class PlaceSelectionManager : MonoBehaviour {
		
		public static PlaceSelectionManager inst;
		public GameObject mapAvatarPrefab;
		public GameObject createNewMapButton; // need an object manager for MapMode TODO
		public Camera cam;
		public WrldMap wrldMap;
		public Text loadingMap;
		public Transform wrldMapParent;
		bool waitingForLocation = true;
		public GameObject loadButton;

		LibPlacenote.MapInfo mapToLoad;

		public void SetMapToLoad(LibPlacenote.MapInfo map){
			mapToLoad = map;
		}
		public GameObject locationPrefab;
//		Text pnt; 
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
				CLogger.Log ("PN was init");
				ListNearbyPlaces ("pn init, from onlocation running");
			} else {
				CLogger.Log ("PN NOT init, will delegate fire?");
				onInitializedDelegate += OnPlacenoteSdkInitialized;
			}


		}

		public delegate void OnInitializedDelegate();
		public OnInitializedDelegate onInitializedDelegate;
		bool initializeFinished = false;

		void OnPlacenoteSdkInitialized(){
			onInitializedDelegate -= OnPlacenoteSdkInitialized;
			ListNearbyPlaces ("pn sdk init");
		}

		public float minimumSpaceSeparation = 0.04f; // km
		public float visibleDistance = 2;


		void ClearMap (){
			foreach (GeographicTransform gt in FindObjectsOfType<GeographicTransform>()) {
				Api.Instance.GeographicApi.UnregisterGeographicTransform (gt);
				Destroy (gt.gameObject);
			}
		}
		public void ListNearbyPlaces(string source="default"){
			CLogger.Log ("Listing nearby maps ... source:"+source);
			loadButton.SetActive (false);
			createNewMapButton.SetActive (false);

			ClearMap ();
			// Only runs at the start of app.

//			debugGroup.SetActive (true);
			loadingMap.gameObject.SetActive (true);
			CLogger.Log ("List 2:");
//			pnt.text += "Listing nearby maps.\n";
			if (!LibPlacenote.Instance.Initialized()) {
//				pnt.text += "Oops sdk not ready..\n";
				return;
			}
				

			float radius = 50;

			// Place map avatar on phone GPS
			GeographicTransform mapAvatarCF = (GeographicTransform)Instantiate(mapAvatarPrefab.GetComponent<GeographicTransform>());
			Api.Instance.GeographicApi.RegisterGeographicTransform(mapAvatarCF);
			LatLong mapAvatarPosition = LatLong.FromDegrees(Input.location.lastData.latitude ,Input.location.lastData.longitude);
			mapAvatarCF.SetPosition(mapAvatarPosition);
			//ListMapsCheck ();

			LibPlacenote.Instance. SearchMaps (Input.location.lastData.latitude, Input.location.lastData.longitude, radius, (mapList) => {
				
				loadingMap.gameObject.SetActive (false);

				bool foundCloseMap = false; // if this remains false we allow "new" button to show
				foreach (LibPlacenote.MapInfo mapId in mapList) {
					
					if (mapId.metadata == null || mapId.metadata.location == null || mapId.metadata.userdata == null){
						CLogger.Log("meta, loc, or user  null for "+mapId.placeId);
						continue;
					}

					float lat = mapId.metadata.location.latitude;
					float lng = mapId.metadata.location.longitude;

					UserDataManager.CoffeeCommandObject userdata = mapId.metadata.userdata.ToObject<UserDataManager.CoffeeCommandObject>();
					if (userdata.invisible) {
//						CLogger.Log("invisible , skipping:"+mapId.metadata.name);
						continue;
					}

//					CLogger.Log("list got map:"+mapId.placeId);



					var distance = MapInfoElement.Calc (Input.location.lastData.latitude, Input.location.lastData.longitude,
						lat,
						lng);

					if (distance < minimumSpaceSeparation){
						GeographicTransform coordinateFrame = (GeographicTransform)Instantiate(locationPrefab.GetComponent<GeographicTransform>());
						Api.Instance.GeographicApi.RegisterGeographicTransform(coordinateFrame);
						LatLong pointA = LatLong.FromDegrees(lat,lng);
						coordinateFrame.SetPosition(pointA);
						// Are you sure it's necessary to deseralize and reserialize this object?!
						coordinateFrame.GetComponent<MapMarkerInfo>().SetMapInfo(mapId);
						coordinateFrame.GetComponent<MapMarkerInfo>().SetStatus(MapMarkerInfo.Status.Selectable);
						foundCloseMap = true;

					} else  if (distance < visibleDistance ) {
						GeographicTransform coordinateFrame = (GeographicTransform)Instantiate(locationPrefab.GetComponent<GeographicTransform>());
						Api.Instance.GeographicApi.RegisterGeographicTransform(coordinateFrame);
						LatLong pointA = LatLong.FromDegrees(lat,lng);
						coordinateFrame.SetPosition(pointA);
						coordinateFrame.GetComponent<MapMarkerInfo>().SetMapInfo(mapId);
						coordinateFrame.GetComponent<MapMarkerInfo>().SetStatus(MapMarkerInfo.Status.OutOfRange);
					}
//					CLogger.Log ("List 6f:");
				}
				CLogger.Log ("List 7:");
				if (foundCloseMap == false) {
					CLogger.Log ("List 8:");
					createNewMapButton.SetActive(true);
//					pnt.text += "None closeby ...\n";
				}
//				pnt.text += "Finished map pop. \n";
//				CLogger.Log("fin map pop");
			});

		}


		void Update () {

			// so awkward. I couldn't figure out how to hook into Placenote's initialized callback.
			if (!initializeFinished){
				if (LibPlacenote.Instance.Initialized ()) {
					initializeFinished = true;
					if (onInitializedDelegate != null) {
						CLogger.Log ("init del fired from update.");
						onInitializedDelegate();
					}
				}
			}
			

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
			#if UNITY_EDITOR 
			if  (Input.GetKeyDown(KeyCode.N)){
				createNewMapButton.SetActive(true);
			}
			#endif
		}


		public void CreateNewPlaceButtonClicked(){
			GameManager.inst.SetState (GameManager.State.Game);
//			Invoke ("CreateNewPlace", 1.05f);
			UserDataManager.OnMapSelected ();

		}
		public void LoadExistingPlaceButtonClicked(){
			GameManager.inst.SetState (GameManager.State.Game);
//			Invoke ("LoadPlace", 1.05f);
			UserDataManager.OnMapSelected (true, mapToLoad);
			CoffeeCommandView.inst.SelectMap (mapToLoad);
			CoffeeCommandView.inst.LoadSelectedMap ();
		}



		//public void ListMapsCheck(){
		//	//CLogger.Log ("List maps check:");
		//	//CLogger.Log ("LibPlacenote status:" + LibPlacenote.Instance.GetStatus ());

		//	LibPlacenote.Instance.ListMaps ((mapList) => {
		//		CLogger.Log ("List 1a:");
		//		// render the map list!
		//		foreach (LibPlacenote.MapInfo mapId in mapList) {
		//			//CLogger.Log ("List 1b:");
		//			if (mapId.metadata.userdata != null) {
		//				//Debug.Log("list map succeed. id:"+mapId.placeId);
		//			}

		//		}
		//	});
		//	//CLogger.Log ("List 2:");
		//}
	}

}