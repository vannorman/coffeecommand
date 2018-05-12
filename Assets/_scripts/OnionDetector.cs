using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnionDetector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
//		RaycastHit hit = new RaycastHit ();
		Ray ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0f));
		float radius = 0.05f;
		bool hitt = false;
		foreach (RaycastHit hit in Physics.SphereCastAll(ray,radius)) {
			hitt = true;
			MetalOnion mo = hit.collider.gameObject.GetComponent<MetalOnion> ();
			if (mo) {
				mo.CameraHovering ();
				//				Debug.Log ("Cam hit;" + mo.name);
				DebugText.SetCamHoverObj ("cam hit:" + hit.collider.name);
			}
			
			DamageReceiver dr = hit.collider.gameObject.GetComponent<DamageReceiver> ();
			if (dr || (mo && (mo.state != MetalOnion.State.Unwrapped)) ) {
				
				CC.crosshair.SetState (Crosshair.State.Destructible);
			
				
			} else {
				CC.crosshair.SetState(Crosshair.State.Nominal);
			}
			
		
		}
		if (!hitt) {
			CC.crosshair.SetState(Crosshair.State.Nominal);
		
		}

//			DebugText.SetCamHoverObj ("cam hit: no hit");
//		}
				
	}
}
