using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.Linq;

public class DriveGesture : MonoBehaviour
{

	/* Drive gesture: a sliding movement made by all the fingers and the palm,
	 * substitutes the old slide gesture. 
	 * The wrist is not moved and the palm goes left and right or up and down
	 * only the angle made by the hand movement is checked
	 */



	/* the hand should return around the original position after a push gesture, 
	* no more gestures are accepted in case this offset condition is not respected
	*/
	[Range (-5f, 0f)]
	public float offset = -0.2f;


	// In roder to recognize the gesture a minimum angle should be done by the hand movement
	[Range (-30f, 0f)]
	public float threshold = -15f;

	//no more than K previous frames are taken into account
	[Range (0, 50)]
	public int K = 10;

	[Range (0, 10)]
	public int x_movement = 1;

	public HandController hc;



	private LinkedList<float> pitch_list = new LinkedList<float> ();
	private LinkedList<float> yaw = new LinkedList<float> ();

	//public bool ninety_deg_hand, one_hundred_and_eighty_hand;



	int frames_since_last_gesture;
	//TODO get this fro tuning as the zero position;
	float tuning_offset = -Mathf.Deg2Rad * 10f;

	// Use this for initialization
	void Start ()
	{
		frames_since_last_gesture = 20;
		/*ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;*/
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (hc.GetFixedFrame ().Hands.Count == 1) {
			
			if (pitch_list.Count >= K) {
				if (frames_since_last_gesture >= 60) {
					CheckPitchDriveGesture (hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch + tuning_offset);
				} else {
					frames_since_last_gesture++;
				}
				pitch_list.RemoveFirst ();
			}
			pitch_list.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch + tuning_offset);
		}
	}




	void CheckPitchDriveGesture (float current_pitch)
	{
		

		float max_pitch = pitch_list.Max ();
		float min_pitch = pitch_list.Min ();

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





	void CheckYawDriveGesture (float current_yaw)
	{
		//search for the max pitch in the previous K frames
		float max_yaw = yaw.Max ();
		float min_yaw = yaw.Min ();

		/* if the hand in moved in the left direction the max yaw must be the around zero position
		 * and if the (current_yaw - max_yaw) < threshold the gesture is recognized
		 */
		if ((current_yaw - max_yaw) < threshold && current_yaw < offset) {

			Vector3 new_position = transform.position - new Vector3 (x_movement, 0, 0);

			transform.position = new_position;
			Debug.Log (transform.position.x.ToString ());
		} 

		/* otherwise if the hand is moved in the right direction the min yaw must be around the zero position
		 * and if the current_yaw - min_yaw) > threshold the gesture is recognized
		 */
		/*else if ((current_yaw - min_yaw) > threshold && min_yaw > (-offset)) {
			Vector3 new_position = transform.position + new Vector3 (x_movement, 0, 0);

			transform.position = new_position;
			Debug.Log (transform.position.x.ToString ());
		}*/

	}



	/*public void NinetyTrue ()
	{
		ninety_deg_hand = true;
		one_hundred_and_eighty_hand = false;
	}

	public void OneHundredEightyTrue ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = true;
	}*/
}
