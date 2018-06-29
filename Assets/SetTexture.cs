using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		Texture2D newTex = new Texture2D(2,2);		

		int dim = 10;
		Texture2D newTex = new Texture2D (dim, dim);
		GetComponent<Renderer>().material.mainTexture = newTex;

		Color[] pixels = new Color[newTex.width * newTex.height];
		Color[] colors = new Color[]{ Color.red, Color.blue, Color.green, Color.yellow };
		for (int i=0;i<dim*dim;i++){
			if (i < dim * 0.25f) {
				//					Debug.Log ("set i" + colors [0]);

				pixels [i] = colors [0];

			} else if (i < dim * 0.50f) {
				//					Debug.Log ("set i" + colors [2]);
				pixels [i] = colors [2];
			} else if (i < dim * 0.75f) {
				//					Debug.Log ("set i" + colors [1]);
				pixels [i] = colors [1];
			} else {
				//					Debug.Log ("set i" + colors [2]);
				pixels [i] = colors [2];
			}
		}
		newTex.SetPixels (pixels);
		newTex.Apply ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
