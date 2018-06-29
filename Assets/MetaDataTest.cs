using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


public class MetaDataTest : MonoBehaviour {

	bool completed = false;
	void Update () {
		if (LibPlacenote.Instance != null && !completed) {
			completed = true;
			Debug.Log ("setting metadata.");
		
			LibPlacenote.MapMetadataSettable metadata = new LibPlacenote.MapMetadataSettable ();
			metadata.name = "test";
			metadata.userdata = JObject.FromObject ( new {test = true });
			LibPlacenote.Instance.SetMetadata ("0oehl9bvzxn9e38djasf57z0zqc5qnojvrxeof93elj08ij0k6vqcis7dbbesri4ckwcsg6spqsya9lymr6f9tht9stgsn4sv4l8", metadata);
		} else {
			Debug.Log ("PN not initialized.");
		}
	}
	

}
