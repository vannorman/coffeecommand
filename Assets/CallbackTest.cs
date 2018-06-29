using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using System.Runtime.InteropServices;
//using UnityEngine.UI;
//using UnityEngine.XR.iOS;
//using System.IO;
//using System.Threading;
//using AOT;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;
//using System.Runtime.InteropServices;

public class CallbackTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MethodWithCallback ((cb) => {
			Debug.Log("cb?"+cb);
		});	
	}
	


	void MethodWithCallback(Action<string> callback){
		Debug.Log ("method called");
		callback("asd");
		
	}
}
