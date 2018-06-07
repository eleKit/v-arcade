using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.Linq;

public class DrivePitchGesture : MonoBehaviour
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


	// In order to recognize the gesture a minimum angle should be done by the hand movement
	[Range (-30f, 0f)]
	public float threshold = -15f;

	//no more than K previous frames are taken into account
	[Range (10, 50)]
	public int K = 10;


	[Range (0, 10)]
	public int num_frames_in_average_list = 6;


	[Range (0, 10)]
	public float x_movement = 1;

	public HandController hc;



	private LinkedList<float> pitch_list = new LinkedList<float> ();
	private LinkedList<float> pitch_average = new LinkedList<float> ();

	const int N = 60;


	int frames_since_last_gesture;

	//TODO get this from tuning as the zero position;
	float tuning_offset = -Mathf.Deg2Rad * 10f;

	// Use this for initialization
	void Start ()
	{
		frames_since_last_gesture = N;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (hc.GetFixedFrame ().Hands.Count == 1) {

			if (pitch_average.Count >= num_frames_in_average_list) {
				pitch_average.RemoveFirst ();
			}
			pitch_average.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch + tuning_offset);

			if (pitch_list.Count >= K) {
				if (frames_since_last_gesture >= N) {
					CheckPitchDriveGesture ();
				} else {
					frames_since_last_gesture++;
				}
				pitch_list.RemoveFirst ();
			}
			pitch_list.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch + tuning_offset);
		}
	}




	void CheckPitchDriveGesture ()
	{
		

		float max_pitch = pitch_list.Max ();
		float min_pitch = pitch_list.Min ();

		float current_pitch = pitch_average.Average ();

		//down gesture --> see yaw description
		if ((current_pitch - max_pitch) < Mathf.Deg2Rad * threshold && current_pitch < offset) {

			frames_since_last_gesture = 0;

			Vector3 new_position = transform.position - new Vector3 (x_movement, 0, 0);

			transform.position = new_position;


		} else if ((current_pitch - min_pitch) > Mathf.Deg2Rad * (-threshold) && current_pitch > (-offset)) {
			
			frames_since_last_gesture = 0;

			Vector3 new_position = transform.position + new Vector3 (x_movement, 0, 0);

			transform.position = new_position;


		}
	}




}
