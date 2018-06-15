using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Counter : MonoBehaviour {


	public GameObject capsule;
	public TextMesh text;

	// Update is called once per frame
	float deltaTime = 0f;
	void Update () {
		capsule.transform.Rotate (capsule.transform.forward * Time.deltaTime * 720);
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string t = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		text.text = t;

	}
}
