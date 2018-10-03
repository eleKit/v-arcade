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




	private HandController hc;

	private Vector3 verticalAcc;



	// Use this for initialization
	public void RollStart (HandController handController)
	{
		


		Physics2D.gravity = Vector3.zero;

		hc = handController;

		verticalAcc = new Vector3 (0f, gravityValue, 0f);
	}
	
	// Update is called once per frame
	public void RollFixedUpdate ()
	{
		//controllo che il bambino giochi con una mano
		if (hc.GetFixedFrame ().Hands.Count == 1 && CarManager.Instance.GetIsPlaying ()) {
			float roll = hc.GetFixedFrame ().Hands.Leftmost.PalmNormal.Roll;
			float pitch = hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch;
			float yaw = hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw; 

			Vector3 horizontalAcc = Vector3.zero;
			Vector3 res = Vector3.zero;


			horizontalAcc = new Vector3 (scale * Mathf.Sin (-roll), 0, 0);
			res = (horizontalAcc + verticalAcc);
			

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
