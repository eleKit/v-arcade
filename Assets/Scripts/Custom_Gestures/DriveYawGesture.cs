using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.Linq;

public class DriveYawGesture : MonoBehaviour
{
	/* Drive gesture: a sliding movement made by all the fingers and the palm,
	 * substitutes the old slide gesture. 
	 * The wrist is not moved and the palm goes left and right or up and down
	 * only the angle made by the hand movement is checked
	 */

	/* used average method seen in Leap Motion Forum topic: custom gesture cration guide
	 * https://forums.leapmotion.com/t/creating-my-own-gesture/603/7
	 */

	/* the hand should return around the original position after a gesture, 
	* no more gestures are accepted in case this offset condition is not respected
	*/
	[Range (-5f, 0f)]
	public float offset = -0.2f;




	[Range (0f, 30f)]
	public float speed = 10f;



	[Range (-5f, 0f)]
	public float pitch_offset = -0.2f;


	// In roder to recognize the gesture a minimum angle should be done by the hand movement


	//no more than K previous frames are taken into account
	[Range (10, 50)]
	public int K = 10;


	[Range (0, 10)]
	public int num_frames_in_average_list = 6;

	[Range (0, 10)]
	public float x_movement = 1;

	public HandController hc;

	bool yaw;

	private LinkedList<float> yaw_average = new LinkedList<float> ();


	private LinkedList<float> pitch_average = new LinkedList<float> ();

	/* THRESHOLD: In roder to recognize the gesture a minimum angle should be done by the hand movement in RAD
	 * since the hands have different abilities each hand has it own threshold and 
	 * before checking the gesture it is checked what hand is in use
	 */
	private float yaw_left_threshold = -15f;
	private float yaw_right_threshold = -15f;

	private float pitch_left_threshold = -15f;
	private float pitch_right_threshold = -15f;




	// Use this for initialization
	void Start ()
	{
		yaw = false;
		yaw_left_threshold = -GlobalPlayerData.globalPlayerData.player_data.left_yaw_scale;
		yaw_right_threshold = -GlobalPlayerData.globalPlayerData.player_data.right_yaw_scale;

		pitch_left_threshold = -GlobalPlayerData.globalPlayerData.player_data.left_pitch_scale;
		pitch_right_threshold = -GlobalPlayerData.globalPlayerData.player_data.right_pitch_scale;
	
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (hc.GetFixedFrame ().Hands.Count == 1) {

			transform.position = transform.position + new Vector3 (0f, 0.1f, 0f);


			if (yaw) {
				if (yaw_average.Count >= num_frames_in_average_list) {
					yaw_average.RemoveFirst ();
				}

				yaw_average.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw);

				/*if this is a right hand the bool is false and in the CheckYawDriveGesture (left)
				 * it is used the right_yaw_threshold 
				 */
				CheckYawDriveGesture (hc.GetFixedFrame ().Hands.Leftmost.IsLeft);

				

			} else {
				

				if (pitch_average.Count >= num_frames_in_average_list) {
					pitch_average.RemoveFirst ();
				}
				pitch_average.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);

				/*if this is a right hand the bool is false and in the CheckYawDriveGesture (left)
				 * it is used the right_yaw_threshold 
				 */
				CheckPitchPushGesture (hc.GetFixedFrame ().Hands.Leftmost.IsLeft);

			}
		}

	}








	void CheckYawDriveGesture (bool is_left)
	{
		float threshold;

		if (is_left) {
			threshold = yaw_left_threshold;
		} else {
			threshold = yaw_right_threshold;
		}
			
		float current_yaw = yaw_average.Average ();

		/* if the hand in moved in the left direction the max yaw must be the around zero position
		 * and if the (current_yaw - max_yaw) < threshold the gesture is recognized
		 */
		if (current_yaw < threshold && current_yaw < offset) {



			transform.Translate (Vector3.left * Time.deltaTime * speed);



		} else if (current_yaw > (-threshold) && current_yaw > (-offset)) {



			transform.Translate (Vector3.right * Time.deltaTime * speed);



		}

		/* otherwise if the hand is moved in the right direction the min yaw must be around the zero position
		 * and if the current_yaw - min_yaw) > threshold the gesture is recognized
		 */


	}




	void CheckPitchPushGesture (bool is_left)
	{

		float threshold;

		if (is_left) {
			threshold = yaw_left_threshold;
		} else {
			threshold = yaw_right_threshold;
		}
		float current_pitch = pitch_average.Average ();

		/* if the hand in moved in the left direction the max yaw must be the around zero position
		 * and if the (current_yaw - max_yaw) < threshold the gesture is recognized
		 */
		if (current_pitch < threshold && current_pitch < offset) {



			transform.Translate (Vector3.right * Time.deltaTime * speed);


		} else if (current_pitch > (-threshold) && current_pitch > (-offset)) {



			transform.Translate (Vector3.left * Time.deltaTime * speed);


		}

		
	}



	public void TrueYawBool ()
	{
		yaw = true;
	}

	public void FalseYawBool ()
	{
		yaw = false;
	}
}
