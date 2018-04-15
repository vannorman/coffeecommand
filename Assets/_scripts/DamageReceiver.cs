using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour {


	public int hitPoints = 50;
	public bool directional = true; // only receive damage from this direction
	public int angleToTakeDamage = 120;
	public Vector3 localDir = Vector3.up;
	public GameObject objToDie;
//	public GameObject objToSendMessage;
//	public string messageToSend;
	public void TryTakeDamage(DamageGiver dg){

		if (directional) {
			Vector3 dirToDamageGiver = (dg.transform.position - this.transform.position).normalized;
			if (Vector3.Angle (dirToDamageGiver, transform.TransformVector (localDir)) > angleToTakeDamage) {
				return;
			}
		}
		hitPoints -= dg.damageAmount;
		FX.inst.SmallExplosionDamageEffect (dg.transform.position);
		if (hitPoints < 1) {
			Die ();
		}
	}

	void Die() {
//		Destroy (this.gameObject);
		Destroy(objToDie);
	}
	void OnDestroy(){
	
//		objToSendMessage.SendMessage(messageToSend);
	}

}
