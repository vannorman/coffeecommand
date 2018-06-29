﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.XR.iOS;
using System.IO;
using System.Threading;
using AOT;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;




using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


namespace EditorCoroutines {

	[System.Serializable]
	public class WWWStatus
	{
		public bool status;
	}

	public class PlacenoteWebAPI : MonoBehaviour {


//		private string baseURL = "https://us-central1-placenote-sdk.cloudfunctions.net/";
//		private string mkey;
//		private string mMapID;
//
//		private LibPlacenote.MapList mMapIDList;
//
//		private bool mMapListUpdated;
//
//		public void PrintMapList(string key, Action<LibPlacenote.MapList> listCb2) {
//			mkey = key.Trim();
//			mMapListUpdated = false;
//
//			EditorCoroutines.StartCoroutine (GetMapListCoroutine ( (listCb3) => {
//				Debug.Log("3:"+listCb3[0]);	
//				listCb2(listCb3);
//			}));
////			listCb(
//
//		}
//
//		IEnumerator GetMapListCoroutine(Action<LibPlacenote.MapList> listCb) {
//			
//			var headers = new Hashtable();
//			headers.Add("APIKEY", mkey);
//
//			using (WWW www = new WWW(baseURL + "apiKeyVerify", null, headers))
//			{
//				yield return www;
//				WWWStatus ret_val = JsonConvert.DeserializeObject<WWWStatus>(www.text);
//				if (ret_val.status == true) {
//					Debug.Log ("API Key Verified.");
//				} else {
//					Debug.LogError ("API Key Not verified. Please get a key from developers.placenote.com and enter it under the GameObject Place Mesh in the Inspector");
//					EditorCoroutines.StopAllCoroutines (this);
//					yield break;
//				}
//			}
//
//			using (WWW www = new WWW (baseURL + "listPlaces", null, headers)) {
//				Debug.Log ("Recalling Map List");
//				yield return www;
//				mMapIDList = JsonConvert.DeserializeObject<LibPlacenote.MapList> (www.text);
//				mMapListUpdated = true; //tell any other co-routines waiting that map list is updated
//				listCb(mMapIDList);
//				yield break;
//			}
//		}
//
//		public void AddMesh(string APIKey, string MapID, GameObject PlaneMesh, Transform MapParent=null) {
//			mMapID = MapID.Trim ();
//			mkey = APIKey.Trim ();
//			mMapListUpdated = false;
//			EditorCoroutines.StartCoroutine (GetMapListCoroutine (false), this);
//			EditorCoroutines.StartCoroutine (AddMeshCoroutine (PlaneMesh, MapParent), this);
//		}
//
//
//		IEnumerator AddMeshCoroutine(GameObject PlaneMesh, Transform MapParent=null) {
//			yield break;
////			var m_index = -1;
////			var index = 0;
////
////			while (!mMapListUpdated) { //wait for maplist update co-routine to finish
////				yield return new WaitForSeconds(0.2f);
////			}
////
////			foreach (var map in mMapIDList.places) {
////				if (map.placeId == mMapID) {
////					m_index = index;
////					break;
////				}
////				index = index + 1;
////			}
////
////			if (m_index < 0) {
////				Debug.Log ("Can't find this MapID, Blank it out to get full list again");
////				yield break;
////			}
////
////
////			Debug.Log ("Extracting planes from " + mMapID);
////
////			UnityEngine.XR.iOS.PlacenotePlaneUtility.InitializePlanePrefab (PlaneMesh);
////			JToken planeMetaData = mMapIDList.places [m_index].userData ["planes"];
////			UnityEngine.XR.iOS.PlaneMeshList meshList = planeMetaData.ToObject<UnityEngine.XR.iOS.PlaneMeshList> ();
////
////
////			GameObject MeshParent;
////			MeshParent = new GameObject ("Mesh");
////			if (MapParent!=null) {
////				MeshParent.transform.SetParent (MapParent);
////			}
////
////
////			int planeNum=0;
////			foreach (var mesh in meshList.meshList) {
////				GameObject plane = UnityEngine.XR.iOS.PlacenotePlaneUtility.CreatePlaneInScene (mesh);
////				plane.transform.SetParent (MeshParent.transform);
////				plane.name = "Plane " + planeNum.ToString ();
////				planeNum++;
////			}
////			FeaturesVisualizer.DrawPointsRaw (meshList.landmarkList, MeshParent.transform);
//
//		}

	}

}

#endif