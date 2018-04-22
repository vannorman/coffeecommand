using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class stringtester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		JObject metadata = new JObject ();
		StringTest testData = new StringTest();
		testData.stringtests = new string[2];
		testData.stringtests[0] = "test1";
		testData.stringtests[1] = "test2";
		metadata["test"] = JObject.FromObject (testData);

		StringTest tests = metadata ["test"].ToObject<StringTest> ();
		DebugText.Overflow (tests.stringtests[0] + "," + tests.stringtests[1]);
//		Debug.Log ("OK?"+tests.stringtests [0] + "," + tests.stringtests [1]);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
