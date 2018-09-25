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

	/* the hand should return around the original position after a push gesture, 
	* no more gestures are accepted in case this offset condition is not respected
	*/
	[Range (-5f, 0f)]
	public float offset = -0.2f;


	// In roder to recognize the gesture a minimum angle should be done by the hand movement DEG
	[Range (-30f, 0f)]
	public float threshold = -15f;



	[Range (-5f, 0f)]
	public float pitch_offset = -0.2f;


	// In roder to recognize the gesture a minimum angle should be done by the hand movement
	[Range (-30f, 0f)]
	public float pitch_threshold = -15f;

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


	//public bool ninety_deg_hand, one_hundred_and_eighty_hand;

	const int N = 60;


	int frames_since_last_gesture;

	//TODO get this fro tuning as the zero position;
	float tuning_offset = -Mathf.Deg2Rad * 10f;


	// Use this for initialization
	void Start ()
	{
		frames_since_last_gesture = N;
		yaw = false;
		threshold = -GlobalPlayerData.globalPlayerData.player_data.yaw_scale * Mathf.Rad2Deg;
		Debug.Log ("New yaw scale" + threshold);
	
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

				if (frames_since_last_gesture >= N) {
					CheckYawDriveGesture ();
				} else {
					frames_since_last_gesture++;
				}
				

			} else {
				

				if (pitch_average.Count >= num_frames_in_average_list) {
					pitch_average.RemoveFirst ();
				}
				pitch_average.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch + tuning_offset);

				if (frames_since_last_gesture >= N) {
					CheckPitchPushGesture ();
				} else {
					frames_since_last_gesture++;
				}
			}
		}

	}








	void CheckYawDriveGesture ()
	{


		float current_yaw = yaw_average.Average ();

		/* if the hand in moved in the left direction the max yaw must be the around zero position
		 * and if the (current_yaw - max_yaw) < threshold the gesture is recognized
		 */
		if (current_yaw < Mathf.Deg2Rad * threshold && current_yaw < offset) {

			frames_since_last_gesture = 0;

			Vector3 new_position = transform.position - new Vector3 (x_movement, 0, 0);

			transform.position = new_position;


		} else if (current_yaw > Mathf.Deg2Rad * (-threshold) && current_yaw > (-offset)) {

			frames_since_last_gesture = 0;

			Vector3 new_position = transform.position + new Vector3 (x_movement, 0, 0);

			transform.position = new_position;


		}

		/* otherwise if the hand is moved in the right direction the min yaw must be around the zero position
		 * and if the current_yaw - min_yaw) > threshold the gesture is recognized
		 */


	}




	void CheckPitchPushGesture ()
	{



		float current_pitch = pitch_average.Average ();

		/* if the hand in moved in the left direction the max yaw must be the around zero position
		 * and if the (current_yaw - max_yaw) < threshold the gesture is recognized
		 */
		if (current_pitch < Mathf.Deg2Rad * threshold && current_pitch < offset) {

			frames_since_last_gesture = 0;

			Vector3 new_position = transform.position - new Vector3 (x_movement, 0, 0);

			transform.position = new_position;


		} else if (current_pitch > Mathf.Deg2Rad * (-threshold) && current_pitch > (-offset)) {

			frames_since_last_gesture = 0;

			Vector3 new_position = transform.position + new Vector3 (x_movement, 0, 0);

			transform.position = new_position;


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
