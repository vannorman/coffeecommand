using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC : MonoBehaviour {

	public PlaneAnchorManager planeAnchorManagerRef;
	public static PlaneAnchorManager planeAnchorManager;
	public GameObject planePrefab;
	public static DebugText debugText;
	public DebugText debugTextRef;
	public Crosshair crosshairRef;
	public static Crosshair crosshair;
	public FeaturesVisualizer featuresVisualizerRef;
	public static FeaturesVisualizer featuresVisualizer;
	public CCFeaturesManager featuresManagerRef;
	public static CCFeaturesManager featuresManager;

	public static ShipWeapons shipWeapons;
	public ShipWeapons shipWeaponsRef;
	// Use this for initialization
	void Start () {
		planeAnchorManager = planeAnchorManagerRef;
		debugText = debugTextRef;
		shipWeapons = shipWeaponsRef;
		crosshair = crosshairRef;
		featuresVisualizer = featuresVisualizerRef;
		featuresManager = featuresManagerRef;
		UnityEngine.XR.iOS.UnityARUtility.InitializePlanePrefab (planePrefab);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
