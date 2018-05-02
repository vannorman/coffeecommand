using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalOnion : MonoBehaviour {

	// appears randomly spread out throughout the scene on start
	// float around until player gets "near" to one, then it floats to the nearest "surface" until brown  dots > 50
	// now ready to be "unrwapped"
	// player grabs one of the tendrils "grab" ui
	// player moves phone in correct direction dragging the UI radial fill around, which starts as gray and slowly fades to green as the sweep is completed.
	// hatpics doot doot as you drag
	// After a sweep is completed, some success sounds, particles, another tendril pops out, maybe 3 total will pop
	// after 3rd tendril drag is completed, the onion plants!
	public TextMesh debugText ;
	public enum State {
		Floating,
		Ready,
		Unwrapping,
		Unwrapped
	}

	public GameObject oilDerrick;
	public GameObject onionGraphics;
	public GameObject face;


	public DishGroup dishGroup;

	public State state;


	public Image unwrapIndicator;
	float targetFillAmount = 0;


//	public Tendril tendril; // the initial tendril is "deactivated" and will serve as a prefab for additional tendrils
	List<Tendril> tendrils = new List<Tendril>();
	Vector3 startPos;

	void Start(){
		startPos = transform.position;
		InitBrownianPoints ();
		unwrapIndicator.fillAmount = 0;
		oilDerrick.SetActive (false);
//		dishGroup.gameObject.SetActive(false);

		DebugText.SetOnionCount(FindObjectsOfType<MetalOnion>().Length.ToString());
		tendrils.AddRange(GetComponentsInChildren<Tendril> ());
	}

	public void SetState(State newState){
		state = newState;
		DebugText.SetOnionState (state.ToString());
		face.SetActive (false);
		onionGraphics.SetActive (false);
		oilDerrick.SetActive (false);
		dishGroup.gameObject.SetActive(false);
		switch (state) {
		case State.Floating:
			face.SetActive (true);
			onionGraphics.SetActive (true);
			break;
		case State.Unwrapping:
			onionGraphics.SetActive (true);
			dishGroup.gameObject.SetActive (true);
			break;
		case State.Unwrapped: 
			oilDerrick.SetActive (true);
			transform.rotation = Quaternion.identity;
			break;
		default:
			break;
		}

	}

	float cameraHoverTimer = 0f;
	float cameraHoverThreshhold  = 1.5f; // after this passed, tendril will lock out and allow rotation.
	bool cameraHovering = true; // for initializeation of camhovercountdown in update if camhovering false
	public void CameraHovering(){
//		Debug.Log ("camhov:" + cameraHoverTimer);
		cameraHoverTimer = 1.2f;
	}


	float camHoverCountdown = 0;
	Quaternion targetRot;


	public void OnDishGroupDestroyed(){
		
		dishGroup.gameObject.SetActive (false);
		SetState (State.Unwrapped);

	}

	Vector3 targetPos;

	enum MovementState {
		Brownian,
		SeekPoints,
		AwaitingDestruction,
		DestroyedAndFalling,
		Stationary
	}
	float movementTime = 0f;
	MovementState movementState = MovementState.Brownian;


	void SeekPointClusters() {
		if (CC.onionLocationHelper.FoundTargetNearOnion (this)) {
			targetPos = CC.onionLocationHelper.TargetFeatureCluster ();
			debugText.text = "t:" + targetPos;
			float moveSpeed = 1.5f;
			MoveTowardsTarget ();
		} else {
			debugText.text = "t: random";

			MoveRandomly ();
		}
	}

	void MoveTowardsTarget(float moveSpeed = 1f){
		Vector3 dirToTarget = (targetPos - transform.position).normalized;
		movementDir = Vector3.Lerp (movementDir, dirToTarget, Time.deltaTime); // smoothing
		transform.position += movementDir * Time.deltaTime;
//		transform.position = Vector3.MoveTowards (transform.position, targetPos, Time.deltaTime * moveSpeed);
	}

	public Transform panelsParent;
	bool AllPanelsDestroyed {
		get {
			if (panelsParent.childCount < 35) {
				foreach (Transform t in panelsParent.transform) {
					Destroy (t.gameObject);
				}
			}
			return panelsParent.childCount == 0;
		}
	}


	public GameObject smokeTrail;
	void SetMovementState(MovementState newState){
		Debug.Log ("<color=red><b> move state;</b></color>" + newState);
		movementState = newState;
		switch (newState) {
		case MovementState.DestroyedAndFalling:
			
			smokeTrail.SetActive (true);

			break;
		case MovementState.Stationary:
			smokeTrail.SetActive (false);
			break;
	
			
		}

	}


	float timeToMoveBrownian = 10f;
	float brownianRadius = .3f;
	float timeToSeekPoints = 10f;
	int brownianIndex = 0;
	List<Vector3> brownianPoints = new List<Vector3>();
	void InitBrownianPoints(){
		brownianPoints.Add (startPos + Vector3.right * brownianRadius);
		brownianPoints.Add (startPos - Vector3.right * brownianRadius);
		brownianPoints.Add (startPos + Vector3.forward * brownianRadius);
		brownianPoints.Add (startPos - Vector3.forward * brownianRadius);
		string s = "";
		foreach (Vector3 p in brownianPoints) {
			s += p + ", ";
		}
		Debug.Log ("p:" + s);
//		for(int i

	}



	bool NearToTarget {
		
		get {
			return (transform.position - targetPos).magnitude < .05f;
		}
	}

	float autoDestructTimer = 4f; // finished mapping but player didn't destroy it!! so auto destroy after time.


	bool BrownianComplete = false;
	void MoveBrownian(){
		SetTarget (brownianPoints [brownianIndex]);
		MoveTowardsTarget ();
		if (Vector3.Magnitude (transform.position - brownianPoints [brownianIndex]) < 0.1f){
			if (brownianIndex < brownianPoints.Count - 1) {
				brownianIndex++;
			} else {
				BrownianComplete = true;
			}
		}
	}

	// Update is called once per frame
	void Update () {

		#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.L)) {
			CameraHovering ();
		}
		#endif

		cameraHoverTimer -= Time.deltaTime;
		if (cameraHoverTimer > 0) {
			cameraHovering = true; 
		} else {
			cameraHovering = false;
		}
		float fillSpeed = 0.2f;
		float rotSpeed = 2f;

//		if (cameraHovering) DebugText.SetCamHoverObj ("onion:" + this.name + " at:" + Time.time);


		switch(state){
		case State.Floating:
			if (cameraHovering) {
				movementTime += Time.deltaTime;
			}
			switch (movementState) {
			
			case MovementState.Brownian:
				
				if (cameraHovering) {
					MoveBrownian ();

//					MoveRandomly ();
				}
				if (BrownianComplete) {
					SetMovementState (MovementState.SeekPoints);
					movementTime = 0;	
				}
				break;
			case MovementState.SeekPoints:
				if (cameraHovering) SeekPointClusters ();
				if (movementTime > timeToSeekPoints) {
					SetMovementState (MovementState.AwaitingDestruction);
					movementTime = 0;
				}
				break;
			case MovementState.AwaitingDestruction:
//				Debug.Log ("await dest");
				if (AllPanelsDestroyed) {
					SetMovementState (MovementState.DestroyedAndFalling);
				} else {
					MoveRandomly (0.1f);
					autoDestructTimer -= Time.deltaTime;
					if (autoDestructTimer < 0) {
						SetMovementState (MovementState.DestroyedAndFalling);
					}
				}
				break;
			case MovementState.DestroyedAndFalling:
				GetPlaneWithHighestPointScore ();
				MoveTowardsTarget ();
				if (NearToTarget) {
					SetMovementState (MovementState.Stationary);
				}
				break;
			case MovementState.Stationary:
				SetState (State.Ready);
				break;
			default:
				break;
			}
			
		
			
			break;
		case State.Ready:
			if (cameraHovering) {
				camHoverCountdown -= Time.deltaTime;
				//				Debug.Log ("camhover count:" + camHoverCountdown);
				if (camHoverCountdown < 0) {
//					nearest.GetComponent<PlaneInfo> ().plantedOnion = this;
					SetState (State.Unwrapping);
				}


			} else {
				foreach (Tendril t in tendrils) {
					//				Debug.Log ("popin");
					t.PopIn (); //SetState (Tendril.State.Out);
				}
				targetFillAmount = 0;
			}

//			DebugText.Overflow
			unwrapIndicator.fillAmount = Mathf.MoveTowards (unwrapIndicator.fillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRot, Time.deltaTime * rotSpeed);
			// we are at a plane and ready for unwrap
			break;
		case State.Unwrapping:

			// new, turret pop and turn randomly

			// old, easy unwrapping
//			if (cameraHovering) {
//				foreach (Tendril t in tendrils) {
//					//				Debug.Log ("popin");
//					t.PopOut (); //SetState (Tendril.State.Out);
//				}
//				
//				targetRot = Quaternion.LookRotation (Camera.main.transform.position - transform.position);
//				targetFillAmount = 1;
//				if (unwrapIndicator.fillAmount > 0.99) {
//					unwrapIndicator.fillAmount = 1;
//					SetState (State.Unwrapped);
//					GetComponent<AudioSource> ().Play ();
//				}
//
//				unwrapIndicator.fillAmount = Mathf.MoveTowards (unwrapIndicator.fillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
//
//				transform.rotation = Quaternion.Slerp (transform.rotation, targetRot, Time.deltaTime * rotSpeed);
//
//			} else {
//				SetState (State.Ready);
//			}
			break;
		case State.Unwrapped:
			break;
		default:
			break;

		}


	}

	void GetPlaneWithHighestPointScore(){
	
		GameObject nearest = CC.onionLocationHelper.GetNearestPlane (this.transform, 1.5f);
		if (nearest)
			targetPos = nearest.transform.position;
	}

//	void Move(float moveSpeed = 1f){
//		if (nearest) {
//			transform.position = Vector3.MoveTowards (transform.position, nearest.transform.position, Time.deltaTime * moveSpeed);
//		}
//
//	}
//
//	void StoppedUnwrapping(){
//		// user failed to complete an unwrap
//		cameraHovering = false;
//		targetFillAmount = 0;
//		camHoverCountdown = cameraHoverThreshhold;
//		targetRot = Quaternion.Euler (Random.onUnitSphere);
//		SetState (State.Ready);
//
//	}


	Vector3 GetRandomPos {

		get {
			float randomRadius = 1.2f;
			Vector3 rp = Vector3.zero;
			for (int i = 0; i < 20; i++) {
				rp = startPos + Random.insideUnitSphere * randomRadius;
				if (Vector3.Magnitude (startPos - Camera.main.transform.position) > .35f) {
					return rp;
				}
			}
			return rp;
		}
	}

	float lastRandomDirectionTime = 0;
	Vector3 randomDir = Vector3.right;
	Vector3 movementDir = Vector3.zero;
	Vector3 randomPos = Vector3.zero;
	void MoveRandomly(float randomMovementSpeed = 1f){
		float randomDirectionInterval = 3f;
		if (Time.time - lastRandomDirectionTime > randomDirectionInterval) {
			lastRandomDirectionTime = Time.time;
			randomPos = GetRandomPos; 
		}
		SetTarget (startPos + randomPos);

//		targetPos = startPos + randomPos;
		MoveTowardsTarget ();


//		float randomDirectionInterval = 3f;
//		if (Time.time - lastRandomDirectionTime > randomDirectionInterval) {
//			lastRandomDirectionTime = Time.time;
//			randomDir = new Vector3 (Random.Range (-1, 1f), Random.Range (-0.4f, 0.4f),Random.Range (-1, 1f));
//		}
//		movementDir = Vector3.Lerp (movementDir, randomDir, Time.deltaTime * 1f); //smooth
////		Debug.Log ("rand:" + randomDir.magnitude + ", movedir;" + movementDir.magnitude);
//		transform.position += movementDir * Time.deltaTime * randomMovementSpeed;
	}

	void SetTarget(Vector3 p){
		targetPos = p;
	}
}
