using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoffeeCommand {



	public class FlagSetup : MonoBehaviour {
		public static FlagSetup inst;


		bool flagColorWasSet = false;
		public Image[] colors;
//		Color[] savedFlagColors;



		void Start(){
			 
			SetFlagColor (UserDataManager.Flag.GetLocalColors);
		}

		public void ResetLocalColors () {
			PlayerPrefs.DeleteKey ("SavedFlagColors");
			SetFlagColor (UserDataManager.Flag.GetLocalColors);
		}


		public void SetFlagColor(Color[] flagColors){
			for (int i = 0; i < colors.Length; i++) {
				if (flagColors.Length > i - 1) {
					colors [i].color = flagColors [i];
				} 
			}
		}
	}

	
}
