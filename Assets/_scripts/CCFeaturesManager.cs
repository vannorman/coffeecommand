using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCFeaturesManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	class CurrentFeature {
		public Vector3 point;
		public Color color;
	}

	List<CurrentFeature> currentFeatures = new List<CurrentFeature>();

	void GetCurrentFeatures(){
		currentFeatures = new List<CurrentFeature> ();
		Vector3[] verts = CC.featuresVisualizer.GetComponent<MeshFilter> ().mesh.vertices;
		for(int i=0;i<verts.Length; i++){
			CurrentFeature cf = new CurrentFeature ();
			cf.point = CC.featuresVisualizer.transform.TransformPoint (verts[i]);
			cf.color = CC.featuresVisualizer.GetComponent<MeshFilter> ().mesh.colors [i];
			currentFeatures.Add (cf);		
		}
	}


	// Total points in scene at present time (since last cached value)
	public Vector3[] MapPoints {
		get { 
			List<Vector3> pts = new List<Vector3> ();
			foreach (CurrentFeature cf in currentFeatures) {
				pts.Add (cf.point);
			}
			return pts.ToArray ();
		}
	}


	
	// Update is called once per frame
	float featureUpdateTimer  = 0f;

	void Update () {
		return;
		featureUpdateTimer -= Time.deltaTime;
		if (featureUpdateTimer < 0) {
			featureUpdateTimer = Random.Range (0.3f, 0.7f);
			GetCurrentFeatures ();

			foreach (CurrentFeature cf in currentFeatures) {
				int numGreenDotsVisible = 0;
				int numBrownDotsVisible = 0;
				if (Utils2.PointVisibleToCamera(cf.point,Camera.main)){
					// green
					if (cf.color.g > 0.6f) {
						numGreenDotsVisible++;
					} else {
						numBrownDotsVisible++;
					}
				}
				DebugText.SetBrownDots (numBrownDotsVisible.ToString ());
				DebugText.SetGreenDots (numGreenDotsVisible.ToString ());
				FindObjectOfType<BatteryUpload> ().SetFillAmount ((float)numGreenDotsVisible / 50f);
			}

			
		}


	}
}
