
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.XR.iOS;
using System.Runtime.InteropServices;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CoffeeCommand {



	public class CoffeeCommandView : MonoBehaviour, PlacenoteListener
	{
		public static CoffeeCommandView inst;

		public GameObject onionPrefab;
		[SerializeField] GameObject mMapSelectedPanel;
		[SerializeField] GameObject mInitButtonPanel;
		[SerializeField] GameObject mMappingButtonPanel;
		[SerializeField] GameObject mMapListPanel;
		[SerializeField] GameObject mExitButton;
		[SerializeField] GameObject mListElement;
		[SerializeField] RectTransform mListContentParent;
		[SerializeField] ToggleGroup mToggleGroup;
	//	[SerializeField] GameObject mPlaneDetectionToggle;
		[SerializeField] Text mLabelText;
		[SerializeField] public Material mShapeMaterial;
		[SerializeField] PlacenoteARGeneratePlane mPNPlaneManager;
	//	[SerializeField] Slider mRadiusSlider;
		[SerializeField] float mMaxRadiusSearch;
	//	[SerializeField] Text mRadiusLabel;

		private UnityARSessionNativeInterface mSession;
		private bool mFrameUpdated = false;
		private UnityARImageFrameData mImage = null;
		private UnityARCamera mARCamera;
		private bool mARKitInit = false;
		private List<GameObject> shapeObjList = new List<GameObject> ();
		private LibPlacenote.MapMetadataSettable mCurrMapDetails;

		private bool mReportDebug = false;

		private LibPlacenote.MapInfo mSelectedMapInfo;
		private string mSelectedMapId {
			get {
				return mSelectedMapInfo != null ? mSelectedMapInfo.placeId : null;
			}
		}
		private string mSaveMapId = null;


		private BoxCollider mBoxColliderDummy;
		private SphereCollider mSphereColliderDummy;
		private CapsuleCollider mCapColliderDummy;



		#region testing / not user accessible
		public void PlaceOneOnion(){
			ClearOnions ();
			PlaceOnion ();
		}

		public void PlaceOnion(){
			GameObject onion = (GameObject)Instantiate (onionPrefab, Camera.main.transform.position + Camera.main.transform.forward * 2f, Quaternion.identity);
		}
		public void ClearOnions(){
			foreach (MetalOnion mo in FindObjectsOfType<MetalOnion>()) {
				Destroy (mo.gameObject);
			}
		}
		#endregion


		void SpawnDogs(){
			FindObjectOfType<Dogs> ().SpawnDogs();
		}

		// Use this for initialization
		void Start ()
		{
			Input.location.Start ();

			mMapListPanel.SetActive (false);

			mSession = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
			StartARKit ();
			CoffeeCommandFeaturesVisualizer.EnablePointcloud ();
			LibPlacenote.Instance.RegisterListener (this);
	//		mRadiusSlider.value = 1.0f;
		}


		private void ARFrameUpdated (UnityARCamera camera)
		{
			mFrameUpdated = true;
			mARCamera = camera;
		}


		private void InitARFrameBuffer ()
		{
			mImage = new UnityARImageFrameData ();

			int yBufSize = mARCamera.videoParams.yWidth * mARCamera.videoParams.yHeight;
			mImage.y.data = Marshal.AllocHGlobal (yBufSize);
			mImage.y.width = (ulong)mARCamera.videoParams.yWidth;
			mImage.y.height = (ulong)mARCamera.videoParams.yHeight;
			mImage.y.stride = (ulong)mARCamera.videoParams.yWidth;

			// This does assume the YUV_NV21 format
			int vuBufSize = mARCamera.videoParams.yWidth * mARCamera.videoParams.yWidth/2;
			mImage.vu.data = Marshal.AllocHGlobal (vuBufSize);
			mImage.vu.width = (ulong)mARCamera.videoParams.yWidth/2;
			mImage.vu.height = (ulong)mARCamera.videoParams.yHeight/2;
			mImage.vu.stride = (ulong)mARCamera.videoParams.yWidth;

			mSession.SetCapturePixelData (true, mImage.y.data, mImage.vu.data);
		}


		// Update is called once per frame
		void Update ()
		{
			if (mFrameUpdated) {
				mFrameUpdated = false;
				if (mImage == null) {
					InitARFrameBuffer ();
				}

				if (mARCamera.trackingState == ARTrackingState.ARTrackingStateNotAvailable) {
					// ARKit pose is not yet initialized
					return;
				} else if (!mARKitInit && LibPlacenote.Instance.Initialized()) {
					mARKitInit = true;
					mLabelText.text = "ARKit Initialized";
				}

				Matrix4x4 matrix = mSession.GetCameraPose ();

				Vector3 arkitPosition = PNUtility.MatrixOps.GetPosition (matrix);
				Quaternion arkitQuat = PNUtility.MatrixOps.GetRotation (matrix);

				LibPlacenote.Instance.SendARFrame (mImage, arkitPosition, arkitQuat, mARCamera.videoParams.screenOrientation);
			}
		}


	//	public void OnListMapClick ()
	//	{
	//		if (!LibPlacenote.Instance.Initialized()) {
	//			Debug.Log ("SDK not yet initialized");
	//			ToastManager.ShowToast ("SDK not yet initialized", 2f);
	//			return;
	//		}
	//
	//		foreach (Transform t in mListContentParent.transform) {
	//			Destroy (t.gameObject);
	//		}
	//
	//
	//		mMapListPanel.SetActive (true);
	//		mInitButtonPanel.SetActive (false);
	////		mRadiusSlider.gameObject.SetActive (true);
	//		LibPlacenote.Instance.ListMaps ((mapList) => {
	//			// render the map list!
	//			foreach (LibPlacenote.MapInfo mapId in mapList) {
	//				if (mapId.metadata.userdata != null) {
	//					Debug.Log(mapId.metadata.userdata.ToString (Formatting.None));
	//				}
	//				AddMapToList (mapId);
	//			}
	//		});
	//	}

	//	public void OnRadiusSelect ()
	//	{
	//		Debug.Log ("Map search:" + mRadiusSlider.value.ToString("F2"));
	//		LocationInfo locationInfo = Input.location.lastData;
	//
	//		foreach (Transform t in mListContentParent.transform) {
	//			Destroy (t.gameObject);
	//		}
	//
	//		float radiusSearch = mRadiusSlider.value * mMaxRadiusSearch;
	//		mRadiusLabel.text = "Distance Filter: " + (radiusSearch / 1000.0).ToString ("F2") + " km";
	//
	//		LibPlacenote.Instance.SearchMaps(locationInfo.latitude, locationInfo.longitude, radiusSearch, 
	//			(mapList) => {
	//				// render the map list!
	//				foreach (LibPlacenote.MapInfo mapId in mapList) {
	//					if (mapId.metadata.userdata != null) {
	//						Debug.Log(mapId.metadata.userdata.ToString (Formatting.None));
	//					}
	//					AddMapToList (mapId);
	//				}
	//			});
	//	}

		public void OnCancelClick ()
		{
			mMapSelectedPanel.SetActive (false);
			mMapListPanel.SetActive (false);
			mInitButtonPanel.SetActive (true);
		}


		public void OnExitClick ()
		{
			mInitButtonPanel.SetActive (true);
			mExitButton.SetActive (false);
	//		mPlaneDetectionToggle.SetActive (false);

			//clear all existing planes
			mPNPlaneManager.ClearPlanes ();
	//		mPlaneDetectionToggle.GetComponent<Toggle>().isOn = false;

			LibPlacenote.Instance.StopSession ();
		}




		public void SelectMap (LibPlacenote.MapInfo mapInfo)
		{
			mSelectedMapInfo = mapInfo;
			mMapSelectedPanel.SetActive (true);
		}


		public void LoadSelectedMap ()
		{
			ConfigureSession (false);

			if (!LibPlacenote.Instance.Initialized()) {
				Debug.Log ("SDK not yet initialized");
				ToastManager.ShowToast ("SDK not yet initialized", 2f);
				return;
			}

			mLabelText.text = "Loading Map ID: " + mSelectedMapId;
			LibPlacenote.Instance.LoadMap (mSelectedMapId,
				(completed, faulted, percentage) => {
					if (completed) {
						mMapSelectedPanel.SetActive (false);
						mMapListPanel.SetActive (false);
						mInitButtonPanel.SetActive (false);
						mExitButton.SetActive (true);
	//					mPlaneDetectionToggle.SetActive(true);

						LibPlacenote.Instance.StartSession ();

						if (mReportDebug) {
							LibPlacenote.Instance.StartRecordDataset (
								(datasetCompleted, datasetFaulted, datasetPercentage) => {

									if (datasetCompleted) {
										mLabelText.text = "Dataset Upload Complete";
									} else if (datasetFaulted) {
										mLabelText.text = "Dataset Upload Faulted";
									} else {
										mLabelText.text = "Dataset Upload: " + datasetPercentage.ToString ("F2") + "/1.0";
									}
								});
							Debug.Log ("Started Debug Report");
						}

						mLabelText.text = "Loaded ID: " + mSelectedMapId;
					} else if (faulted) {
						mLabelText.text = "Failed to load ID: " + mSelectedMapId;
					} else {
						mLabelText.text = "Map Download: " + percentage.ToString ("F2") + "/1.0";
					}
				}
			);
		}

	//	public void OnDeleteMapClicked ()
	//	{
	//		if (!LibPlacenote.Instance.Initialized()) {
	//			Debug.Log ("SDK not yet initialized");
	//			ToastManager.ShowToast ("SDK not yet initialized", 2f);
	//			return;
	//		}
	//
	//		mLabelText.text = "Deleting Map ID: " + mSelectedMapId;
	//		LibPlacenote.Instance.DeleteMap (mSelectedMapId, (deleted, errMsg) => {
	//			if (deleted) {
	//				mMapSelectedPanel.SetActive (false);
	//				mLabelText.text = "Deleted ID: " + mSelectedMapId;
	//				OnListMapClick();
	//			} else {
	//				mLabelText.text = "Failed to delete ID: " + mSelectedMapId;
	//			}
	//		});
	//	}



		public void StartNewMap ()
		{
			ConfigureSession (false);

			if (!LibPlacenote.Instance.Initialized()) {
				mLabelText.text = "Not initialized, no new map.";
	//			Debug.Log ("SDK not yet initialized");
				return;
			}

			mLabelText.text = "Started map.";
			mInitButtonPanel.SetActive (false);
			mMappingButtonPanel.SetActive (true);
			Debug.Log ("Started Session");
			LibPlacenote.Instance.StartSession ();

			if (mReportDebug) {
				LibPlacenote.Instance.StartRecordDataset (
					(completed, faulted, percentage) => {
						if (completed) {
							mLabelText.text = "Dataset Upload Complete";
						} else if (faulted) {
							mLabelText.text = "Dataset Upload Faulted";
						} else {
							mLabelText.text = "Dataset Upload: (" + percentage.ToString ("F2") + "/1.0)";
						}
					});
				Debug.Log ("Started Debug Report");
			}
			Invoke ("PlaceOneOnion", 2f);
			Invoke ("SpawnDogs", 2f);
		}


		private void StartARKit ()
		{
			mLabelText.text = "Initializing ARKit";
			Application.targetFrameRate = 60;
			ConfigureSession (false);
		}


		private void ConfigureSession(bool clearPlanes) {
			#if !UNITY_EDITOR
			ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration ();


			if (UnityARSessionNativeInterface.IsARKit_1_5_Supported ()) {
				config.planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
			} else {
				config.planeDetection = UnityARPlaneDetection.Horizontal;
			}

			mPNPlaneManager.StartPlaneDetection ();

			if (clearPlanes) {
				mPNPlaneManager.ClearPlanes ();
			}


			config.alignment = UnityARAlignment.UnityARAlignmentGravity;
			config.getPointCloudData = true;
			config.enableLightEstimation = true;
			mSession.RunWithConfig (config);
			#endif
		}


		public void OnSaveMapClick ()
		{
			if (!LibPlacenote.Instance.Initialized()) {
				Debug.Log ("SDK not yet initialized");
				ToastManager.ShowToast ("SDK not yet initialized", 2f);
				return;
			}

			bool useLocation = Input.location.status == LocationServiceStatus.Running;
			LocationInfo locationInfo = Input.location.lastData;


			// Case 0: This is the first map in this location (user did not load a previous map)
			// Save the map normally.
			// Case 1: This is a map loaded from a previous save. 
			// Update that save with new metadata 
			// Save an "invisible" map to keep the mesh


			mLabelText.text = "Saving...";
			LibPlacenote.Instance.SaveMap (
				(mapId) => {
					LibPlacenote.Instance.StopSession ();
					mSaveMapId = mapId;
					mInitButtonPanel.SetActive (true);
					mMappingButtonPanel.SetActive (false);
	//				mPlaneDetectionToggle.SetActive (false);

					//clear all existing planes
					mPNPlaneManager.ClearPlanes ();
	//				mPlaneDetectionToggle.GetComponent<Toggle>().isOn = false;

					LibPlacenote.MapMetadataSettable metadata = new LibPlacenote.MapMetadataSettable();
					metadata.name = RandomName.Get ();
					mLabelText.text = "Saved Map Name: " + metadata.name;


					metadata.userdata = UserDataManager.localDataObject;



					if (useLocation) {
						mLabelText.text = "location!";
						metadata.location = new LibPlacenote.MapLocation();
						metadata.location.latitude = locationInfo.latitude;
						metadata.location.longitude = locationInfo.longitude;
						metadata.location.altitude = locationInfo.altitude;
					} else {
						Debug.Log("no loc");
					}
					LibPlacenote.Instance.SetMetadata (mapId, metadata);
					mCurrMapDetails = metadata;
				},
				(completed, faulted, percentage) => {
					if (completed) {
						mLabelText.text = "Upload Complete:" + mCurrMapDetails.name;
					}
					else if (faulted) {
						mLabelText.text = "Upload of Map Named: " + mCurrMapDetails.name + "faulted";
					}
					else {
						mLabelText.text = "Uploading Map Named: " + mCurrMapDetails.name + "(" + percentage.ToString("F2") + "/1.0)";
					}
				}
			);
		}



		public void OnPose (Matrix4x4 outputPose, Matrix4x4 arkitPose) {}


		public void OnStatusChange (LibPlacenote.MappingStatus prevStatus, LibPlacenote.MappingStatus currStatus)
		{
			Debug.Log ("prevStatus: " + prevStatus.ToString() + " currStatus: " + currStatus.ToString());
			if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.LOST) {
				mLabelText.text = "Localized";
//				LoadShapesJSON (mSelectedMapInfo.metadata.userdata);
			} else if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.WAITING) {
				mLabelText.text = "Mapping";
			} else if (currStatus == LibPlacenote.MappingStatus.LOST) {
				mLabelText.text = "Searching for position lock";
			} else if (currStatus == LibPlacenote.MappingStatus.WAITING) {
				if (shapeObjList.Count != 0) {
//					ClearShapes ();
				}
			}
		}
	}


	/*
	 * 
	  *  Old SDK, updated June 20
	 * using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System;
	using UnityEngine.UI;
	using UnityEngine.XR.iOS;
	using System.Runtime.InteropServices;
	using System.IO;
	using Newtonsoft.Json.Linq;
	using Newtonsoft.Json;

	public enum GameItemType {
		Onion
	}

	[System.Serializable]
	public class OnionInfo
	{
		public float px;
		public float py;
		public float pz;
		public float qx;
		public float qy;
		public float qz;
		public float qw;
		public Vector3 position {
			get {
				return new Vector3 (px, py, pz);
			}
		}
		public Quaternion rotation {
			get {
				return new Quaternion (qx, qy, qz, qw);
			}
		}
	//	public GameItemType itemType;
		public MetalOnion.State onionState;
	}


	[System.Serializable]
	public class OnionList
	{
		public OnionInfo[] onions;
	}



	public class CoffeeCommandView : MonoBehaviour, PlacenoteListener
	{
		[SerializeField] GameObject mMapSelectedPanel;
		[SerializeField] GameObject mInitButtonPanel;
		[SerializeField] GameObject mMappingButtonPanel;
		[SerializeField] GameObject mMapListPanel;
		[SerializeField] GameObject mExitButton;
		[SerializeField] GameObject mListElement;
		[SerializeField] RectTransform mListContentParent;
		[SerializeField] ToggleGroup mToggleGroup;
		[SerializeField] Text mLabelText;

		public GameObject onionPrefab;

		private LibPlacenote.MapMetadataSettable mCurrMapDetails;
		private UnityARSessionNativeInterface mSession;
		private bool mFrameUpdated = false;
		private UnityARImageFrameData mImage = null;
		private UnityARCamera mARCamera;
		private bool mARKitInit = false;
		private List<ShapeInfo> shapeInfoList = new List<ShapeInfo> ();

		private List<GameObject> shapeObjList = new List<GameObject> ();

		private LibPlacenote.MapInfo mSelectedMapInfo;
		private string mSelectedMapId {
			get {
				return mSelectedMapInfo != null ? mSelectedMapInfo.placeId : null;
			}
		}

		private BoxCollider mBoxColliderDummy;
		private SphereCollider mSphereColliderDummy;
		private CapsuleCollider mCapColliderDummy;


		#region testing / not user accessible
		public void PlaceOneOnion(){
			ClearOnions ();
			PlaceOnion ();
		}

		public void PlaceOnion(){
			GameObject onion = (GameObject)Instantiate (onionPrefab, Camera.main.transform.position + Camera.main.transform.forward * 2f, Quaternion.identity);
		}
		public void ClearOnions(){
			foreach (MetalOnion mo in FindObjectsOfType<MetalOnion>()) {
				Destroy (mo.gameObject);
			}
		}
		#endregion

		// Use this for initialization
		void Start ()
		{
			Input.location.Start ();

			mMapListPanel.SetActive (false);

			mSession = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
			StartARKit ();
			CoffeeCommandFeaturesVisualizer.EnablePointcloud ();
			LibPlacenote.Instance.RegisterListener (this);
	//		LibPlacenote.Instance.onInitializedDelegate += OnInitialized ;
		}

	//	bool initialized=  false;
	//	void OnInitialized(){
	//		if (initialized)
	//			return;
	//		initialized = true;
	//		Invoke ("OnNewMapClick", 2);
	//		LibPlacenote.Instance.onInitializedDelegate -= OnInitialized;
	//	}



		private void ARFrameUpdated (UnityARCamera camera)
		{
			mFrameUpdated = true;
			mARCamera = camera;
		}


		private void InitARFrameBuffer ()
		{
			mImage = new UnityARImageFrameData ();

			int yBufSize = mARCamera.videoParams.yWidth * mARCamera.videoParams.yHeight;
			mImage.y.data = Marshal.AllocHGlobal (yBufSize);
			mImage.y.width = (ulong)mARCamera.videoParams.yWidth;
			mImage.y.height = (ulong)mARCamera.videoParams.yHeight;
			mImage.y.stride = (ulong)mARCamera.videoParams.yWidth;

			// This does assume the YUV_NV21 format
			int vuBufSize = mARCamera.videoParams.yWidth * mARCamera.videoParams.yWidth/2;
			mImage.vu.data = Marshal.AllocHGlobal (vuBufSize);
			mImage.vu.width = (ulong)mARCamera.videoParams.yWidth/2;
			mImage.vu.height = (ulong)mARCamera.videoParams.yHeight/2;
			mImage.vu.stride = (ulong)mARCamera.videoParams.yWidth;

			mSession.SetCapturePixelData (true, mImage.y.data, mImage.vu.data);
		}


		JObject ons = new JObject();

		// Update is called once per frame
		void Update ()
		{



			#if UNITY_EDITOR
		
	//		if (Inpu
	//		Onions2JSON

			if (Input.GetKeyDown(KeyCode.S)){
				ons = Onions2JSON();
				Debug.Log("ons:"+ons.ToString());
			}
			if (Input.GetKeyDown(KeyCode.L)){
	//			LoadOnionsJSON(ons);
	//			Debug.Log("lowd:"+ons.ToString());
	//			LoadMapNow("6f817ec9-12b2-4191-b49f-6bb7abe176c7");
			}
	//		if (Input.GetKeyDown(KeyCode.C)){
	//			ClearOnions();
	//		}
			#endif


			if (mFrameUpdated) {
				mFrameUpdated = false;
				if (mImage == null) {
					InitARFrameBuffer ();
				}

				if (mARCamera.trackingState == ARTrackingState.ARTrackingStateNotAvailable) {
					// ARKit pose is not yet initialized
					return;
				} else if (!mARKitInit) {
					mARKitInit = true;
					mLabelText.text = "ARKit Initialized";
				}

				Matrix4x4 matrix = mSession.GetCameraPose ();

				Vector3 arkitPosition = PNUtility.MatrixOps.GetPosition (matrix);
				Quaternion arkitQuat = PNUtility.MatrixOps.GetRotation (matrix);

				if (!localized) {
					// PJ said to stop sending it frames once you localize.
					LibPlacenote.Instance.SendARFrame (mImage, arkitPosition, arkitQuat, mARCamera.videoParams.screenOrientation);
				}
			}
		}

		bool localized = false; // only set true if you load a map.
		public void OnListMapClick ()
		{
			if (!LibPlacenote.Instance.Initialized()) {
				Debug.Log ("SDK not yet initialized");
				ToastManager.ShowToast ("SDK not yet initialized", 2f);
				return;
			}

			foreach (Transform t in mListContentParent.transform) {
				Destroy (t.gameObject);
			}

			mMapListPanel.SetActive (true);
			mInitButtonPanel.SetActive (false);
			LibPlacenote.Instance.ListMaps ((mapList) => {
				// render the map list!
				foreach (LibPlacenote.MapInfo mapId in mapList) {
					if (mapId.metadata != null) {
						Debug.LogError (mapId.metadata.ToString());
					} else {
					}
					AddMapToList (mapId);
				}
			});
		}


		public void OnCancelClick ()
		{
			mMapSelectedPanel.SetActive (false);
			mMapListPanel.SetActive (false);
			mInitButtonPanel.SetActive (true);
		}


		public void OnExitClick ()
		{
			mInitButtonPanel.SetActive (true);
			mExitButton.SetActive (false);
			LibPlacenote.Instance.StopSession ();
		}


		void AddMapToList (LibPlacenote.MapInfo mapInfo)
		{
			GameObject newElement = Instantiate (mListElement) as GameObject;
			MapInfoElement listElement = newElement.GetComponent<MapInfoElement> ();
			listElement.Initialize (mapInfo, mToggleGroup, mListContentParent, (value) => {
				OnMapSelected (mapInfo);
			});
		}


		public void OnMapSelected (LibPlacenote.MapInfo mapInfo)
		{
			mSelectedMapInfo = mapInfo;
			mMapSelectedPanel.SetActive (true);
		}


		public void OnLoadMapClicked ()
		{
			if (!LibPlacenote.Instance.Initialized()) {
				Debug.Log ("SDK not yet initialized");
				ToastManager.ShowToast ("SDK not yet initialized", 2f);
				return;
			}
			ClearOnions ();

			mLabelText.text = "Loading Map ID: " + mSelectedMapId;
			LoadMapNow (mSelectedMapId);



		}

		public void LoadMapNow(string id) {
			LibPlacenote.Instance.LoadMap (id,
				(completed, faulted, percentage) => {
					if (completed) {
						Debug.Log("loaded:"+"");
						mMapSelectedPanel.SetActive (false);
						mMapListPanel.SetActive (false);
						mInitButtonPanel.SetActive (false);
						mExitButton.SetActive (true);
						FindObjectOfType<Dogs> ().SpawnDogs();
						LibPlacenote.Instance.StartSession ();
						mLabelText.text = "Loaded ID: " + mSelectedMapId;




					} else if (faulted) {
						mLabelText.text = "Failed to load ID: " + mSelectedMapId;
					}

				}
			);
		}


		public void OnDeleteMapClicked ()
		{
			if (!LibPlacenote.Instance.Initialized()) {
				Debug.Log ("SDK not yet initialized");
				ToastManager.ShowToast ("SDK not yet initialized", 2f);
				return;
			}

			mLabelText.text = "Deleting Map ID: " + mSelectedMapId;
			LibPlacenote.Instance.DeleteMap (mSelectedMapId, (deleted, errMsg) => {
				if (deleted) {
					mMapSelectedPanel.SetActive (false);
					mLabelText.text = "Deleted ID: " + mSelectedMapId;
					OnListMapClick();
				} else {
					mLabelText.text = "Failed to delete ID: " + mSelectedMapId;
				}
			});
		}


		public void OnNewMapClick ()
		{
			if (!LibPlacenote.Instance.Initialized()) {
				Debug.Log ("SDK not yet initialized");
				return;
			}

			mInitButtonPanel.SetActive (false);
			mMappingButtonPanel.SetActive (true);

			LibPlacenote.Instance.StartSession ();
			Invoke ("PlaceOneOnion", 2f);
			Invoke ("SpawnDogs", 2f);
		}

		void SpawnDogs(){
			FindObjectOfType<Dogs> ().SpawnDogs();
		}

		private void StartARKit ()
		{
			mLabelText.text = "Initializing ARKit";
			Application.targetFrameRate = 60;
			ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration ();
			config.planeDetection = UnityARPlaneDetection.Horizontal;
			config.alignment = UnityARAlignment.UnityARAlignmentGravity;
			config.getPointCloudData = true;
			config.enableLightEstimation = true;
			mSession.RunWithConfig (config);
		}


		public void OnSaveMapClick ()
		{
			if (!LibPlacenote.Instance.Initialized()) {
				Debug.Log ("SDK not yet initialized");
				ToastManager.ShowToast ("SDK not yet initialized", 2f);
				return;
			}

			bool useLocation = Input.location.status == LocationServiceStatus.Running;
			LocationInfo locationInfo = Input.location.lastData;

			GameObject.Find ("DebugText").GetComponent<Text> ().text = "loc:" + useLocation + ", locinf:" + locationInfo.latitude;

			mLabelText.text = "Saving...";
			LibPlacenote.Instance.SaveMap (
				(mapId) => {
					LibPlacenote.Instance.StopSession ();
					mLabelText.text = "Saved Map ID: " + mapId;
					mInitButtonPanel.SetActive (true);
					mMappingButtonPanel.SetActive (false);

					LibPlacenote.MapMetadataSettable metadata = new LibPlacenote.MapMetadataSettable();

	//				JObject userdata = new JObject ();

	//				JObject shapeList = Shapes2JSON();
	//				userdata["shapeList"] = shapeList;
	//				JObject onionList = Onions2JSON();
	//				userdata["onionList"] = onionList;
					metadata.userdata = UserDataManager.localDataObject;
					metadata.name = RandomName.Get();

					if (useLocation) {
						metadata.location = new LibPlacenote.MapLocation();
						metadata.location.latitude = locationInfo.latitude;
						metadata.location.longitude = locationInfo.longitude;
						metadata.location.altitude = locationInfo.altitude;
					}
					LibPlacenote.Instance.SetMetadata (mapId, metadata);
					mCurrMapDetails = metadata;
				},
				(completed, faulted, percentage) => {}
			);
		}

		JObject testMet;
		public void TestSaveFunction(){

			testMet = new JObject ();
			JObject onionList = Onions2JSON();
			testMet["onionList"] = onionList;
	//		Debug.Log("saved! meta:"+testMet.ToString());
		}

		public void TestLoadFunction(){
			LoadOnionsJSON (testMet);
		}
			

		public void OnDropShapeClick ()
		{
			Vector3 shapePosition = Camera.main.transform.position + Camera.main.transform.forward * 0.3f;
			Quaternion shapeRotation = Camera.main.transform.rotation;

			System.Random rnd = new System.Random ();
			PrimitiveType type = (PrimitiveType) rnd.Next(0, 3);

			ShapeInfo shapeInfo = new ShapeInfo ();
			shapeInfo.px = shapePosition.x;
			shapeInfo.py = shapePosition.y;
			shapeInfo.pz = shapePosition.z;
			shapeInfo.qx = shapeRotation.x;
			shapeInfo.qy = shapeRotation.y;
			shapeInfo.qz = shapeRotation.z;
			shapeInfo.qw = shapeRotation.w;
			shapeInfo.shapeType = type.GetHashCode ();
			shapeInfoList.Add(shapeInfo);

			GameObject shape = ShapeFromInfo(shapeInfo);
			shapeObjList.Add(shape);

		}


		private GameObject ShapeFromInfo(ShapeInfo info)
		{
			GameObject shape = GameObject.CreatePrimitive ((PrimitiveType)info.shapeType);
			shape.transform.position = new Vector3(info.px, info.py, info.pz);
			shape.transform.rotation = new Quaternion(info.qx, info.qy, info.qz, info.qw);
			shape.transform.localScale = new Vector3 (0.05f, 0.05f, 0.05f);

			return shape;
		}


		private void ClearShapes () {
			foreach (var obj in shapeObjList) {
				Destroy (obj);
			}
			shapeObjList.Clear ();
			shapeInfoList.Clear ();
		}

		private JObject Shapes2JSON ()
		{
			ShapeList shapeList = new ShapeList ();
			shapeList.shapes = new ShapeInfo[shapeInfoList.Count];
			for (int i = 0; i < shapeInfoList.Count; i++) {
				shapeList.shapes [i] = shapeInfoList [i];
			}

			return JObject.FromObject (shapeList);
		}


		private JObject Onions2JSON ()
		{
			OnionList onionList = new OnionList ();

			List<MetalOnion> mos = new List<MetalOnion> ();
			foreach (MetalOnion m in FindObjectsOfType<MetalOnion>()) {
	//			Debug.Log ("added m!");
				mos.Add (m);

			}


			onionList.onions = new OnionInfo[mos.Count];
			for (int i = 0; i < onionList.onions.Length; i++) {
				onionList.onions [i] = new OnionInfo ();
				onionList.onions [i].onionState = mos [i].state;

				onionList.onions[i].px = mos [i].transform.position.x;
				onionList.onions[i].py = mos [i].transform.position.y;
				onionList.onions[i].pz = mos [i].transform.position.z;

				onionList.onions[i].qw = mos [i].transform.rotation.w;
				onionList.onions[i].qx = mos [i].transform.rotation.x;
				onionList.onions[i].qy = mos [i].transform.rotation.y;
				onionList.onions [i].qz = mos [i].transform.rotation.z;
				
			}
			return JObject.FromObject (onionList);
		}

		private void LoadOnionsJSON (JToken mapMetadata)
		{
			string d = "";
			d += mapMetadata.ToString ();
			

			if (mapMetadata is JObject && mapMetadata ["onionList"] is JObject) {
				OnionList onionList = mapMetadata ["onionList"].ToObject<OnionList> ();
				d += " .. onionlist exist";

				if (onionList.onions == null) {
					d += " .. onions was null";
					Debug.Log ("no onions dropped");
					ToastManager.ShowToast ("no oniones!",1f);
					return;
				}
				d += " .. onions Not null! ..";

				foreach (var onionInfo in onionList.onions) {
					d += " .. Placing an onion!";
					GameObject onion = (GameObject)Instantiate (onionPrefab, onionInfo.position, onionInfo.rotation);
					onion.GetComponent<MetalOnion> ().SetState (onionInfo.onionState);
				}
			}
	//		Debug.Log (d);
		}



		private void LoadShapesJSON (JToken mapMetadata)
		{
			ClearShapes ();

			if (mapMetadata is JObject && mapMetadata ["shapeList"] is JObject) {
				ShapeList shapeList = mapMetadata ["shapeList"].ToObject<ShapeList> ();
				if (shapeList.shapes == null) {
					Debug.Log ("no shapes dropped");
					return;
				}

				foreach (var shapeInfo in shapeList.shapes) {
					shapeInfoList.Add (shapeInfo);
					GameObject shape = ShapeFromInfo(shapeInfo);
					shapeObjList.Add(shape);
				}
			}
		}


		public void OnPose (Matrix4x4 outputPose, Matrix4x4 arkitPose) {}


		public JToken currentMapData;

		public void OnStatusChange (LibPlacenote.MappingStatus prevStatus, LibPlacenote.MappingStatus currStatus)
		{
			Debug.Log ("prevStatus: " + prevStatus.ToString() + " currStatus: " + currStatus.ToString());
			if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.LOST) {
				mLabelText.text = "Localized";
				localized = true;
				currentMapData = mSelectedMapInfo.metadata.userdata;
				LoadShapesJSON (mSelectedMapInfo.metadata.userdata);
				LoadOnionsJSON (mSelectedMapInfo.metadata.userdata);

				// Should already be an object loaded ..
	//			UserDataManager.LocalData = mSelectedMapInfo.metadata.userdata ["CoffeeCommandData"].ToObject<JObject>();


	//			StringTest tests = mSelectedMapInfo.userData ["test"].ToObject<StringTest> ();
	//			DebugText.Overflow (tests.stringtests[0] + "," + tests.stringtests[1]);
	//			if (mapMetadata is JObject && mapMetadata ["shapeList"] is JObject) {
	//				ShapeList shapeList = mapMetadata ["shapeList"].ToObject<ShapeList> ();

			} else if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.WAITING) {
				mLabelText.text = "Mapping";
			} else if (currStatus == LibPlacenote.MappingStatus.LOST) {
				mLabelText.text = "Searching for position lock";
			} else if (currStatus == LibPlacenote.MappingStatus.WAITING) {
				if (shapeObjList.Count != 0) {
					ClearShapes ();
				}
			}
		}
	}
*
*/
}