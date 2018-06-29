using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.iOS;
namespace CoffeeCommand {

	public class DetectMapLocationClick : MonoBehaviour {



		public GameObject gameGroup;
		public GameObject mapGroup;

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
						foreach (MapMarkerInfo mmi2 in FindObjectsOfType<MapMarkerInfo>()) {
							if (mmi != mmi2) {
								mmi2.fx.SetActive (false);
							}
							else {
								mmi2.fx.SetActive (true);
							}
						}

						loadButton.SetActive (true);
						createButton.SetActive (false);
						CLogger.Log ("set map to load id;" + mmi.mapInfo.placeId);
						PlaceSelectionManager.inst.SetMapToLoad(mmi.mapInfo);


					}
				}

			}
		}




	}

}