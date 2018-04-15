using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadReplacement : MonoBehaviour {

	public GameObject body;
	
	void OnDestroy(){
		GameObject d = (GameObject)Instantiate (body);
		d.transform.position = transform.position;
		d.transform.rotation = transform.rotation;
	}
}
