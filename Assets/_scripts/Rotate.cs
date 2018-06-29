using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {


	public bool randomizeDir = false;
	public Vector3 dir = new Vector3(1,1,1);
	public float rotSpeed = 0.1f;
    public Space space = Space.World;
	// Use this for initialization
	void Start () {
		if (randomizeDir){
			dir = Random.insideUnitSphere;
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (dir * rotSpeed * Time.deltaTime, space);

	}
}
