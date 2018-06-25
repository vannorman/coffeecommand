using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CoffeeCommand {
	public class ShipWeapons : MonoBehaviour {
//		public static ShipWeapons inst;
		public AudioClip firingBullets;
		AudioSource bulletsFiring;
		public bool weaponsOnline = true;
		void Start(){
			bulletsFiring = this.gameObject.AddComponent<AudioSource> ();
	//		bulletsFiring.transform.SetParent (transform);
			bulletsFiring.playOnAwake = false;
			bulletsFiring.loop = true;
			bulletsFiring.clip = firingBullets;

		}


		bool firing = false;
		public void FireButtonDown() {
			if (weaponsOnline) {
				firing = true;
				bulletsFiring.Play ();
			}
		}

		public void FireButtonUp() {
			firing = false;
			bulletsFiring.Stop ();
		}

		public Transform leftCannon;
		public Transform rightCannon;
		public GameObject bulletPrefab;
		float fireTime = 0;
		float fireInterval =.1f;
		public float bulletForce = 600;

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
						GameObject bullet = (GameObject)Instantiate (bulletPrefab, t.position, t.rotation);
						bullet.transform.Rotate (new Vector3 (90, 0, 0), Space.Self);
						bullet.GetComponent<Rigidbody> ().AddForce (t.forward * bulletForce);
						float rotationForce = Random.Range (-50, 50);
						bullet.GetComponent<Rigidbody> ().AddTorque (t.right * rotationForce);
					}
				}

			}
		}
	}

}