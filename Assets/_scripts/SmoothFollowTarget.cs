using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowTarget : MonoBehaviour {

	public Transform target;
	public float followSpeed = 3f;
	// Update is called once per frame
	Vector3 targetPos;
//	public float moveSpeed = 0.2f;
	void LateUpdate () {
//		Utils2.CubicInOut(x:3f);
//		targetPos = Vector3.Lerp (transform.position, target.position, Time.deltaTime * followSpeed);
//		transform.position = Vector3.MoveTowards (transform.position, targetPos, Time.deltaTime * moveSpeed);
		transform.position = Vector3.Lerp (transform.position, target.position, Time.deltaTime * followSpeed);
		transform.rotation = Quaternion.Lerp (transform.rotation, target.rotation, Time.deltaTime * followSpeed / 1.5f);
	}
}
