using UnityEngine;
using System.Collections;
using System;

public class LeftAnglesRetrieveScript : MonoBehaviour {
	
	private float xExt, yExt, zExt;

	// Update is called once per frame
	void Update () {

		xExt = GameObject.Find("MyHandController").GetComponent<LeftXRotationStatsScript> ().GetXExtension();

		yExt = GameObject.Find("MyHandController").GetComponent<LeftYRotationStatsScript> ().GetYExtension();

		zExt = GameObject.Find("MyHandController").GetComponent<LeftZRotationStatsScript> ().GetZExtension();

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
