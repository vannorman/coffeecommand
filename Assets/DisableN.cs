using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableN : MonoBehaviour {

	public List<GameObject> toDisable = new List<GameObject>();
	int i=0;
	public void DisableIt(){
		if (i >= toDisable.Count) {
			CoffeeCommand.CLogger.Log ("Can't disable nomo, dun");
		} else {
			toDisable [i].SetActive (false);
			CoffeeCommand.CLogger.Log ("disabled "+toDisable[i].name+", "+i+" of "+toDisable.Count);
			i++;
		}

	}
}
