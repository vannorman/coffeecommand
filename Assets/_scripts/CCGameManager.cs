using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

	public void LoadUserTestScene(){
		// main game
		UnityEngine.SceneManagement.SceneManager.LoadScene (1);
	}

	public void LoadDeveloperScene(){
		UnityEngine.SceneManagement.SceneManager.LoadScene (2);	
	}

	public void LoadFirstScene(){
		LibPlacenote.Instance.StopSession ();
		UnityEngine.SceneManagement.SceneManager.LoadScene (1);
	}

	public void LoadDummyScene(){
		LibPlacenote.Instance.StopSession ();
		UnityEngine.SceneManagement.SceneManager.LoadScene (2);
	}

	public void LoadScene(Button b){
		UnityEngine.SceneManagement.SceneManager.LoadScene (b.GetComponent<LoadSceneButton> ().sceneName);// scene.name);
	}
}
