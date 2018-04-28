using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarControllerScript : MonoBehaviour
{
	/* 180° */

	[Range (0f, 19.62f)]
	public float gravityValue = 9.81f;

	[Range (0f, 100f)]
	public float scale = 1f;

	/*bool used to determine the 90° or 180° mode
	 * it is true when the game is played with 90° roll of the hand
	 */

	//TODO

	public bool half_pi = false;



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

			Vector3 horizontalAcc = new Vector3 (scale * Mathf.Sin (-roll), 0, 0);
			Vector3 res = (horizontalAcc + verticalAcc);

			Physics2D.gravity = res.normalized * gravityValue;
			
		} else {
			Physics2D.gravity = Vector3.zero;
			
		}
	}

	public void SetGravityToZero ()
	{
		Physics2D.gravity = Vector3.zero;
	}
}
