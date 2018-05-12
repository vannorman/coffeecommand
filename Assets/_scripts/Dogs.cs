using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dogs : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		SpawnDogs();	
	}

	public GameObject dogShipPrefab;

	public int ct = 4;
	public void SpawnDogs(){
		foreach (DogShip ds in FindObjectsOfType<DogShip>()) {
			Destroy (ds.gameObject);
		}
		transform.position = Camera.main.transform.position + Vector3.up * -0.2f; // Utils2.FlattenVector (Camera.main.transform.position) + Vector3.up * transform.position.y; // center on cam before spawn.
		for (int i = 0; i < ct; i++) {
			GameObject dog = (GameObject)Instantiate (dogShipPrefab, transform.position, Quaternion.Euler (0, Random.Range (0, 360), 0));
		}

	}

	// Update is called once per frame
	void Update () {
		
	}
}
