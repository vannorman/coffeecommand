using UnityEngine;
using System.Collections;

public class WaveFlag : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
//	// Update is called once per frame
	void Update () {
		Mesh m = GetComponent<MeshFilter>().sharedMesh;
		Vector3[] verts = m.vertices;
		float dimY = m.bounds.extents.y;
		float amp = .5f;
		float speed = 3.4f;
		for (int v=0;v<verts.Length;v++){
			verts[v] = new Vector3(Mathf.Sin (Time.time*speed + verts[v].y)*verts[v].y/dimY*amp,verts[v].y,verts[v].z);
		}
		m.vertices = verts;
		m.RecalculateBounds();
		m.RecalculateNormals();
	}
}
