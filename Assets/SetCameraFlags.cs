using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraFlags : MonoBehaviour {
	public Camera cam;
	public void SetCamFlag(){
		cam.clearFlags = CameraClearFlags.Depth;
	}
}
