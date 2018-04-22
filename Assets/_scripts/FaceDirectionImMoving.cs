using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDirectionImMoving : MonoBehaviour {

	RecordPosition rp;

	void Start(){
		rp = !!GetComponent<RecordPosition> () ? GetComponent<RecordPosition> () : gameObject.AddComponent<RecordPosition> ();
	}

	public float swingSpeed = 1f;
	// Update is called once per frame
	Vector3 lastMoveDir = Vector3.zero;
	void Update () {
		Vector3 moveDir = rp.nowPosition - rp.lastPosition;
		if (moveDir.sqrMagnitude > 0) {
			lastMoveDir = moveDir;
		}
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (lastMoveDir), Time.deltaTime * swingSpeed);
//		transform.forward = Vector3.Lerp (transform.forward, rp.nowPosition - rp.lastPosition, Time.deltaTime * swingSpeed);
	}
}
