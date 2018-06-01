using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OnionLocationHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		InitQuadrantScores ();	
	}
	
	// Update is called once per frame
	void Update () {
		targetFeatureClusterTimer -= Time.deltaTime;
		if (Input.GetKey (KeyCode.C)) {
			for (int i = 0; i < 100; i++) {
			
				GameObject o = GameObject.Find ("debSphere");
				if (o) {
					Destroy (o);
				}
			}
		}
	}




	public bool FeatureClusterNearby(MetalOnion mo){
		// Iterate through all Map[] points and see if enough green ones are close to mo.tran.pos
		// save/cache (or dump) the current target feature position
		return false;
	}

	Dictionary<Vector3,float> QuadrantScores = new Dictionary<Vector3,float>();
//	void InitQuadrantScores(){
//		QuadrantScores.Clear ();
//		for (int i=-1;i<2;i++){
//			for (int j = -1; j < 2; j++) {
//				for (int k = -1; k < 2; k++) {
////					Debug.Log ("ijk:" + i + "," + j + "," + k);
//					QuadrantScores.Add (new Vector3 (i, j, k), 0);
//				}
//			}
//		}
//
//	}

	float targetFeatureClusterInterval = 1f;
	float targetFeatureClusterTimer = 0f;
	Vector3 cachedTarget;
	public bool foundAnyQuadrant = false;
	public Vector3 TargetFeatureCluster (){
		return cachedTarget;
	}
	public float searchRadius = 8f;
	MetalOnion lastOnionWhoRequested;
	public bool FoundTargetNearOnion(MetalOnion mo) {
		if (targetFeatureClusterTimer < 0) {
			foundAnyQuadrant = false;
			if (lastOnionWhoRequested != mo) {
				searchRadius = 8f;
			}
			lastOnionWhoRequested = mo;
			targetFeatureClusterTimer = targetFeatureClusterInterval;
//			InitQuadrantScores ();
			QuadrantScores.Clear();
			// Let's make 27 quadrants relative to onion's world position
			// Check each quadrant for total green features
			// The quadrant with the most green features is where the onion moves.
			
			// iterate through all Green map points
			Vector3[] greenPoints = CC.featuresVisualizer.CurrentGreenPoints;
			DebugText.SeekPlanes("greenpts:"+greenPoints.Length);
			foreach (Vector3 gp in greenPoints) {
				
				for (int i=-1;i<2;i++){
					for (int j = -1; j < 2; j++) {
						for (int k = -1; k < 2; k++) {
							// For each point compare its distance to one of 27 quadrants surrounding the onion
							// Closer quadrants are scored more highly
							// Result should be that the quadrant with the most green dots gets the highest score.
							float distFromThisQuadrantToGreenPoint = Vector3.SqrMagnitude (mo.transform.position + new Vector3 (i, j, k) * searchRadius - gp);
							Vector3 key = new Vector3 (i, j, k);
							float val = Mathf.Min (1, 1 / distFromThisQuadrantToGreenPoint);
//							Debug.Log ("val for " + i + "," + j + "," + k + ": " + val);
							if (QuadrantScores.ContainsKey (key)) {
								QuadrantScores [key] += val; // never more than 1
							} else {
								QuadrantScores.Add (key, val);
							}
						}
					}
				}
			}
//
//			foreach (Vector3 k in QuadrantScores.Keys) {
//				GameObject d = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//				d.transform.position = k;
//				d.transform.localScale = .002f * QuadrantScores [k] * Vector3.one;
//				d.transform.position = mo.transform.position + k * searchRadius;
//				d.name = "debSphere";
////				d.GetComponent<Renderer>().material.color = new Color(
//			}

			
//			float max = 0;
			Vector3 bestQuadrant = GetBestQuadrant ();

			if (foundAnyQuadrant) {
//				Debug.Log ("set cached target;" + mo.transform.position + " plus <color=green>" + bestQuadrant + "</color>");
				cachedTarget = mo.transform.position + bestQuadrant;
				// If the best quadrant is zero, shrink the radius
				if (bestQuadrant == Vector3.zero) {
					searchRadius /= 2f;
//					Debug.Log("<color=blue>Decereased by half. New radius:"+searchRadius+"</color>");
				}

			} else {
				Debug.Log ("Found no quads");
			}
//			Debug.Log("<color=blue>Cached :"+cachedTarget+"</color>");
			
		}
		return foundAnyQuadrant;


	}


	Vector3 GetBestQuadrant(){
		float max = 0;
		Vector3 bestQuadrant = Vector3.zero;
		foreach (KeyValuePair<Vector3,float> kvp in QuadrantScores) {
			// Determine the highest scored quadrant
			if (kvp.Value > max) {
				//					Debug.Log ("val of " + kvp.Key + ":" + kvp.Value+" vs <color=red>max:</color>"+max);
				max = kvp.Value;
				bestQuadrant = kvp.Key;
				foundAnyQuadrant = true;
			}	
		}
		return bestQuadrant;
	}



	float planeSeekTimer = 0f;
	float nearestPlaneSeekInterval = 0.3f;
//	Dictionary<Transform,GameObject> cachedPlanes = new Dictionary<Transform, GameObject>();
	public GameObject GetNearestPlane(Transform nearObj, float r){
//		if (!cachedPlanes.ContainsKey (nearObj)) {
//			cachedPlanes.Add (nearObj,null);
//		}

		PlaneInfo[] planes = FindObjectsOfType<PlaneInfo> ();
		Dictionary<PlaneInfo,float> scoredPlanes = new Dictionary<PlaneInfo, float> ();
		for (int i = 0; i < planes.Length; i++) {
			float score = 0;
			Vector3[] pts = CC.featuresVisualizer.CurrentGreenPoints;
			for (int j = 0; j < pts.Length; j++) {
				score += Mathf.Min (1, 1 / (pts [j] - planes [i].transform.position).magnitude);
			}
			scoredPlanes.Add (planes [i], score);
		}


//		PlaneInfo max = null;
		PlaneInfo[] maxArr = (from x in scoredPlanes where x.Value == scoredPlanes.Max(v => v.Value) select x.Key).ToArray();



		if (maxArr.Length == 0) {
			// in case no planes are found, just return the center of the previously discovered "best" quadrant.
			GameObject fake = new GameObject ();
			fake.transform.position = cachedTarget; // nearObj.transform.position + GetBestQuadrant ();
			return fake;
		} else {
			return maxArr[0].gameObject;
		}

//
//		planeSeekTimer -= Time.deltaTime;
//		if (planeSeekTimer < 0) {
//
//			GameObject nearest = null;
//			planeSeekTimer = nearestPlaneSeekInterval;
//			float nearDist = Mathf.Infinity;
//			int i = 0;
//			foreach (PlaneInfo pi in FindObjectsOfType<PlaneInfo>()) {
//				float curDist = Vector3.Magnitude (pi.gameObject.transform.position - nearObj.position);
//
//				if (curDist < nearDist && curDist < r && pi.plantedOnion == null) {
//					curDist = nearDist;
//					nearest = pi.gameObject;
//
//
//				}
//				i++;
//			}
////			cachedPlanes[nearObj] = nearest;
//
//			return nearest;
//		} else {
//
////			return cachedPlanes[nearObj];
//		}
	}


}
