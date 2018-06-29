using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedMarkerFx : MonoBehaviour {

	Vector3 startSize;
	Color startColor;
	Renderer r;
	// Use this for initialization
	void Start () {
		startSize = transform.localScale;
		r = GetComponent<Renderer> ();
		startColor = r.material.color;


	}
	
	float t = 0;
	float duration = 3.5f;
	float scale = 2f;
	void Update () {
		t += Time.deltaTime;
		if (t > duration) {
			transform.localScale = startSize;
			r.material.color = startColor;
			t = 0;
		}
		transform.localScale = startSize * (t + 1) * 2f;
		r.material.color = Color.Lerp (r.material.color, Color.clear, Time.deltaTime);
	}
}
