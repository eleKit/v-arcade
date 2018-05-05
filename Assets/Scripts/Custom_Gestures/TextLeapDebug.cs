using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLeapDebug : MonoBehaviour
{

	public Text tx;

	public GameObject handController;

	private HandController hc;


	// Use this for initialization
	void Start ()
	{
		hc = handController.GetComponent<HandController> ();
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{

		float roll = hc.GetFixedFrame ().Hands.Leftmost.PalmNormal.Roll;
		float pitch = hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch;
		float yaw = hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw; 

		tx.text = "pitch: " + (pitch * Mathf.Rad2Deg).ToString () + "°\n"
		+ "yaw: " + (yaw * Mathf.Rad2Deg).ToString () + "°\n"
		+ "roll: " + (roll * Mathf.Rad2Deg).ToString () + "°\n";
		
	}
}
