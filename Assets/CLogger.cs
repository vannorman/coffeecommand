using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoffeeCommand {
	public class CLogger : MonoBehaviour {

		public Text t;
		public Text t2;

		public static CLogger inst;

		public void Clear(){
			lines.Clear ();
			inst.t.text = "";
		}

		static int maxLines = 20;
		static List<string> lines = new List<string> ();
		public static void Log(string s){
			Debug.Log ("CLOG:" + s);
			inst.t.text = "";
			lines.Add (s);
//			Debug.Log("max:"+10+", count;"+lines.Count);
			while (lines.Count > maxLines) {
				lines.RemoveAt (0);
			}
			foreach (string line in lines) {
				inst.t.text += line + "\n";
			}
		}



		static List<string> lines2 = new List<string> ();
		public static void Log2(string s){
			Debug.Log ("CLOG2:" + s);
			inst.t2.text = "";

			lines2.Add (s);
			//			Debug.Log("max:"+10+", count;"+lines.Count);
			while (lines2.Count > maxLines) {
				lines2.RemoveAt (0);
			}
			foreach (string line in lines2) {
				inst.t2.text += line + "\n";
			}
		}
	}

}