using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace CoffeeCommand {
	public class MapMarkerInfo : MonoBehaviour {

		public LibPlacenote.MapInfo mapInfo;
		public Text people;
		public Text coins;
		public InGameFlag flag;
		public GameObject fx;
		public GameObject outOfRangeFx;

		public bool outOfRange = false;
		public void NotifyLocationOutOfRange(){
			outOfRange = true;
		}

		public void SetMapInfo(LibPlacenote.MapInfo mapId){
			int numP = UserDataManager.NumUniquePlayersAtPlace(mapId.metadata);

//			CLogger.Log("num players:"+numP);
			people.text = numP.ToString();
			int numCoin = UserDataManager.NumCoinsAtPlace(mapId.metadata);
			CLogger.Log("num coin:"+numCoin);
			coins.text =  numCoin.ToString(); // mapId.metadata.userdata.ToObject<UserDataManager.CoffeeCommandObject>().mine.coins.count.ToString(); 
			Color[] colors = UserDataManager.GetColorsOfPlaceOnwer(mapId.metadata);
			//					CLogger.Log("cols:"+colors.Length);
			flag.SetColors(colors);
			mapInfo = mapId;
//			CLogger.Log ("mapid:" + mapId.placeId);
				
		}


	}
}