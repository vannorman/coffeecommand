using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Crosshair : MonoBehaviour {

	Vector3 startSize;
	Vector3 bigSize;
	Vector3 targetSize;
	Color targetColor;
	Image im;
	public Text messageText;
	string message = "";
	int messageLetterCount = 0;

	void Start(){
		startSize = transform.localScale;

		bigSize = startSize * 1.2f;
		im = GetComponent<Image> ();

		SetState (State.Nominal);
//		targetColor = Color.white;

	}
	public enum State {
		Asleep,
		Nominal,
		Destructible
	}
	State state = State.Asleep;
	float stateTimeout = 0f;
	public void SetState(State newState){
		if (stateTimeout > 0)
			return;
		stateTimeout = 0.4f;
		if (state != newState) {
			state = newState;
			Debug.Log ("Set cross:" + newState);
			switch (state) {
			case State.Nominal:
				targetColor = Color.white;
				targetSize = startSize;
				SetMessage (""); //message = "";
				messageLetterCount = 0;
				break;
			case State.Destructible:
				SetMessage ("DESTRUCTIBLE");
				targetSize = bigSize;
				targetColor = Color.red;

				break;
			default:
				break;
			}
		}
	}


	float messageTimer = 0;

	void SetMessage(string s){
		message = s;
//		targetLetterCount = s.Length;
//		messageLetterCount = s.Length;
	}

	int targetLetterCount = 0;

	void Update(){
		stateTimeout -= Time.deltaTime;
		float growSpeed = 3f;
//		Debug.Log ("target size;" + targetSize + ", targ color:" + targetColor);
		transform.localScale = Vector3.Lerp (transform.localScale, targetSize, Time.deltaTime * growSpeed);
		im.color = Color.Lerp (im.color, targetColor, Time.deltaTime * growSpeed);
		messageTimer -= Time.deltaTime;
		if (messageTimer < 0) {
			messageTimer = .05f;
			messageLetterCount = Mathf.Min(message.Length, messageLetterCount +1);
//			Debug.Log ("message letter count:" + messageLetterCount);
		}
			
		messageText.text = message.Substring (0, messageLetterCount);
	}
}
