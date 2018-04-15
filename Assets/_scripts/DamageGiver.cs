using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour {

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
}
