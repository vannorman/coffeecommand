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
	public CoffeeCommandFeaturesVisualizer featuresVisualizerRef;
	public static CoffeeCommandFeaturesVisualizer featuresVisualizer;
	public OnionLocationHelper onionLocationHelperRef;
	public static OnionLocationHelper onionLocationHelper;

	public static ShipWeapons shipWeapons;
	public ShipWeapons shipWeaponsRef;
	// Use this for initialization
	void Start () {
		planeAnchorManager = planeAnchorManagerRef;
		debugText = debugTextRef;
		shipWeapons = shipWeaponsRef;
		crosshair = crosshairRef;
		featuresVisualizer = featuresVisualizerRef;
		onionLocationHelper = onionLocationHelperRef;
	
//		UnityEngine.XR.iOS.ARPlaneAnchor.InitializePlanePrefab (planePrefab);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
