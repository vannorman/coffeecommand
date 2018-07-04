using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace CoffeeCommand {
	public static class UserDataManager {
		

		// <summary>
		// Initializes the UserData object to be saved with metadata, including
		// Setting founder of this map
		// Adding to visitor log
		// Adding to current visitors
		//
		// And keeps track of all events that modify the metadata of this map including
		// Change of mine status ("undiscovered" => "discovered")
		// Change of mine coin count
		// Change of mine ownership
		// This is always a user induced action so we mark this user in the object
		// </summary>



		public static int LocalCoins {
			get { 
				if (!PlayerPrefs.HasKey ("Coins")) {
					PlayerPrefs.SetInt ("Coins", 0);
				}
				return PlayerPrefs.GetInt ("Coins");
			}
			set {
//				CLogger.Log ("set local coins:" + value);
				if (!PlayerPrefs.HasKey ("Coins")) {
//					CLogger.Log ("set local coins: Create playerprefs var");
					PlayerPrefs.SetInt ("Coins", 0);
				}
				PlayerPrefs.SetInt ("Coins", value);
			}
		}

		public static JToken InvisibleFlag {
			get { 
//				Dictionary<string, bool> d = new Dictionary<string, bool> ();
//				d.Add ("invisible", true);
				return JObject.FromObject (new { invisible = true });
//				Newtonsoft.Json.JsonConvert.Seri
//				JToken i = new JToken ();
//				i["invisible"] = true;
//				return i;
			}
		}

		public static void ChangePlaceOwner(User newOwner){
			CLogger.Log ("new owner:" + newOwner.userId);
			CLogger.Log ("local data:" + localData.ToString ());
			CLogger.Log ("local data owner:" + localData.owner.userId.ToString ());
//			CLogger.Log ("new owner:" + localData.owner.userId.ToString ());
			localData.owner = newOwner;	
			CLogger.Log ("settings new owner:" + newOwner.userId);
//			UpdateLocalUserDataOnServer ();
		}


		public static bool LocalUserIsOwner {
			get {
				return LocalData.owner.userId == LocalUser.userId;
			}
		}

		public static User LocalUser {
			get { 
				User u = new User ();
				u.userId = UserID;
				u.flag = new Flag ();
				u.flag.flagColors = Flag.GetLocalColors;
				return u;
			}
		}

		public static bool loadedExistingMap = false; // if true, we will save a "ghost" map to get the mesh, and upload metadata over the existing map
		private static string loadedMapPlaceIdP;
		public static string loadedMapPlaceId {
			get { 
				if (loadedMapPlaceIdP == null) {
					Debug.Log ("no id");
					return "NONE";
				}
				return loadedMapPlaceIdP;
			}
			set { 
//				CLogger.Log ("set id:" + value);
				loadedMapPlaceIdP = value;

			}
		}

		private static CoffeeCommandObject localData;
		public static CoffeeCommandObject LocalData {
			get { 
				return localData;
			} 
			set { 
				localData = value;
			}
		}

//
		public static CoffeeCommandObject InitCoffeeCommandObject(string debug = "init"){
//			CLogger.Log (debug+" local data started.");
			CoffeeCommandObject newLocalData = new CoffeeCommandObject ();
			newLocalData.mine = new Mine ();
			newLocalData.mine.coins = new Coins ();
			newLocalData.mine.coins.count = 0;  // make earning inital coins a separate event

			newLocalData.owner = new User ();
//			newLocalData.mine.coins.coinGenerationRate = 1;
			newLocalData.mine.coins.timeLastCollected = System.DateTime.UtcNow;
			newLocalData.invisible = false;
			User u = LocalUser;
			Visitor v = new Visitor ();
			v.user = u;
			v.dateTime = System.DateTime.UtcNow;
			newLocalData.visitors.Add (v);
//			CLogger.Log (debug +" local data finished.");
			return newLocalData;
		}

//		public static JObject localDataObject (CoffeeCommandObject coffeeCommandObject) {
//			get { 
//				return JObject.FromObject (localData); // Newtonsoft.Json.JsonConvert.DeserializeObject(Newtonsoft.Json.JsonConvert.SerializeObject( localData));
////				return new JObject();
//			}
//		}
//
		public class CoffeeCommandObject {
//			public string mapId;
			public string mapName = "none";
			public List<Visitor> visitors = new List<Visitor>();
			public bool invisible = false;
			public Mine mine;
			public User owner;
			public User founder;
			public DateTime lastUpdatedTime;

//			public bool invisible = false; // if inviisble, it is not searchable / updateable (the game doesn't care about this map -- because another map already exists in this location)

		}

		public static Color[] GetLocalFlagColors{
			get { 
				if (localData.owner != null) {
//					CLogger.Log ("local owner not null, color 0:" + localData.owner.flag.flagColors);

					return localData.owner.flag.flagColors;

				} else {
//					CLogger.Log ("local owner null");

					return new Color[3]{ Color.black, Color.black, Color.gray };
				}
			}
		}

		public enum MineStatus {
			Undiscovered,
			Discovered
		}

		public class Mine {
			public MineStatus status = MineStatus.Undiscovered;


			public Coins coins;
		}

		public class Coins {
//			private int coinGenerationRateP = 1;
			public float coinGenerationRate {
				get { 
					return 0.05f;
				}	
//				set { 
//					coinGenerationRateP = value;
//				}
			}
			public int count;
			public System.DateTime timeLastCollected;
		}

		public class Visitor {
			public User user;

			public System.DateTime dateTime;
		}

		public class User {
			public string userId = "none";
			public Flag flag;
		}

		public static Color[] GetColorsOfPlaceOnwer(LibPlacenote.MapMetadata metadata) {
			CoffeeCommandObject remoteObject = metadata.userdata.ToObject<CoffeeCommandObject> ();	
//			CLogger.Log ("metadata:" + metadata.userdata.ToString ());
//			CLogger.Log ("get colors. Owner?:" + remoteObject.owner);
//			CLogger.Log ("get colors. Owner?:" + remoteObject.owner.flag);
//			CLogger.Log ("get color of owner: 0:" + remoteObject.owner.flag.flagColors[0]);
			return remoteObject.owner.flag.flagColors;

		}

		public static int NumUniquePlayersAtPlace(LibPlacenote.MapMetadata metadata) {
			CoffeeCommandObject remoteObject = metadata.userdata.ToObject<CoffeeCommandObject> ();	
			return remoteObject.visitors.Select (e => e.user).Distinct ().ToList ().Count;

		}

		public static int NumCoinsAtPlace(LibPlacenote.MapMetadata metadata){
			CoffeeCommandObject remoteObject = metadata.userdata.ToObject<CoffeeCommandObject> ();	
//			Debug.Log ("sys date utc:" + System.DateTime.UtcNow);
//			Debug.Log ("remote date utc:" + remoteObject.mine.coins.timeLastCollected);
			int timeD = (int)(System.DateTime.UtcNow - remoteObject.mine.coins.timeLastCollected).TotalSeconds;
			int coin = Mathf.RoundToInt(remoteObject.mine.coins.coinGenerationRate * NumUniquePlayersAtPlace(metadata) * timeD);
			CLogger.Log ("coins at "+metadata.name+", last harvested "+remoteObject.mine.coins.timeLastCollected+", has coins :"+coin+", timed;" + timeD+", rate:"+remoteObject.mine.coins.coinGenerationRate+", num uniques:"+NumUniquePlayersAtPlace(metadata));
			return coin;

		}



		public static void CollectCurrentMineCoins (Action<int> callbackCoinCt) {
//			Debug.Log ("collect.");	
//			CLogger.Log ("harvst .. coins at "+Mathf.Round(Time.time));
			if (loadedMapPlaceId == null) {
				Debug.Log ("can't, null");
//				Debug.Log ("null.");
				return;
			}


			// Update locally stored place coins based on server (because maybe someone else already mined them)
			LibPlacenote.Instance.GetMetadata(loadedMapPlaceId, (mapData) => {
//				CLogger.Log ("harvst .. old meta for "+loadedMapPlaceId);
				localData = mapData.userdata.ToObject<CoffeeCommandObject>();
				if (localData.owner.userId != LocalUser.userId){
					ToastManager.ShowToast("Error, someone else took over this place already!");
					return;
				}
				int ct = NumCoinsAtPlace(mapData);
				localData.mine.coins.timeLastCollected = System.DateTime.UtcNow;
				localData.lastUpdatedTime = System.DateTime.UtcNow; // For checking against GetMetaData in loop -- will be deprecated when SetMetaData returns a callback


				LibPlacenote.MapMetadataSettable replacementMetadata = new LibPlacenote.MapMetadataSettable();
				replacementMetadata.location = mapData.location;
				replacementMetadata.name = mapData.name;
				replacementMetadata.userdata = JObject.FromObject(localData); 
				LibPlacenote.Instance.SetMetadata (loadedMapPlaceId, replacementMetadata); // This doesn't seem to work -- last time collected is NOT changed
				CLogger.Log("harvested coins at "+System.DateTime.UtcNow+" and set last time collected to :"+localData.mine.coins.timeLastCollected); // Reports the correct "last time collected"
				callbackCoinCt(ct);
			});

		}

		public static System.DateTime lastMetaUpdateTime;

		public class Flag {
			public enum FlagStyle {
				Squares,
				Triangles
			}
			
			public FlagStyle flagStyle;
			public Color[] flagColors; // limited to 4 for now
			public static Color[] GetLocalColors {
				get { 

					// This user's locally saved flag

					if (!PlayerPrefs.HasKey ("SavedFlagColors")) {
						PlayerPrefs.SetString ("SavedFlagColors", Newtonsoft.Json.JsonConvert.SerializeObject (Utils2.GetRandomColors (3, 0.3f)));
					}

					return Newtonsoft.Json.JsonConvert.DeserializeObject<Color[]>(PlayerPrefs.GetString("SavedFlagColors"));
				}
			}

			public static Color[] GetPlaceOwnerColors {
				get {
					if (LocalData.owner == null) {
						return Flag.GetNpcColors;
					} else {
						return GetLocalFlagColors; 
					}
				}
			}

			public static Color[] GetNpcColors {
				get {
					return new Color[] { Color.green, Color.green, Color.green };
				}
			}
		}

		public static void OnMapSelected(bool usedExistingMap=false, LibPlacenote.MapInfo mapInfo=null){
//			CLogger.Log ("OnMapSel ");
			loadedExistingMap = usedExistingMap;
			if (loadedExistingMap) {
//				CLogger.Log ("onMapSel: loaded existing ..");
//				Debug.Log ("loaded existing:"+mapInfo.metadata.userdata.ToString());
				localData = mapInfo.metadata.userdata.ToObject<CoffeeCommandObject>(); 
				localData.mapName = mapInfo.metadata.name;
				loadedMapPlaceId = mapInfo.placeId;
//				CLogger.Log ("OnMapSel: place id "+loadedMapPlaceId);
//				CLogger.Log ("OnMapSel: owner id "+localData.owner.userId);
//				CLogger.Log ("OnMapSel: owner colors 1 "+localData.owner.flag.flagColors[0]);
//				CLogger.Log ("OnMapSel: owner colors 1 "+localData.owner.flag.flagColors[1]);
			} else {
				localData = InitCoffeeCommandObject ();
//				CLogger.Log ("OnMapSel: Local data loaded. visitor count:"+localData.visitors.Count);
			}
		}

		public static string UserID {
			get { 
				return SystemInfo.deviceUniqueIdentifier;
			}
		}


		//	public static JObject LocalData;
	///*
		/// 
		/*

		User loads the local map and sees a list of any nearby maps (if any). Get the metadata for those maps and show Coin count and Visitor log length
		



		


		"visitor log" : [
			{ 
				"UUID" : "UUID1235", 
				"Date" : DateTimeObject
			},{
				"UUID" : "UUID1235", 
				"Date" : DateTimeObject
			}
		]
		"current visitors" : [
			{ 
				"UUID" : "UUID1235", 
				"Date" : DateTimeObject
			}
		]q
		"founder" {
			"UUID" : "UUID12345",
			"Date Founded" : DateTimeObject
		}
	 	"mine" : {
			"status" : "undiscovered",
			"owner" : "none",
			"coins" : {
				"count" : 0,
				"time" : DateTimeObject
			}
		},

	*/
	}

}