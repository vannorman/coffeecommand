using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCGameManager : MonoBehaviour {

	public void Reset(){
		Application.LoadLevel(Application.loadedLevel);
	}
}
