using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarControllerScript : MonoBehaviour
{
	/* 180° */

	[Range (0f, 9.81f)]
	public float gravityValue = 9.81f;

	[Range (0f, 50f)]
	public float scale = 1f;

	public Text angle;


	public GameObject handController;

	private HandController hc;

	private Vector3 verticalAcc;


	// Use this for initialization
	void Start ()
	{
		Physics2D.gravity = Vector3.zero;

		hc = handController.GetComponent<HandController> ();

		verticalAcc = new Vector3 (0f, gravityValue, 0f);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		//controllo che il bambino giochi con una mano
		if (hc.GetFrame ().Hands.Count == 1) {
			float roll = hc.GetFixedFrame ().Hands.Leftmost.PalmNormal.Roll;
			float pitch = hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch;
			float yaw = hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw; 
			angle.text = "Roll: " + (Mathf.Rad2Deg * roll).ToString () + "\n" +
			"Pitch: " + (Mathf.Rad2Deg * pitch).ToString () + "\n" +
			"Yaw: " + (Mathf.Rad2Deg * yaw).ToString ();

			Vector3 horizontalAcc = new Vector3 (scale * Mathf.Sin (-roll), 0, 0);
			Vector3 res = (horizontalAcc + verticalAcc);

			Physics2D.gravity = res.normalized * gravityValue;
			
		} else {
			Physics2D.gravity = Vector3.zero;
			
		}
	}
}
