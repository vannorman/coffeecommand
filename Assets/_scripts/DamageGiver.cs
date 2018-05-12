using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour {


	RecordPosition rp;
	void Start(){
		rp = GetComponent<RecordPosition> ();
	}

	public int damageAmount = 1;
	void OnCollisionEnter(Collision hit){
		TryGiveDamage (hit.collider.GetComponent<DamageReceiver> ());
//		DamageReceiver dr = hit.collider.GetComponent<DamageReceiver> ();
//		if (dr)
//			GiveDamage (dr);
	}

	void OnTriggerEnter(Collider collider){
		TryGiveDamage (collider.GetComponent<DamageReceiver> ());
//		DamageReceiver dr = collider.GetComponent<DamageReceiver> ();
//		if (dr)
//			GiveDamage (dr);
	}

	void TryGiveDamage(DamageReceiver dr){
		if (dr == null)
			return;
		dr.TryTakeDamage(this);
	}

	void Update(){
		RaycastHit hit;
		if (Physics.Raycast (new Ray (rp.lastPosition, rp.nowPosition - rp.lastPosition), out hit, (rp.lastPosition - rp.nowPosition).magnitude * 4f)) {
			DamageReceiver dr = hit.collider.GetComponent<DamageReceiver> ();
			if (dr) {
				if (!dr.directional || dr.DirectionValid (rp.lastPosition - rp.nowPosition)) {
					
					dr.TakeDamage (damageAmount);
					dr.DamageFx (this);
					Destroy (this);
				}
			}
		}
	}
}
