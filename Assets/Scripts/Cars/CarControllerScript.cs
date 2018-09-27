using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarControllerScript : MonoBehaviour
{
	//DEPRECATED!!

	/* Roll gesture 180°: a roll movement made by all the fingers and the palm,
	 * substitutes the old plane controller gesture. 
	 * The wrist is moved and the palm rolls down and up
	 */



	/* TODO Slap gesture 90°: a slap movement made by all the fingers and the palm,
	 * substitutes the old slap gesture.
	 * The wrist is not moved and the palm goes left and right.
	 */

	[Range (0f, 19.62f)]
	public float gravityValue = 9.81f;

	[Range (0f, 100f)]
	public float scale = 1f;



	public GameObject handController;

	private HandController hc;

	private Vector3 verticalAcc;

	bool ninety_deg_hand, one_hundred_and_eighty_hand, roll_hand;


	// Use this for initialization
	void Start ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;
		roll_hand = false;


		Physics2D.gravity = Vector3.zero;

		hc = handController.GetComponent<HandController> ();

		verticalAcc = new Vector3 (0f, gravityValue, 0f);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		//controllo che il bambino giochi con una mano
		if (hc.GetFixedFrame ().Hands.Count == 1 && CarManager.Instance.GetIsPlaying ()) {
			float roll = hc.GetFixedFrame ().Hands.Leftmost.PalmNormal.Roll;
			float pitch = hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch;
			float yaw = hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw; 

			Vector3 horizontalAcc = Vector3.zero;
			Vector3 res = Vector3.zero;

			if (roll_hand) {
				horizontalAcc = new Vector3 (scale * Mathf.Sin (-roll), 0, 0);
				res = (horizontalAcc + verticalAcc);
			} else if (one_hundred_and_eighty_hand) {
				horizontalAcc = new Vector3 (scale * Mathf.Sin (yaw), 0, 0);
				res = (horizontalAcc + verticalAcc);
			} else if (ninety_deg_hand) {

				//TODO fix pich doesnt work!!
				horizontalAcc = new Vector3 (scale * Mathf.Sin (pitch), 0, 0);
				res = (horizontalAcc + verticalAcc);

			}

			Physics2D.gravity = res.normalized * gravityValue;
			
		} else {
			Physics2D.gravity = Vector3.zero;
			
		}
	}

	public void SetGravityToZero ()
	{
		Physics2D.gravity = Vector3.zero;
	}


	public void NinetyTrue ()
	{
		ninety_deg_hand = true;
		one_hundred_and_eighty_hand = false;
		roll_hand = false;
	}

	public void OneHundredEightyTrue ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = true;
		roll_hand = false;
	}

	public void RollTrue ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;
		roll_hand = true;
	}


}
