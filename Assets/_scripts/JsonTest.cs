using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Runtime.InteropServices;
//using UnityEngine.UI;
//using UnityEngine.XR.iOS;
//using System.IO;
//using System.Threading;
//using AOT;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;


namespace CoffeeCommand {
	[System.Serializable]
	public class WWWStatus
	{
		public bool status;
	}

	public class JsonTest : MonoBehaviour {

		// Use this for initialization
		void Start () {
	//		var distance = MapInfoElement.Calc (0, 0, 0, 1);
	//		Debug.Log ("distance:" + distance);
			JsonT();
		}
		private string baseURL = "https://us-central1-placenote-sdk.cloudfunctions.net/";
//		LibPlacenote.Mapli
		IEnumerator GetMapListCoroutine(Action<LibPlacenote.MapList> listCb) {

			var headers = new Hashtable();
			headers.Add("APIKEY", CoffeeCommandSettings.apiKey);

			using (WWW www = new WWW(baseURL + "apiKeyVerify", null, headers))
			{
				yield return www;
				WWWStatus ret_val = JsonConvert.DeserializeObject<WWWStatus>(www.text);
				if (ret_val.status == true) {
//					Debug.Log ("API Key Verified.");
				} else {
					Debug.LogError ("API Key Not verified. Please get a key from developers.placenote.com and enter it under the GameObject Place Mesh in the Inspector");
					StopAllCoroutines ();
					yield break;
				}
			}

			using (WWW www = new WWW (baseURL + "listPlaces", null, headers)) {
//				Debug.Log ("Recalling Map List");
				yield return www;
//				Debug.Log ("ww:" +www.text);
				JObject jo = JsonConvert.DeserializeObject<JObject> (www.text);
				JObject[] jo1 = JsonConvert.DeserializeObject<JObject[]> (jo["places"].ToString());
				List<LibPlacenote.MapInfo> maps = new List<LibPlacenote.MapInfo> ();
				foreach (JObject j in jo1){
//					Debug.Log ("j:" + j);
					LibPlacenote.MapInfo mapInfo = new LibPlacenote.MapInfo ();
					mapInfo.placeId = j ["placeId"].ToString ();
					mapInfo.metadata = new LibPlacenote.MapMetadata ();
					mapInfo.metadata.userdata = JsonConvert.DeserializeObject<JToken> (j ["userData"].ToString());
					maps.Add (mapInfo);
				}
				LibPlacenote.MapList mMapIDList = new LibPlacenote.MapList ();
				mMapIDList.places = maps.ToArray ();
//				Debug.Log ("maplist:" + mMapIDList.places [0].placeId);
//				mMapListUpdated = true; //tell any other co-routines waiting that map list is updated
				listCb(mMapIDList);
				yield break;
			}
		}


		IEnumerator AfterSeconds(float s){
			yield return new WaitForSeconds (s);
			Debug.Log("Mine coins now:"+UserDataManager.NumCoinsAtPlace(savedMap.metadata));
		}

		LibPlacenote.MapInfo savedMap;
		void JsonT () {
			StartCoroutine(GetMapListCoroutine( (mapList) => {
				
				foreach (LibPlacenote.MapInfo mapId in mapList.places) {
//					Debug.Log("mapid;"+mapId.placeId);

//					Debug.Log("mapid.meta;"+mapId.metadata.ToString());
//					Debug.Log("mapid.meta.user;"+mapId.metadata.userdata.ToString());
//					Debug.Log("meta mine owner:"+ mapId.metadata.userdata.ToObject<UserDataManager.CoffeeCommandObject>().mine.coins.timeLastCollected);
					savedMap = mapId;
					Debug.Log("Mine coins now:"+UserDataManager.NumCoinsAtPlace(mapId.metadata));
					//					UserDataManager.Mine mine = mapId.metadata.userdata.ToObject<UserDataManager.CoffeeCommandObject>().mine;
					//					Debug.Log("mine
					break;
				}
				StartCoroutine(AfterSeconds(5));
			}));
				
				

//			EditorCoroutines.PlacenoteWebAPI; //  .Instance.ListMaps ((mapList) => {
//
//
				
				


//			UserDataManager.CoffeeCommandObject cco = UserDataManager.InitCoffeeCommandObject ();
////			cco.mine.coins.l
//			Debug.Log("coins:"+UserDataManager.NumCoinsAtPlace( cco.mine.coins.count);
//			StartCoroutine
//			return;


//			JToken i = UserDataManager.InvisibleFlag;
//			Debug.Log ("i:" + i.ToString ());
//			bool a = i ["invisible"] == null ?  false : i ["invisible"].ToObject<bool> ();
//			bool b = i ["invisible2"] == null ? false : i ["invisible2"].ToObject<bool> ();
//			Debug.Log ("a: " + a + ", b:" + b); // True, false
//			inv:" + i ["invisible"]); // True
//			if (i["invisible2"] == true){
//				Debug.Log ("blah");
//			}
//			Debug.Log ("inv2:" + (i ["invisible2"] == true).ToString()); // null

			return;
//			UserDataManager.CoffeeCommandObject cco = new UserDataManager.CoffeeCommandObject ();
//			UserDataManager.User testUser = new UserDataManager.User ();
//			testUser.userId = "123a";
//			testUser.flag = new UserDataManager.Flag ();
//			testUser.flag.flagColors = new Color[] { Color.red, Color.black, Color.blue, Color.yellow };
////			testUser.flag.flagStyle = UserDataManager.FlagStyle.Triangles;
//
//			cco.mine = new UserDataManager.Mine ();
//
//			UserDataManager.Visitor newVisitor = new UserDataManager.Visitor ();
//			newVisitor.user = testUser;
//			newVisitor.dateTime = System.DateTime.Now;
//			cco.visitors.Add (newVisitor);
//			cco.visitors.Add (newVisitor);
//
//			string js = JsonConvert.SerializeObject (cco);
//			print (js);
//
//			UserDataManager.CoffeeCommandObject cco2 = JsonConvert.DeserializeObject<UserDataManager.CoffeeCommandObject> (js);
//			Debug.Log ("cco2 vist len:" + cco2.visitors.Count);
//
//			// Suppose I want only unique visitors for a total pop count
//			var uniqueCerts = cco.visitors.Select(e => e.user).Distinct().ToList(); //  SelectMany(e => e.user).Distinct().ToList();



	//		JObject o = UserDataManager.CoffeeCommandObject ("123a");
	//		UserDataManager.AddUserToVisitorLog (o, "abcd");
	//		Debug.Log ("o:");
	//		print (o.ToString ());

		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}

}