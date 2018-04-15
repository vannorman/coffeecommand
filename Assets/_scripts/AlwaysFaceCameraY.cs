using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceCameraY : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.LookRotation (Utils2.FlattenVector (Camera.main.transform.position - transform.position),Vector3.up);
	}
}
