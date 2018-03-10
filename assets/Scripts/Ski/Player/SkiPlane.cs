using UnityEngine;
using System.Collections;

public class SkiPlane : MonoBehaviour {

	public GameObject firstTreeGenerator, secondTreeGenerator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void CreateFirstFlags (float pos) {
		firstTreeGenerator.SendMessage ("CreateFlags", pos);
	}

	public void CreateSecondFlags(float pos){
		secondTreeGenerator.SendMessage ("CreateFlags", pos);
	}
}
