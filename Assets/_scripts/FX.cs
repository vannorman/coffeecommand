using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX : MonoBehaviour {

	public enum EffectName {
		BulletPoof,
		SmallExplosionDamage,
	}

	[System.Serializable]
	public class Effect
	{
		public EffectName name;
		public GameObject particleSystem;
		public ParticleSystem ps;
	
	}
	[SerializeField]
	public Effect[] effects;
	public static FX inst;
	Dictionary<EffectName, Effect> effectsList = new Dictionary<EffectName,Effect>();
	void Start(){
		inst = this;
		foreach (Effect e in effects) {
			e.ps = (ParticleSystem)Instantiate (e.particleSystem).GetComponent<ParticleSystem> ();
			effectsList.Add (e.name, e);
			e.ps.transform.position = Vector3.zero;
		}
	}

	public void BulletPoof(Vector3 p){
		ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams ();
		emitParams.position = p;
		effectsList[EffectName.BulletPoof].ps.Emit(emitParams,1);
	}

	public void SmallExplosionDamageEffect(Vector3 p){
		ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams ();
		emitParams.position = p;
		effectsList[EffectName.SmallExplosionDamage].ps.Emit(emitParams,1);
	}
}
