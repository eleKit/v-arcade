using UnityEngine;
using System.Collections;

public class LeftHandControllerScript : MonoBehaviour {
	
	private float xExt, yExt;
	
	// Update is called once per frame
	void Update () {
		xExt = GameObject.Find ("leftHand").GetComponent<LeftRotationScript> ().GetXExtension();
		yExt = GameObject.Find ("leftHand").GetComponent<LeftRotationScript> ().GetYExtension();
	}

	public float GetXExt(){
		return xExt;
	}
	
	public float GetYExt(){
		return yExt;
	}
}
