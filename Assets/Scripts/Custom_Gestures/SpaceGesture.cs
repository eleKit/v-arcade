using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.Linq;

public class SpaceGesture : MonoBehaviour
{

	/* Space Gesture: a sliding movement made by all the fingers and the palm 
	 * The wrist is not moved and the palm goes left and right (like the yaw part of the shooting gesture)
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
	public float yaw_offset = -0.2f;


	// In order to recognize the gesture a minimum angle should be done by the hand movement
	[Range (-30f, 0f)]
	public float yaw_threshold = -15f;

	//no more than K previous frames are taken into account
	[Range (10, 100)]
	public int K_yaw = 10;


	[Range (10, 100)]
	public int K_lost = 30;


	[Range (0, 20)]
	public int num_frames_in_yaw_average_list = 6;


	[Range (0, 10)]
	public float x_movement = 1;


	public HandController hc;

	private LinkedList<float> yaw_list = new LinkedList<float> ();
	private LinkedList<float> yaw_average = new LinkedList<float> ();


	//# min frames between a gesture recongnized and the next
	const int N = 60;


	int frames_since_last_reconnection;

	int frames_since_last_gesture;

	//TODO get this from tuning as the zero position;
	float yaw_tuning_offset = 0;

	float x_max_player_position = 15f;
	float x_min_player_posiion = -15f;

	// Use this for initialization
	void Start ()
	{
		frames_since_last_gesture = N;
		frames_since_last_reconnection = 0;
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (hc.GetFixedFrame ().Hands.Count == 1) {


			//change pointer colour
			if (gameObject.GetComponent<SpriteRenderer> ().color.Equals (Color.black)) {
				gameObject.GetComponent<SpriteRenderer> ().color = Color.grey;
			}

			frames_since_last_reconnection++;	


			//save average list

			if (yaw_average.Count >= num_frames_in_yaw_average_list) {
				yaw_average.RemoveFirst ();
			}

			yaw_average.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw + yaw_tuning_offset);


			//check gestures if lists are full of hand angle data and if a gesture has not been done just before this update
			if (yaw_list.Count >= K_yaw && frames_since_last_gesture >= N
			    && frames_since_last_reconnection >= K_lost) {
				CheckMoveSpaceshipGesture ();

				//change pointer colour
				if (gameObject.GetComponent<SpriteRenderer> ().color.Equals (Color.grey)) {
					gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
				}

			} else {
				frames_since_last_gesture++;
			}

			//save new data
			if (yaw_list.Count >= K_yaw)
				yaw_list.RemoveFirst ();

			yaw_list.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw + yaw_tuning_offset);

		} else {
			//if no hand is visible change colour in black
			gameObject.GetComponent<SpriteRenderer> ().color = Color.black;

			yaw_list.Clear ();
			yaw_average.Clear ();

			frames_since_last_reconnection = 0;
		}
	}




	void CheckMoveSpaceshipGesture ()
	{

		float max_yaw = yaw_list.Max ();
		float min_yaw = yaw_list.Min ();

		float current_yaw = yaw_average.Average ();


		if ((current_yaw - max_yaw) < Mathf.Deg2Rad * yaw_threshold && current_yaw < yaw_offset) {

			frames_since_last_gesture = 0;

			if ((transform.position.x - x_movement) >= x_min_player_posiion) {
				Vector3 new_position = transform.position - new Vector3 (x_movement, 0, 0);

				transform.position = new_position;
			}


		} else if ((current_yaw - min_yaw) > Mathf.Deg2Rad * (-yaw_threshold) && current_yaw > (-yaw_offset)) {

			frames_since_last_gesture = 0;

			if ((transform.position.x - x_movement) <= x_max_player_position) {
				Vector3 new_position = transform.position + new Vector3 (x_movement, 0, 0);

				transform.position = new_position;
			}


		}
	}


}
