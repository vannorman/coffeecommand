using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.iOS;
namespace CoffeeCommand {

	public class DetectMapLocationClick : MonoBehaviour {




		public CoffeeCommandView coffeeCommandView;
	

		public GameObject createButton;

		float timeout = 0;
		void Update () {
			timeout -= Time.deltaTime;
			DetectMapSelection ();
		}



		void DetectMapSelection() {
			if (Input.touchCount > 0) {
//				CLogger.Log ("touches: " + Input.touchCount);
				Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch(0).position);
				foreach (RaycastHit hit in Physics.RaycastAll(ray)) {
	//				pnt += "hit:" + hit.collider.name + "\n";
					MapMarkerInfo mmi = hit.collider.GetComponent<MapMarkerInfo> ();
					if (mmi && timeout < 0) {
						timeout = 0.4f;

						CLogger.Log ("hit:" + hit.collider.name);

						foreach (MapMarkerInfo mmi2 in FindObjectsOfType<MapMarkerInfo>()) {
							CLogger.Log ("mmi2:" + mmi2.name);
							if (mmi != mmi2) {
								CLogger.Log ("mmi2 setF 1:" + mmi2.name);
								mmi2.fx.SetActive (false);
								mmi2.outOfRangeFx.SetActive (false);
								CLogger.Log ("mmi2 setF 2:" + mmi2.name);
							}

							else {
								CLogger.Log ("mmi2 setT 1:" + mmi2.name);
								switch (mmi.status) {
								case MapMarkerInfo.Status.OutOfRange:
									CLogger.Log ("mmi2 out of range 1:" + mmi2.name);
									mmi2.outOfRangeFx.SetActive (true);
									CLogger.Log ("this marker out of range.");
									break;
								case MapMarkerInfo.Status.Selectable:
									CLogger.Log ("mmi2 not out of range 1:" + mmi2.name);
									PlaceSelectionManager.inst.SetMapToLoad(mmi.mapInfo);
									mmi2.fx.SetActive (true);
									PlaceSelectionManager.inst.loadButton.SetActive (true);
									break;


								}
							}
						}
//						CLogger.Log ("set map to load id;" + mmi.mapInfo.placeId);


					}
				}

			}
		}




	}

}