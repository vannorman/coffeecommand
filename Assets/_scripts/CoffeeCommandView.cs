﻿
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

		// TODO: Refactor state changes instead of activating/dactivagint buttons and setting user visible text in random functions

		public enum State {
			Inactive,
			Mapping,
			Uploading,
			FinishedUploading,
			Haresting,
			FinishedHarvesting,
			Restarting

		}
		public State state = State.Inactive;

		// SetState on each state change (not implemented yet -- July 9 2018


		public static CoffeeCommandView inst;
		public GameObject restartButton;
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
		public LibPlacenote.MapMetadataSettable mCurrMapDetails;

		private bool mReportDebug = false;

		public LibPlacenote.MapInfo mSelectedMapInfo;
		public string mSelectedMapId {
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
			Invoke ("PlaceOneOnion", 3f);
//			Invoke ("SpawnDogs", 2f);
			Invoke ("SpawnDogs", 3f);
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


		public void SaveMapNow (Action cbFunc)
		{
			if (!LibPlacenote.Instance.Initialized()) {
				Debug.Log ("SDK not yet initialized");
				ToastManager.ShowToast ("SDK not yet initialized", 2f);
				return;
			}
//			CLogger.Log ("Save: 0 begin");

			bool useLocation = Input.location.status == LocationServiceStatus.Running;
			LocationInfo locationInfo = Input.location.lastData;


			// Case 0: This is the first map in this location (user did not load a previous map)
			// Save the map normally.
			// Case 1: This is a map loaded from a previous save. 
			// Update that save with new metadata 
			// Save an "invisible" map to keep the mesh


			mLabelText.text = "Wormhole started .. ";

			LibPlacenote.Instance.SaveMap (
				(mapId) => {
					
					LibPlacenote.Instance.StopSession ();
					mSaveMapId = mapId;
					CoffeeCommandFeaturesVisualizer.inst.DisablePointcloud();
					mPNPlaneManager.ClearPlanes ();


					if (UserDataManager.loadedExistingMap){
//						CLogger.Log ("Save: 5a1 .. was existed");



						// Go ahead and upload this map but make it invisible
						UserDataManager.CoffeeCommandObject invisibleData = UserDataManager.InitCoffeeCommandObject("invisible");
						invisibleData.invisible = true; // needlessly complex obhect .. should just be a json with a bool
						LibPlacenote.MapMetadataSettable metadataInvisible = new LibPlacenote.MapMetadataSettable();
						metadataInvisible.userdata = JObject.FromObject(invisibleData);
						metadataInvisible.name = "Invisible map at "+System.DateTime.Now;

						if (useLocation) {
//							mLabelText.text = "location!";
							metadataInvisible.location = new LibPlacenote.MapLocation();
							metadataInvisible.location.latitude = locationInfo.latitude;
							metadataInvisible.location.longitude = locationInfo.longitude;
							metadataInvisible.location.altitude = locationInfo.altitude;
						} else {
							Debug.Log("no loc");
						}
//						CLogger.Log("Save: 5a2 new invisible map with id:"+mapId);
						LibPlacenote.Instance.SetMetadata (mSaveMapId, metadataInvisible); // this new map is now "invisible"

						// Now, update the former map
						LibPlacenote.Instance.GetMetadata(UserDataManager.loadedMapPlaceId, (metaCb)=> {

							LibPlacenote.MapMetadataSettable replacementMetadata = new LibPlacenote.MapMetadataSettable();
							replacementMetadata.location = metaCb.location;
							replacementMetadata.name = metaCb.name;
//							replacementMetadata.
//							replacementMetadata.userdata = metaCb.location
							
							replacementMetadata.userdata = JObject.FromObject(UserDataManager.LocalData);
							replacementMetadata.name = UserDataManager.LocalData.mapName;

							
							// Also, modify the map ID we loaded earlier
							LibPlacenote.Instance.SetMetadata (UserDataManager.loadedMapPlaceId, replacementMetadata);
//							UserDataManager.lastMetaUpdateTime = 
//							CLogger.Log("Save: 5a3 overwrite metadata with id:"+UserDataManager.loadedMapPlaceId);
						});


					} else {
						// Brand new map
						LibPlacenote.MapMetadataSettable metadata = new LibPlacenote.MapMetadataSettable();
						metadata.name = RandomName.Get ();
						mLabelText.text = "Wormhole ripped, establishing future wormhole link ..: " + metadata.name;
						if (useLocation) {
							

							metadata.location = new LibPlacenote.MapLocation();
							metadata.location.latitude = locationInfo.latitude;
							metadata.location.longitude = locationInfo.longitude;
							metadata.location.altitude = locationInfo.altitude;
						} else {
							CLogger.Log("no location?");
						}
						UserDataManager.loadedMapPlaceId = mapId;

						metadata.userdata = JObject.FromObject(UserDataManager.LocalData);
//						mCurrMapDetails = metadata;
						LibPlacenote.Instance.SetMetadata (mapId, metadata);
						CLogger.Log("saved new map w location:"+metadata.location.latitude+","+metadata.location.longitude+".");

					}
//					ToastManager.ShowToast("Save complete, wait for upload");

				},
				(completed, faulted, percentage) => {
					if (completed) {
						cbFunc();

						if (UserDataManager.loadedExistingMap){
							mLabelText.text = "You took over this wormhole! You can now harvest MeshCoin from the future.";// + mCurrMapDetails.name;
//							ToastManager.ShowToast("Success! You took over this mine.");
						} else {
							mLabelText.text = "You established a wormhole! You can now harvest MeshCoin from the future.";// + mCurrMapDetails.name;
						}
						harvestCoinsButton.SetActive (true);
					}
					else if (faulted) {
						mLabelText.text = "Upload faulted." ; //+ mCurrMapDetails.name + "faulted";
					}
					else {
						mLabelText.text = "Establishing wormhole to the future!" + "(" + percentage.ToString("F2") + "/1.0)";
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
//				UserDataManager.SetLocalData
//				LoadShapesJSON (mSelectedMapInfo.metadata.userdata);
			} else if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.WAITING) {
				mLabelText.text = "Mapping";
			} else if (currStatus == LibPlacenote.MappingStatus.LOST) {
//				mLabelText.text = "Searching for position lock";
			} else if (currStatus == LibPlacenote.MappingStatus.WAITING) {
				if (shapeObjList.Count != 0) {
//					ClearShapes ();
				}
			}
		}

		int currentCoinsToHarvest  = -1;

		public GameObject harvestCoinsButton;
		public void HarvestCoinsNow(){
			harvestCoinsButton.SetActive (false);
			mLabelText.text = "Harvesting - PLEASE WAIT";
			if (!UserDataManager.loadedExistingMap) {
				// New map gives the default num of coins
				int ct = UserDataManager.LocalData.mine.coins.count;
				if (ct < 5) {
					mLabelText.text = "This wormhole mine doesn't have enough coins to harvest now. Come back later.";
					restartButton.SetActive (true);
//					ToastManager.ShowToast ("There aren't enough coins to harvest. Come back tomorrow.");
					return;
				}
				UserDataManager.LocalCoins += ct;
				UserDataManager.LocalData.mine.coins.count = 0;
				mLabelText.text = "You harvested "+ct+" MeshCoin from the future!";

			} else {

				UserDataManager.CollectCurrentMineCoins ( (ct) => {
					if (ct < 5){
						mLabelText.text = "This wormhole mine doesn't have enough coins to harvest now. Come back later.";
						restartButton.SetActive (true);
//						ToastManager.ShowToast ("There aren't enough coins to harvest. Come back later.");
						return;
					}
//					CLogger.Log("will try to collect "+ct+ " coins");
					int count = ct;
					currentCoinsToHarvest = ct;
					StartCoroutine("TryCollectCoins",count);
//					StartCoroutine(TryCollectCoins,ct);
					mLabelText.text = "You are harvesting "+ct+" MeshCoin from this wormhole. Please wait!";
//					ToastManager.ShowToast ("You conquered this mine and are collecting " + ct + " coins!");
				});

				// old map needs to calc the coins
			}
		}

		IEnumerator TryCollectCoins(int count){
//			CLogger.Log ("Try collect at:" + Math.Round (Time.time));
			bool collected = false;
			int tries = 20;
			while (collected == false && tries > 0) {
				CLogger.Log ("Trying to collect! tries left:" + tries);
				LibPlacenote.Instance.GetMetadata (UserDataManager.loadedMapPlaceId, (metadata) => {
					if (!collected){

						int secondsDelta = Mathf.RoundToInt((float)(UserDataManager.lastMetaUpdateTime - metadata.userdata.ToObject<UserDataManager.CoffeeCommandObject> ().lastUpdatedTime).TotalSeconds);
						if (secondsDelta > 1) {
							CLogger.Log ("Collect failed, delta:" + secondsDelta);		
						} else {

							collected = true;
							restartButton.SetActive(true);
							mLabelText.text = "You harvested "+count+" MeshCoin from the future!";
							UserDataManager.LocalCoins += count;

						
						}
					}
				});
				tries --;
				yield return new WaitForSeconds (2);

			}
			yield break;
		}
	}


}