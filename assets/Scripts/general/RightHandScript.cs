using UnityEngine;
using System.Collections;

public class RightHandScript : MonoBehaviour {

	private Vector3 startAngles;
	
	// Use this for initialization
	void Start () {
		startAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 updatedAngles = new Vector3(startAngles.x + GameObject.Find("rightHand").GetComponent<RightRotationScript>().GetXExtension(), startAngles.y, startAngles.z);
		transform.eulerAngles = updatedAngles;
	}
}
