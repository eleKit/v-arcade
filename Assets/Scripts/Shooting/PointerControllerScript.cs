using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerControllerScript : MonoBehaviour
{

	/*bool used to determine the 90° or 180° mode
	 * it is true when the game is played with 90° roll of the hand
	 */

	//TODO

	public bool half_pi = false;

	[Range (0f, 100f)]
	public float m_horizontal_scale = 1f;

	[Range (0f, 15f)]
	public float m_vertical_scale = 1f;

	[Range (-50f, 50f)]
	public float pitchOffset = 9f;

	public GameObject handController;

	private HandController hc;


	private Vector3 verticalAcc;

	private Rigidbody2D pointer;


	// Use this for initialization
	void Start ()
	{

		hc = handController.GetComponent<HandController> ();

		pointer = GetComponent<Rigidbody2D> ();

		//the pointer is not affected by Gravity
		pointer.gravityScale = 0;

		pointer.drag = 5f;

		verticalAcc = new Vector3 (0f, 0f, 0f);

		pitchOffset = (pitchOffset * Mathf.Deg2Rad);
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (hc.GetFrame ().Hands.Count == 1) {

			GetComponent<SpriteRenderer> ().color = Color.white;


			float roll = hc.GetFixedFrame ().Hands.Leftmost.PalmNormal.Roll;
			float pitch = hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch + pitchOffset;
			float yaw = hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw; 

			Vector3 horizontalAcc = new Vector3 (m_horizontal_scale * Mathf.Sin (yaw), 0, 0);
			Vector3 verticalAcc = new Vector3 (0, m_vertical_scale * Mathf.Sin (pitch), 0);

		

			Vector3 res = (horizontalAcc + verticalAcc);

			pointer.AddForce (res);
		} else {

			pointer.AddForce (Vector3.zero);
			GetComponent<SpriteRenderer> ().color = Color.black;
		}

	


		
	}
}
