using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;
namespace CoffeeCommand {
	public class GameManager : MonoBehaviour {
	
		public static GameManager inst;
		public GameObject wrldParent;
		public GameObject gameParent;

		public enum State {
			WrldMap,
			Game
		}
		public State state = State.WrldMap;


		public void SetState(State newState){
//			CLogger.Log ("set state:" + newState);
//			Debug.Log ("newstate:" + newState);
			state = newState;
			switch (state) {
			case State.WrldMap:
				wrldParent.SetActive (true);
				gameParent.SetActive (false);
				break;
			case State.Game:
				wrldParent.SetActive (false);
				gameParent.SetActive (true);
				break;
				
			}
		}

		public void SetStateMap(){
			CLogger.Log ("Set state map");
			wrldParent.SetActive (true);
			SetState (State.WrldMap);
			StartCoroutine ("ListAfterSeconds", 0.1f);
		}
		IEnumerator ListAfterSeconds(float s){
			yield return new WaitForSeconds (s);
			PlaceSelectionManager.inst.ListNearbyPlaces ("restart");
		}

	}
}
