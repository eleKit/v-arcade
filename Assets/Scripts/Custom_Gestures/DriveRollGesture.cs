using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DriveRollGesture : MonoBehaviour
{

	/* Roll gesture 180°: a roll movement made by all the fingers and the palm,
	 * substitutes the old plane controller gesture. 
	 * The wrist is moved and the palm rolls down and up
	 */



	//car speed
	[Range (0f, 30f)]
	public float speed = 10f;



	//no more than K previous frames are taken into account
	[Range (10, 50)]
	public int K = 10;


	[Range (0, 10)]
	public int num_frames_in_average_list = 6;

	[Range (0, 10)]
	public float x_movement = 1;

	private HandController hc;

	private LinkedList<float> roll_average = new LinkedList<float> ();

	/* THRESHOLD: In roder to recognize the gesture a minimum angle should be done by the hand movement in RAD
	 * since the hands have different abilities each hand has it own threshold and 
	 * before checking the gesture it is checked what hand is in use
	 */


	private float roll_left_threshold = -0.05f;
	private float roll_right_threshold = -0.05f;


	//The spaceship has a minimum position and a maximum one, it must not escape from the screen
	private float x_max_player_position = 11.5f;
	private float x_min_player_position = -11.5f;






	// Use this for initialization
	public void RollStart (HandController handController)
	{
		hc = handController;

	}
	
	// Update is called once per frame
	public void RollUpdate ()
	{
		var current_frame = GameManager.Instance.GetCurrentFrame ();
		if (current_frame.Hands.Count == 1) {

			transform.Translate (Vector3.up * Time.deltaTime * 6f);





			if (roll_average.Count >= num_frames_in_average_list) {
				roll_average.RemoveFirst ();
			}
			roll_average.AddLast (current_frame.Hands.Leftmost.PalmNormal.Roll);


			CheckRollGesture (current_frame.Hands.Leftmost.IsLeft);


		}
	}


	void CheckRollGesture (bool is_left)
	{

		float threshold;

		if (is_left) {
			threshold = roll_left_threshold;
		} else {
			threshold = roll_right_threshold;
		}
		float current_roll = roll_average.Average ();


		if (current_roll < threshold) {


			if ((transform.position.x + (Vector3.right * Time.deltaTime * speed).x) <= x_max_player_position) {
				transform.Translate (Vector3.right * Time.deltaTime * speed);
			}


		} else if (current_roll > (-threshold)) {


			if ((transform.position.x + (Vector3.left * Time.deltaTime * speed).x) >= x_min_player_position) {

				transform.Translate (Vector3.left * Time.deltaTime * speed);
			}


		}


	}





}
