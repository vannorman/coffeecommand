﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.XR.iOS;
using System.Runtime.InteropServices;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

[System.Serializable]
public class ShapeInfo
{
	public float px;
	public float py;
	public float pz;
	public float qx;
	public float qy;
	public float qz;
	public float qw;
	public int shapeType;
}


[System.Serializable]
public class ShapeList
{
	public ShapeInfo[] shapes;
}

public class StringTest
{
	public string[] stringtests;
}

public class PlacenoteSampleView : MonoBehaviour, PlacenoteListener
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
	[SerializeField] Material mShapeMaterial;

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


	// Use this for initialization
	void Start ()
	{
		Input.location.Start ();

		mMapListPanel.SetActive (false);

		mSession = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
		UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
		StartARKit ();
		FeaturesVisualizer.EnablePointcloud ();
		LibPlacenote.Instance.RegisterListener (this);
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
			} else if (!mARKitInit) {
				mARKitInit = true;
				mLabelText.text = "ARKit Initialized";
			}

			Matrix4x4 matrix = mSession.GetCameraPose ();

			Vector3 arkitPosition = PNUtility.MatrixOps.GetPosition (matrix);
			Quaternion arkitQuat = PNUtility.MatrixOps.GetRotation (matrix);

			LibPlacenote.Instance.SendARFrame (mImage, arkitPosition, arkitQuat, mARCamera.videoParams.screenOrientation);
		}
	}


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
				if (mapId.userData != null) {
					Debug.LogError (mapId.userData.ToString (Formatting.None));
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


	void OnMapSelected (LibPlacenote.MapInfo mapInfo)
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

		mLabelText.text = "Loading Map ID: " + mSelectedMapId;
		LibPlacenote.Instance.LoadMap (mSelectedMapId,
			(completed, faulted, percentage) => {
				if (completed) {
					mMapSelectedPanel.SetActive (false);
					mMapListPanel.SetActive (false);
					mInitButtonPanel.SetActive (false);
					mExitButton.SetActive (true);

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

		mLabelText.text = "Saving...";
		LibPlacenote.Instance.SaveMap (
			(mapId) => {
				LibPlacenote.Instance.StopSession ();
				mLabelText.text = "Saved Map ID: " + mapId;
				mInitButtonPanel.SetActive (true);
				mMappingButtonPanel.SetActive (false);


				JObject metadata = new JObject ();

				JObject shapeList = Shapes2JSON();
				metadata["shapeList"] = shapeList;

				StringTest testData = new StringTest();
				testData.stringtests = new string[2];
				testData.stringtests[0] = "test1";
				testData.stringtests[1] = "test2";
				metadata["test"] = JObject.FromObject (testData);

				if (useLocation) {
					metadata["location"] = new JObject ();
					metadata["location"]["latitude"] = locationInfo.latitude;
					metadata["location"]["longitude"] = locationInfo.longitude;
					metadata["location"]["altitude"] = locationInfo.altitude;
				}
				LibPlacenote.Instance.SetMetadata (mapId, metadata);
			},
			(completed, faulted, percentage) => {}
		);
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
		shape.GetComponent<MeshRenderer> ().material = mShapeMaterial;

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


	public void OnStatusChange (LibPlacenote.MappingStatus prevStatus, LibPlacenote.MappingStatus currStatus)
	{
		Debug.Log ("prevStatus: " + prevStatus.ToString() + " currStatus: " + currStatus.ToString());
		if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.LOST) {
			mLabelText.text = "Localized";
			LoadShapesJSON (mSelectedMapInfo.userData);
			StringTest tests = mSelectedMapInfo.userData ["test"].ToObject<StringTest> ();
			DebugText.Overflow (tests.stringtests[0] + "," + tests.stringtests[1]);
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
