using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CoffeeCommand {
	public class InGameFlag : MonoBehaviour {
		public Renderer flagRenderer;
		public bool ownedByPlayer = false;
		public void Start ()
		{
			if (ownedByPlayer) {
				SetPlayerColors ();
			}
		}

		void SetPlayerColors ()
		{
			SetColors (UserDataManager.Flag.GetLocalColors );
		}

		public void SetColors (Color[] colors){ 


			int dim = 16;
			Texture2D newTex = new Texture2D (dim, dim);

			Color[] pixels = new Color[dim*dim];
			//			for (int i = 0; i < colors.Length; i++) {
//			int dim2 = dim*dim;
			for (int i=0;i<dim;i++){
				for (int j = 0; j < dim; j++) {
					if (i < dim / 2 && j < dim / 2) {
						pixels [i * dim + j] = colors [1];
					} else if (i < dim / 2 && j >= dim / 2) {
						
						pixels [i * dim + j] = colors [2];
					} else if (i >= dim / 2 && j < dim / 2) {
					
						pixels [i * dim + j] = colors [0];
					} else if (i >= dim / 2 && j >= dim / 2) {
						pixels [i * dim + j] = colors [2];
					}
				}
//				if (i < dim2 * 0.25f) {
//
//
//					pixels [i] = Color.red;
//				} else if (i < dim2 * 0.50f) {
//
//					pixels [i] = colors [2];
//					pixels [i] = Color.yellow;
//				} else if (i < dim2 * 0.75f) {
//
//					pixels [i] = Color.green;
//				} else {
//
//					pixels [i] = Color.blue;
//				}
			}
			//			pixels [2] = colors [1];
			//			pixels [3] = colors [2];

			newTex.SetPixels (pixels);
			flagRenderer.material.mainTexture = newTex;
			newTex.Apply ();

//			Color[] colors = FlagSetup.inst.GetFlagColors ();
//			Texture2D newTex = new Texture2D(2,2);		
//			Color[] pixels = new Color[newTex.width * newTex.height];
////			for (int i = 0; i < colors.Length; i++) {
//			pixels[0] = colors[0];
//			pixels [1] = colors [2];
//			pixels [2] = colors [1];
//			pixels [3] = colors [2];
//
//			newTex.SetPixels (pixels);
//			flagRenderer.material.mainTexture = newTex;
//			}
		}
	}

}