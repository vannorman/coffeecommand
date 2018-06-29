using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMetadataTest : MonoBehaviour {

	public void SetMeta(){
		string mapid = FindObjectOfType<PlacenoteSampleView> ().mSelectedMapId;
		LibPlacenote.MapMetadataSettable metaRep = FindObjectOfType<PlacenoteSampleView> ().mCurrMapDetails;
		metaRep.name = "meta test";
		metaRep.userdata = Newtonsoft.Json.Linq.JObject.FromObject (new { test = true  });
		LibPlacenote.Instance.SetMetadata (mapid, metaRep);
		ToastManager.ShowToast ("change map id name:" + mapid);
	}
}
