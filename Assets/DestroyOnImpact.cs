using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnImpact : MonoBehaviour {

	public bool ignoreBounceProjectile = true;

	void OnCollisionEnter(Collision hit){
		if (ignoreBounceProjectile && hit.collider.GetComponent<BounceProjectile> ())
			return;
		
		Destroy (this.gameObject);
		FX.inst.BulletPoof (transform.position);
//		FX.inst.BulletPoof (hit.contacts[0].point);
	}
}
