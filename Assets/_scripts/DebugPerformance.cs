using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CoffeeCommand {
	public class DebugPerformance : MonoBehaviour {

		OnionDetector odc;
		OnionDetector od {
			get {
				if (odc == null) {
					odc = FindObjectOfType<OnionDetector> ();
				}
				return odc;
			}
		}
		public void ToggleRaycast(){
			od.enabled = !od.enabled;
			ToastManager.ShowToast ("Raycats:" + od.enabled);
		}

		OnionLocationHelper ohc;
		OnionLocationHelper oh {
			get {
				if (ohc == null) {
					ohc = FindObjectOfType<OnionLocationHelper> ();
				}
				return ohc;
			}
		}
		public void ToggleOnionHelper(){
			oh.enabled = !oh.enabled;
			ToastManager.ShowToast ("Onion helper:"+oh.enabled);
		}


		CoffeeCommandFeaturesVisualizer coffeeC;
		CoffeeCommandFeaturesVisualizer coffee {
			get {
				if (coffeeC == null) {
					coffeeC = FindObjectOfType<CoffeeCommandFeaturesVisualizer> ();
				}
				return coffeeC;

			}
		}
		public void ToggleFeaturesVisualizer(){
			coffee.enabled = !coffee.enabled;
			ToastManager.ShowToast ("Coffee features vis;"+coffee.enabled);
		}


	}

}