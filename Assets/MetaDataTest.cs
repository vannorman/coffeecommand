using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CoffeeCommand {
	public class MetaDataTest : MonoBehaviour {



		public void SetMeta1(){

			Debug.Log ("meta 1 - 0");
			string mapid = FindObjectOfType<CoffeeCommandView> ().mSelectedMapId;
			LibPlacenote.Instance.GetMetadata (mapid, (mbcb) => {
			
//				LibPlacenote.MapMetadataSettable metaRep = mbcb;
				LibPlacenote.MapMetadataSettable metaRep = new LibPlacenote.MapMetadataSettable ();
				metaRep.location = mbcb.location;
				metaRep.userdata = mbcb.userdata;
				metaRep.name = "meta 1 test";

				LibPlacenote.Instance.SetMetadata (mapid, metaRep);
				ToastManager.ShowToast ("4 change map id name:" + mapid);
				Debug.Log ("fin meta 1 - 0");
			});
		}


		public void SetMeta2(){
			Debug.Log ("meta 2 - 0");
			string mapid = FindObjectOfType<CoffeeCommandView> ().mSelectedMapId;
			LibPlacenote.MapMetadataSettable metaRep = new LibPlacenote.MapMetadataSettable ();
			metaRep.name = "meta 2 test";
			metaRep.userdata = Newtonsoft.Json.Linq.JObject.FromObject ( UserDataManager.LocalData );
			Debug.Log ("userd meta 2:" + metaRep.userdata);
			LibPlacenote.Instance.SetMetadata (mapid, metaRep);
			ToastManager.ShowToast ("4 change map id name:" + mapid);
			Debug.Log ("fin meta 2 - 0");
		}



		public void SetMeta3(){
			
			string mapid = FindObjectOfType<CoffeeCommandView> ().mSelectedMapId;
			LibPlacenote.MapMetadataSettable metaRep =new LibPlacenote.MapMetadataSettable (); // = FindObjectOfType<CoffeeCommandView> ().mSelectedMapInfo.metadata;
			metaRep.name = "Meta 3";
			UserDataManager.User newUser = new UserDataManager.User ();
			newUser.flag = new UserDataManager.Flag ();
			newUser.flag.flagColors = new Color[]{ Color.blue };
			UserDataManager.LocalData.owner = newUser;
			metaRep.userdata = Newtonsoft.Json.Linq.JObject.FromObject (UserDataManager.LocalData);
			LibPlacenote.Instance.SetMetadata (mapid, metaRep);
			ToastManager.ShowToast ("set Meta 3:" + mapid);
		}

		public void SetMeta4(){
			Debug.Log ("meta43 - 0");
			string mapid = FindObjectOfType<CoffeeCommandView> ().mSelectedMapId;
			LibPlacenote.MapMetadataSettable metaRep = new LibPlacenote.MapMetadataSettable ();
			metaRep.name = "meta test";
			metaRep.userdata = Newtonsoft.Json.Linq.JObject.FromObject (new { test = true  });
			LibPlacenote.Instance.SetMetadata (mapid, metaRep);
			ToastManager.ShowToast ("4 change map id name:" + mapid);
			Debug.Log ("fin meta 4 - 0");
		}
	}

}