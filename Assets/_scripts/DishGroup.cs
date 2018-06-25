using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CoffeeCommand {
	public class DishGroup : MonoBehaviour {

		enum State {
			Alive,
			Folding,
			Retracting,
			Dead
		}
		State state = State.Alive;

		MetalOnion metalOnion;
		public Transform dishesParent;
		public Transform dishGroupCylinders;
		public Transform dishGroupCylindersDownPosition;
		public Transform masterVerticalCylinder;
		// Use this for initialization
		void Start () {
			if (!metalOnion) {
				metalOnion = transform.root.GetComponentInChildren<MetalOnion> ();
			}
		}

		void SetState(State newState){
			state = newState;
			switch (state) {
			case State.Dead:
				metalOnion.OnDishGroupDestroyed ();
				break;
			default:
				break;
			}
		}
		// Update is called once per frame
		float aliveCheckTimer = 0;
		void Update () {
			switch (state) {
			case State.Alive:
				aliveCheckTimer -= Time.deltaTime;
				if (aliveCheckTimer < 0) {
					aliveCheckTimer = Random.Range (0.2f, 0.3f);
					if (AllDishesDestroyed) {
						SetState (State.Folding);
					}
				}

				break;
			case State.Folding:
				float foldSpeed = 12f;
				bool finishedFolding = false;
				foreach (Transform t in dishGroupCylinders) {
					float toNeg90 = Mathf.Lerp (t.localRotation.eulerAngles.x, -90, Time.deltaTime * 0.2f);
	//				Debug.Log ("toneg:" + toNeg90);
					t.localRotation = Quaternion.Euler (toNeg90, t.localRotation.eulerAngles.y, 0);//  = Quaternion.RotateTowards (t.rotation, masterVerticalCylinder.rotation, Time.deltaTime * foldSpeed);
	//				if (Vector3.Magnitude (t.rotation.eulerAngles - masterVerticalCylinder.rotation.eulerAngles) < 5) {
	//					finishedFolding = true;
	//				}
					if (Mathf.Abs(t.localRotation.eulerAngles.x%360-270) < 1){
						finishedFolding = true;
					}
				
				}
				if (finishedFolding) {
					foreach (Transform t in dishGroupCylinders) {
						t.rotation = Quaternion.Euler (-90, 0, 0); //masterVerticalCylinder.rotation;
					}
					SetState (State.Retracting);
				}
				break;
			case State.Retracting:
				float retractSpeed = .1f;
				dishGroupCylinders.position = Vector3.MoveTowards (dishGroupCylinders.position, dishGroupCylindersDownPosition.position, Time.deltaTime * retractSpeed);
				masterVerticalCylinder.position = Vector3.MoveTowards (masterVerticalCylinder.position, dishGroupCylindersDownPosition.position, Time.deltaTime * retractSpeed);
				float d = (dishGroupCylinders.position - dishGroupCylindersDownPosition.position).magnitude;
				if (d < .05f) {
					SetState (State.Dead);

				}
				break;
			default:break;
			}
		
		}


		bool AllDishesDestroyed {
			get {
				return 
					dishesParent.childCount == 0;
			}
		}
	}
}