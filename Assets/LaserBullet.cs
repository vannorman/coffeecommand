using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CoffeeCommand {

	public class LaserBullet : MonoBehaviour {

		LineRenderer lr;
		DamageGiver dg;
		public GameObject explosion;
		void Start(){
			lr = GetComponent<LineRenderer> ();
			dg = GetComponent<DamageGiver> ();
			lr.SetPositions (new Vector3[]{ Vector3.zero, Vector3.zero });
			CheckRaycast (transform.position);
	//		transform.forward = Camera.main.transform.forward;
		}

		float t1 = 0;
		float t2 = 0;
		float duration = 0.1f;
		float speed = 2500;
		bool dying = false;

		void LateUpdate () {
			if (!dying)
				t1 += Time.deltaTime;
			else if (dying) {
				duration -= Time.deltaTime;
				if (duration < 0) {
					Destroy (this.gameObject);
				}
			}
			if (t1 > duration) {
				t2 += Time.deltaTime;
			}
			Vector3 startPos = transform.TransformPoint(lr.GetPosition (1));
			CheckRaycast (startPos);
		}
		void CheckRaycast(Vector3 startPos){
	//		Debug.Log ("t1:" + t1 + ",t2:" + t2 + ", lr pos:" + lr.GetPosition(0));
			lr.SetPositions (new Vector3[]{ Vector3.fwd * t1 * speed, Vector3.fwd * t2  * speed});
//			Debug.Log ("startpos:" + startPos);
	//		Vector3 dir = lr.GetPosition (0) - lr.GetPosition (1);
			float dist = Vector3.Magnitude (lr.GetPosition (0) - lr.GetPosition (1)) * 3f; // times 3 beacuse unity frame updates are too slow and might "miss" the target between raycasts otherwise
			Vector3 offset = -Camera.main.transform.forward * 0.15f;
			foreach (RaycastHit hit in Physics.RaycastAll( new Ray(startPos + offset,Camera.main.transform.forward),dist)) {
	//			FindObjectOfType<DebugText>().GetComponent<UnityEngine.UI.Text>().text = "hit:"+hit.collider;
//				CLogger.Log("hit:"+hit.collider);
				DamageReceiver dr = hit.collider.GetComponent<DamageReceiver> ();
				if (dr) {
					if (dr.DirectionValid (Camera.main.transform.forward) || dr.directional == false) {
						dr.TakeDamage (dg.damageAmount);
						Destroy (dg);

						GameObject exp = (GameObject)Instantiate (explosion, hit.point, Quaternion.identity);
						dying = true;
					}
				}
			
			}
		}
	}

}