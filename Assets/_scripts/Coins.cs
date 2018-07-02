using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CoffeeCommand {
	public class Coins : MonoBehaviour {

		public int ct = 0;
		public int target = 0;
//		public void EarnCoin(int newCt){
//			target += newCt;
//
//		}
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		float t = 0;
		void Update () {
			GetComponent<Text> ().text = UserDataManager.LocalCoins.ToString ();
			return;
//			Debug.Log ("coins:" + UserDataManager.LocalCoins);
//			target = UserDataManager.LocalCoins;
//
//			if (ct != target) {
//				t -= Time.deltaTime;
//				float lerpDuration = 1.5f;
//				float interval = (target - ct) / lerpDuration;
//				if (t < interval) {
//					t = 0;
//					int increaseAmount = 1; //Mathf.Max(1,Mathf.RoundToInt(Mathf.Pow(Mathf.Abs(ct - target),0.7f)));
//
//					ct = Mathf.Min (ct + increaseAmount, target);
//				}
//				
//			}

				
//			GetComponent<Text> ().text = UserDataManager.LocalCoins.ToString ();
		}
	}

}