using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWeaponsLaser : MonoBehaviour {

	public bool weaponsOnline = true;
	void Start(){
//		lasersFiring = this.gameObject.AddComponent<AudioSource> ();
//		//		lasersFiring.transform.SetParent (transform);
//		lasersFiring.playOnAwake = false;
//		lasersFiring.loop = true;
//		lasersFiring.clip = firinglasers;

	}

	bool firing = false;
//	bool firing {
//		get { 
//			return Input.touches.Length > 0; // if we want whole screen, but realistically we want to be able to tap the screen WITHOUT firing ...
//		}
//	} 
	public void FireButtonDown() {
		if (weaponsOnline) {
			firing = true;

//			lasersFiring.Play ();
		}
	}

	public void FireButtonUp() {
		firing = false;
//		lasersFiring.Stop ();
	}

	public Transform leftCannon;
	public Transform rightCannon;
	public GameObject laserPrefab;
	float fireTime = 0;
	float fireInterval =.1f;
	public float laserForce = 600;

	//	public Material laserMat;
	//	LineRenderer[] lasers = new LineRenderer[2];
	//	void InitLasers(){
	//		for (int i = 0; i < lasers.Length; i++) {
	//			lasers [i] = this.gameObject.AddComponent<LineRenderer> ();
	//			lasers [i].material = laserMat;
	//			lasers [i].SetVertexCount (0);
	//		}
	//	}

	void Update(){
		if (firing){
			fireTime -= Time.deltaTime;
			if (fireTime < 0) {
				fireTime = fireInterval;
				foreach(Transform t in new Transform[]{ leftCannon, rightCannon }){
					GameObject laser = (GameObject)Instantiate (laserPrefab, t.position, t.rotation);
//					laser.transform.Rotate (new Vector3 (90, 0, 0), Space.Self);
//					laser.GetComponent<Rigidbody> ().AddForce (t.forward * laserForce);
//					float rotationForce = Random.Range (-50, 50);
//					laser.GetComponent<Rigidbody> ().AddTorque (t.right * rotationForce);
				}
			}

		}
	}
}
