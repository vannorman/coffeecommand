using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.iOS;
namespace CoffeeCommand {

	public class DetectMapLocationClick : MonoBehaviour {




		public CoffeeCommandView coffeeCommandView;
	//	UnityEngine.UI.Text db;
		public GameObject loadButton;
		public GameObject createButton;

		float timeout = 0;
		void Update () {
			timeout -= Time.deltaTime;
			DetectMapSelection ();
		}



		void DetectMapSelection() {
			if (Input.touchCount > 0) {
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
								if (mmi.outOfRange) {
									CLogger.Log ("mmi2 out of range 1:" + mmi2.name);
									mmi2.outOfRangeFx.SetActive (true);
									CLogger.Log ("this marker out of range.");
								} else {
									CLogger.Log ("mmi2 not out of range 1:" + mmi2.name);
									PlaceSelectionManager.inst.SetMapToLoad(mmi.mapInfo);
									mmi2.fx.SetActive (true);
									loadButton.SetActive (true);

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