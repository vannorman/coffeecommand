using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogShip : MonoBehaviour {


	public int coinValue = 5;
	Vector3 startPos;
	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}

	Vector3 target = Vector3.zero;
	float range = 5f;
	float upRange = 0.5f;

	// Update is called once per frame
	float t = 0;
	float interval = 5;
	void Update () {
		t -= Time.deltaTime;
//		Debug.Log ("t;" + t);
		if (t < 0) {
//			interval = Random.Range (2, 6f);
			t = Random.Range (8, 16f);
			SetTarget( Utils2.FlattenVector (startPos + Random.insideUnitSphere * range) + Vector3.up * Random.Range (-upRange, upRange));
//			Debug.Log ("swap");
		}
		float moveSpeed = 0.3f;
		transform.position = Vector3.MoveTowards (transform.position, target, Time.deltaTime * moveSpeed);
		float rotSpeed = 4f;
		Vector3 lookRot = target - transform.position;
		if (lookRot != Vector3.zero) {
			Quaternion r = Quaternion.LookRotation (lookRot);
			transform.rotation = Quaternion.Slerp (transform.rotation,r , Time.deltaTime * rotSpeed);
			
		}
	}

	void SetTarget(Vector3 p){
//		Debug.Log ("set:" + p);
		target = p;
	}

	void OnDestroy(){
		FindObjectOfType<Coins> ().EarnCoin (coinValue);
	}
}
