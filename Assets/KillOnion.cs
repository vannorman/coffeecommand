using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnion : MonoBehaviour {

	int stage = 0;
	public void KillEm(){
		foreach (CoffeeCommand.MetalOnion mo in FindObjectsOfType<CoffeeCommand.MetalOnion>()) {
			if (stage == 0) 
				mo.SetState (CoffeeCommand.MetalOnion.State.Unwrapping);
			if (stage == 1)
				mo.OnDishGroupDestroyed ();
			stage++;
		}
	}
}
