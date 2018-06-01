using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnionDetector : MonoBehaviour {
	
	float t = 0;
	void Update () {

		t -= Time.deltaTime;
		if (t < 0) {
			t = 0.1f;
			Ray ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0f));
			float radius = 0.05f;
			bool hitt = false;
			bool detectedOnion = false;

			foreach (RaycastHit hit in Physics.SphereCastAll(ray,radius)) {
				hitt = true;
				MetalOnion mo = hit.collider.gameObject.GetComponent<MetalOnion> ();
				if (mo) {
					detectedOnion = true;
					mo.CameraHovering (); // has a timeout, should last 1 second or so
					switch(mo.state){
					case MetalOnion.State.Unwrapping:
//						ItemPopup.inst.Show (hit.point, "Destroy this! " + mo.DishesRemainingInfo[0].ToString() + " of  "+mo.DishesRemainingInfo[1].ToString() +" dishes remaining." );
						ItemPopup.inst.Show (mo.transform.position, "Destroy this! " + mo.DishesRemainingInfo[0].ToString() + " of  "+mo.DishesRemainingInfo[1].ToString() +" dishes remaining." );
						break;
					default:
						ItemPopup.inst.Hide ();
						break;
					}
//					DebugText.SetCamHoverObj ("cam hit:" + hit.collider.name);
				}
				
				DamageReceiver dr = hit.collider.gameObject.GetComponent<DamageReceiver> ();
				if (dr || (mo && (mo.state != MetalOnion.State.Unwrapped)) ) {
					CC.crosshair.SetState (Crosshair.State.Destructible);
					if (detectedOnion)
						break; // don't need to check other colliders; we already determined Onion and DamageReceiver
					
					
				} else {
					CC.crosshair.SetState(Crosshair.State.Nominal);
				}
				
				
			}
			if (!hitt) {
				CC.crosshair.SetState(Crosshair.State.Nominal);
				
			}
		}

				
	}
}
