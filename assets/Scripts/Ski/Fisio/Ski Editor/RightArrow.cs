using UnityEngine;
using System.Collections;

public class RightArrow : MonoBehaviour {

	GameObject par;
	
	// Use this for initialization
	void Start () {
		par = transform.parent.gameObject;
	}
	
	void OnMouseDown(){
		par.SendMessage ("Increase");
	}
}
