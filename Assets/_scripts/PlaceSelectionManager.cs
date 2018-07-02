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

		LibPlacenote.MapInfo mapToLoad;

		public void SetMapToLoad(LibPlacenote.MapInfo map){
			mapToLoad = map;
		}
		public GameObject locationPrefab;
//		Text pnt; 
		void Start () {

//
//			debugGroup.SetActive (false);
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

		float minimumSpaceSeparation = 0.03f; 

		void ListNearbyMaps(){
			// Only runs at the start of app.

//			debugGroup.SetActive (true);
			loadingMap.gameObject.SetActive (false);
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



	//		LibPlacenote.Instance.ListMaps(  (mapList) => {
			LibPlacenote.Instance. SearchMaps (Input.location.lastData.latitude, Input.location.lastData.longitude, radius, (mapList) => {
//				pnt.text += "Your location:"+ Input.location.lastData.latitude.ToString("#.000")+","+Input.location.lastData.longitude.ToString("#.000")+"\n";
//				pnt.text += "Search finished! Maps: "+mapList.Length+"\n";
				bool foundCloseMap = false; // if this remains false we allow "new" button to show
				foreach (LibPlacenote.MapInfo mapId in mapList) {

					float lat = mapId.metadata.location.latitude;
					float lng = mapId.metadata.location.longitude;

					UserDataManager.CoffeeCommandObject userdata = mapId.metadata.userdata.ToObject<UserDataManager.CoffeeCommandObject>();
					if (userdata.invisible) {
//						CLogger.Log("invisible , skipping:"+mapId.metadata.name);
						continue;
					}
//					CLogger.Log("list got map:"+mapId.placeId);

					GeographicTransform coordinateFrame = (GeographicTransform)Instantiate(locationPrefab.GetComponent<GeographicTransform>());
					Api.Instance.GeographicApi.RegisterGeographicTransform(coordinateFrame);
					LatLong pointA = LatLong.FromDegrees(lat,lng);
					coordinateFrame.SetPosition(pointA);
					// Are you sure it's necessary to deseralize and reserialize this object?!

					coordinateFrame.GetComponent<MapMarkerInfo>().SetMapInfo(mapId);


					var distance = MapInfoElement.Calc (Input.location.lastData.latitude, Input.location.lastData.longitude,
						lat,
						lng);

					if (distance < minimumSpaceSeparation){
						foundCloseMap = true;
	//						newButton.SetActive(false);
					}



				}
				if (foundCloseMap == false) {
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

		public void CreateNewPlace(){
//			CoffeeCommandView.inst.StartNewMap ();

		}

		public void LoadPlace(){
		}
	}

}