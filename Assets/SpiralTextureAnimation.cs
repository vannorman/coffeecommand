using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralTextureAnimation : MonoBehaviour {


	Material m;

	void Start() {
		m = GetComponent<Renderer> ().material;
	}
	
	// Update is called once per frame
	float t = 0.2f;
	public float speed = .002f;
	public float min = 0.25f;
	void Update () {
		float speedFactor = Mathf.Pow(t,1.2f)  * speed;
		t += Time.deltaTime * speedFactor;
		if (t > 1) {
			t = min;
		}
//		Vector2 o = m.GetTextureOffset ("_MainTex");
		m.SetTextureScale ("_MainTex", Vector2.one * t);
//		(1 - (t % 1)) / 2f);// ((2 * t)%1));
		m.SetTextureOffset("_MainTex", Vector2.one * (1 - t)/2f);
//		m.SetTextureOffset("_MainTex", Vector2.one * Mathf.Sqrt(Mathf.Abs(1 - Mathf.Pow(t,2)))); // new Vector2(t,t));


		// Tile  	.50	
		// Offset	.25	
	}
}
