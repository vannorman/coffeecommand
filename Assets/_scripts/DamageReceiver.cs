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

	public bool DirectionValid(Vector3 dir){
		float angle =  Vector3.Angle (dir, transform.TransformVector (localDir));
//		Debug.Log ("direction valid?" + dir+", angle:"+angle);
		if (angle < angleToTakeDamage) {
			//				Debug.Log ("Bad angle, no dam");
			return true;
		} else {
			return false;
		}
	}

	public void TryTakeDamage(DamageGiver dg){
//		Debug.Log ("dg.rig:" + dg.GetComponent<Rigidbody> ().velocity);
		if (directional) {
//			Vector3 dirToDamageGiver = (dg.transform.position - this.transform.position).normalized;
//			Vector3 dirToDamageGiver = dg.GetComponent<Rigidbody>().velocity;
			Vector3 dirToDamageGiver = Camera.main.transform.position - transform.position;
			if (!DirectionValid (dirToDamageGiver)) {
				return;
			}
		}
		TakeDamage (dg.damageAmount);
		DamageFx (dg);
	}

	public void DamageFx(DamageGiver dg){
		
		FX.inst.SmallExplosionDamageEffect (dg.transform.position);
	}
	public void TakeDamage(int dam){
	
	
		hitPoints -= dam;
		if (hitPoints < 1) {
			Die ();
		}
	}

	void Die() {
		if (objToDie != null) Destroy(objToDie);
		Destroy (this.gameObject);
	}
	void OnDestroy(){
		
	
//		objToSendMessage.SendMessage(messageToSend);
	}

}
