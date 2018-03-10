using UnityEngine;
using System.Collections;
using System;

public class LeftAnglesRetrieveScript : MonoBehaviour {
	
	private float xExt, yExt, zExt;

	// Update is called once per frame
	void Update () {

		xExt = GameObject.Find ("HandController").GetComponent<LeftXRotationStatsScript> ().GetXExtension();

		yExt = GameObject.Find ("HandController").GetComponent<LeftYRotationStatsScript> ().GetYExtension();

		zExt = GameObject.Find ("HandController").GetComponent<LeftZRotationStatsScript> ().GetZExtension();

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
