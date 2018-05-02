using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// Class that constructs a pointcloud mesh from the map retrieved from a LibPlacenote mapping/localization session
/// </summary>
public class CoffeeCommandFeaturesVisualizer : MonoBehaviour, PlacenoteListener
{
	private static CoffeeCommandFeaturesVisualizer sInstance;
	[SerializeField] Material mPtCloudMat;
	[SerializeField] GameObject mMap;

	void Awake ()
	{
		sInstance = this;

		// This is required for OnPose and OnStatusChange to be triggered
		LibPlacenote.Instance.RegisterListener (this);
	}

	void Update ()
	{
	}


	Vector3[] FakeGreenPoints {
		get {
			if (fakeGreenPointT == null || fakeGreenPointT.Count == 0) {
				InitFakeGreenPoints ();
			}
			return fakeGreenPointT.Select(t => t.position).ToArray();
		}
	}
	List<Transform> fakeGreenPointT;



	void InitFakeGreenPoints(){
		fakeGreenPointT = new List<Transform> ();
		Vector3 p0 = FindObjectOfType<MetalOnion> ().transform.position;
		for (int i=0;i<50;i++){
			GameObject gp = GameObject.CreatePrimitive (PrimitiveType.Cube);
			Vector3 p =  p0 + Camera.main.transform.right * 2 + Random.insideUnitSphere * 1.5f;
			gp.transform.position = p;
			gp.GetComponent<MeshRenderer> ().material.color = Color.green;
			gp.transform.localScale *= .05f;
			fakeGreenPointT.Add (gp.transform);
			gp.transform.SetParent (transform);
			gp.name = "GreenPoint " + i;
		}
		for (int i = 50; i < 100; i++) {
			GameObject gp = GameObject.CreatePrimitive (PrimitiveType.Cube);
			Vector3 p =  p0 + Camera.main.transform.right * 2.5f + Random.insideUnitSphere * 0.5f;
			gp.transform.position = p;
			gp.GetComponent<MeshRenderer> ().material.color = Color.green;
			gp.transform.localScale *= .05f;
			fakeGreenPointT.Add (gp.transform);
			gp.transform.SetParent (transform);
			gp.name = "GreenPoint " + i;
		}
		for (int i = 100; i < 250; i++) {
			GameObject gp = GameObject.CreatePrimitive (PrimitiveType.Cube);
			Vector3 v = Random.insideUnitSphere;
			v = new Vector3 (v.x, v.y / 5f, v.z);
			Vector3 p =  p0 + Camera.main.transform.forward * 2.5f + v * 2f;
			gp.transform.position = p;
			gp.GetComponent<MeshRenderer> ().material.color = Color.green;
			gp.transform.localScale *= .05f;
			fakeGreenPointT.Add (gp.transform);
			gp.transform.SetParent (transform);
			gp.name = "GreenPoint " + i;
		}
	}

	List<Vector3> currentGreenPoints = new List<Vector3>();
	public Vector3[] CurrentGreenPoints {
		get {
			#if UNITY_EDITOR
			return FakeGreenPoints;
			#endif
			return currentGreenPoints.ToArray();
		}
	}

	/// <summary>
	/// Enable rendering of pointclouds collected from LibPlacenote for every half second
	/// </summary>
	/// <remarks>
	/// NOTE: to avoid the static instance being null, please call this in Start() function in your MonoBehaviour
	/// </remarks>
	public static void EnablePointcloud ()
	{
		if (sInstance.mMap == null) {
			Debug.LogWarning (
				"Map game object reference is null, please initialize in editor. Skipping pointcloud visualization"
			);
			return;
		}
		sInstance.InvokeRepeating ("DrawMap", 0f, 0.5f);
	}

	/// <summary>
	/// Disable rendering of pointclouds collected from LibPlacenote
	/// </summary>
	public static void DisablePointcloud ()
	{
		sInstance.CancelInvoke ();
	}

	public void OnPose (Matrix4x4 outputPose, Matrix4x4 arkitPose)
	{
	}

	public void OnStatusChange (LibPlacenote.MappingStatus prevStatus, LibPlacenote.MappingStatus currStatus)
	{
		if (currStatus == LibPlacenote.MappingStatus.WAITING) {
			Debug.Log ("Session stopped, resetting pointcloud mesh.");
			MeshFilter mf = mMap.GetComponent<MeshFilter> ();
			mf.mesh.Clear ();
		}
	}

	public void DrawMap ()
	{
		currentGreenPoints.Clear ();
		if (LibPlacenote.Instance.GetStatus () != LibPlacenote.MappingStatus.RUNNING) {
			return;
		}

		LibPlacenote.PNFeaturePointUnity[] map = LibPlacenote.Instance.GetMap ();
		if (map == null) {
			return;
		}

		Vector3[] points = new Vector3[map.Length];
		Color[] colors = new Color[map.Length];

		int totBrown = 0;
		int totGreen = 0;

		for (int i = 0; i < map.Length; ++i) {

			points [i].x = map [i].point.x;
			points [i].y = map [i].point.y;
			points [i].z = -map [i].point.z;
			colors [i].r = 1 - map [i].measCount / 10f;
			colors [i].b = 0;
			colors [i].g = map [i].measCount / 10f;

			if (map [i].measCount > 6) {
				currentGreenPoints.Add (new Vector3(map [i].point.x,map [i].point.y,map [i].point.z));
				totGreen++;
			} else {
				totBrown++;
			}

			if (map [i].measCount < 4) {
				colors [i].a = 0f;
			} else {
				colors [i].a = 0.2f + 0.8f * (map [i].measCount / 10f);
			}
		}

		DebugText.SetGreenDots (totGreen.ToString ());
		DebugText.SetBrownDots (totBrown.ToString ());

		// Need to update indicies too!
		int[] indices = new int[map.Length];
		for (int i = 0; i < map.Length; ++i) {
			indices [i] = i;
		}

		// Create GameObject container with mesh components for the loaded mesh.
		Mesh mesh = new Mesh ();
		mesh.vertices = points;
		mesh.colors = colors;
		mesh.SetIndices (indices, MeshTopology.Points, 0);

		MeshFilter mf = mMap.GetComponent<MeshFilter> ();
		if (mf == null) {
			mf = mMap.AddComponent<MeshFilter> ();
		} 
		mf.mesh = mesh;

		MeshRenderer mr = mMap.GetComponent<MeshRenderer> ();
		if (mr == null) {
			mr = mMap.AddComponent<MeshRenderer> ();
		} 

		mr.material = mPtCloudMat;
	}
}
