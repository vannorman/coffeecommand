using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class CCGameManager : MonoBehaviour {


	public enum Scene {
		Real,
		Debug,
		Dummy
	}
	public Scene scene = Scene.Real;

	void Start(){
		switch (scene) {
		case Scene.Real:
//			FindObjectOfType<CoffeeCommandView> ().PlaceOnion ();
			// New map right away
			// Place onion right away

			break;
		}
	}

//	public void Reset(){
//
//
//		Application.LoadLevel(Application.loadedLevel);
//	}

	public void LoadFirstScene(){
		LibPlacenote.Instance.StopSession ();
		UnityEngine.SceneManagement.SceneManager.LoadScene (0);
	}

	public void LoadDummyScene(){
		LibPlacenote.Instance.StopSession ();
		UnityEngine.SceneManagement.SceneManager.LoadScene (1);
	}
}
