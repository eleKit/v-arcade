using UnityEngine;
using System.Collections;
using System;

public class RightAnglesRetrieveScript : MonoBehaviour {
	
	private float xExt, yExt, zExt;
	
	// Update is called once per frame
	void Update () {

		xExt = GameObject.Find ("HandController").GetComponent<RightXRotationStatsScript> ().GetXExtension();

		yExt = GameObject.Find ("HandController").GetComponent<RightYRotationStatsScript> ().GetYExtension();

		zExt = GameObject.Find ("HandController").GetComponent<RightZRotationStatsScript> ().GetZExtension();

	}
	
	public float GetXExt(){
		return xExt;
	}
	
	public float GetYExt(){
		return yExt;
	}

	public float GetZExt(){
		return zExt;
	}
}
