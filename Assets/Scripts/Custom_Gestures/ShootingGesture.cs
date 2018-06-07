using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.Linq;

public class ShootingGesture : MonoBehaviour
{

	/* Shooting Gesture: a sliding movement made by all the fingers and the palm 
	 * The wrist is not moved and the palm goes left and right or up and down
	 * only the angle made by the hand movement is checked
	 * 
	 * N.B. it can be done one gesture per second
	 */

	/* used average method seen in Leap Motion Forum topic: custom gesture cration guide
	* https://forums.leapmotion.com/t/creating-my-own-gesture/603/7
	*/

	/* the hand should return around the original position after a push gesture, 
	* no more gestures are accepted in case this offset condition is not respected
	*/

	[Range (-5f, 0f)]
	public float pitch_offset = -0.2f;

	[Range (-5f, 0f)]
	public float yaw_offset = -0.2f;


	// In order to recognize the gesture a minimum angle should be done by the hand movement
	[Range (-30f, 0f)]
	public float pitch_threshold = -15f;

	[Range (-30f, 0f)]
	public float yaw_threshold = -15f;




	//no more than K previous frames are taken into account
	[Range (10, 100)]
	public int K_pitch = 10;

	[Range (10, 100)]
	public int K_yaw = 10;


	[Range (0, 20)]
	public int num_frames_in_pitch_average_list = 6;

	[Range (0, 20)]
	public int num_frames_in_yaw_average_list = 6;


	[Range (0, 10)]
	public float x_movement = 1;

	[Range (0, 10)]
	public float y_movement = 1;

	public HandController hc;



	private LinkedList<float> pitch_list = new LinkedList<float> ();
	private LinkedList<float> pitch_average = new LinkedList<float> ();

	private LinkedList<float> yaw_list = new LinkedList<float> ();
	private LinkedList<float> yaw_average = new LinkedList<float> ();


	//# min frames between a gesture recongnized and the next
	const int N = 60;


	int frames_since_last_gesture;

	//TODO get this from tuning as the zero position;
	float pitch_tuning_offset = -Mathf.Deg2Rad * 10f;
	float yaw_tuning_offset = 0;

	// Use this for initialization
	void Start ()
	{
		frames_since_last_gesture = N;
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (hc.GetFixedFrame ().Hands.Count == 1) {

			//change pointer colour
			if (gameObject.GetComponent<SpriteRenderer> ().color.Equals (Color.black)) {
				gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
			}

			//save average list
			if (pitch_average.Count >= num_frames_in_pitch_average_list) {
				pitch_average.RemoveFirst ();
			}

			if (yaw_average.Count >= num_frames_in_yaw_average_list) {
				yaw_average.RemoveFirst ();
			}

			pitch_average.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch + pitch_tuning_offset);
			yaw_average.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw + yaw_tuning_offset);


			//check gestures if lists are full of hand angle data and if a gesture has not been done just before this update
			if (pitch_list.Count >= K_pitch && yaw_list.Count >= K_yaw && frames_since_last_gesture >= N) {
				CheckShootGesture ();
			} else {
				frames_since_last_gesture++;
			}

			//save new data
			if (pitch_list.Count >= K_pitch)
				pitch_list.RemoveFirst ();
			if (yaw_list.Count >= K_yaw)
				yaw_list.RemoveFirst ();
			
			pitch_list.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch + pitch_tuning_offset);
			yaw_list.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw + yaw_tuning_offset);

		} else {
			//if no hand is visible change colour in black
			gameObject.GetComponent<SpriteRenderer> ().color = Color.black;
		}
	}




	void CheckShootGesture ()
	{


		float max_pitch = pitch_list.Max ();
		float min_pitch = pitch_list.Min ();

		float current_pitch = pitch_average.Average ();

		float max_yaw = yaw_list.Max ();
		float min_yaw = yaw_list.Min ();

		float current_yaw = yaw_average.Average ();


		//down gesture --> see yaw description
		if ((current_pitch - max_pitch) < Mathf.Deg2Rad * pitch_threshold && current_pitch < pitch_offset) {

			frames_since_last_gesture = 0;

			Vector3 new_position = transform.position - new Vector3 (0, y_movement, 0);

			transform.position = new_position;


		} else if ((current_pitch - min_pitch) > Mathf.Deg2Rad * (-pitch_threshold) && current_pitch > (-pitch_offset)) {

			frames_since_last_gesture = 0;

			Vector3 new_position = transform.position + new Vector3 (0, y_movement, 0);

			transform.position = new_position;


		} else if ((current_yaw - max_yaw) < Mathf.Deg2Rad * yaw_threshold && current_yaw < yaw_offset) {

			frames_since_last_gesture = 0;

			Vector3 new_position = transform.position - new Vector3 (x_movement, 0, 0);

			transform.position = new_position;


		} else if ((current_yaw - min_yaw) > Mathf.Deg2Rad * (-yaw_threshold) && current_yaw > (-yaw_offset)) {

			frames_since_last_gesture = 0;

			Vector3 new_position = transform.position + new Vector3 (x_movement, 0, 0);

			transform.position = new_position;


		}
	}


}
