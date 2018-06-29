using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoffeeCommand {
	public class InstanceSetter : MonoBehaviour {
		



		public CoffeeCommandView coffeeCommandView;
		public PlaceSelectionManager placeSelectionManager;
		public GameManager gameManager;
		public Crosshair crosshair;
		public CoffeeCommandFeaturesVisualizer featuresVisualizer;
		public OnionLocationHelper onionLocationHelper;
		public CLogger clogger;
//		public ShipWeapons shipWeapons;
		public FlagSetup flagSetup;
		void Start(){
			
			CoffeeCommandView.inst = coffeeCommandView;
			PlaceSelectionManager.inst = placeSelectionManager;
			GameManager.inst = gameManager;
			Crosshair.inst = crosshair;
			CoffeeCommandFeaturesVisualizer.inst = featuresVisualizer;
			OnionLocationHelper.inst = onionLocationHelper;
//			ShipWeapons.inst = shipWeapons;
			FlagSetup.inst = flagSetup;
			CLogger.inst = clogger;
			//		Debug.Log ("gm:" + CoffeeCommand.GameManager.inst);
		}






	

	}

}