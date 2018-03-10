using UnityEngine;
using System.Collections;

public class RightHandControllerScript : MonoBehaviour {
	
	private float xExt, yExt;

	// Update is called once per frame
	void Update () {
		xExt = GameObject.Find ("rightHand").GetComponent<RightRotationScript> ().GetXExtension();
		yExt = GameObject.Find ("rightHand").GetComponent<RightRotationScript> ().GetYExtension();
	}

	public float GetXExt(){
		return xExt;
	}

	public float GetYExt(){
		return yExt;
	}
}
