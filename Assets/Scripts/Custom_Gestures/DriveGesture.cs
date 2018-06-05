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



	enum Drive_Phase
	{
		Null,
		NoHands_phase,
		Check_gesture_phase,
		Gesture_happening_phase,
	};



	/* the hand should return around the original position after a push gesture, 
	* no more gestures are accepted in case this offset condition is not respected
	*/
	[Range (-5f, 0f)]
	public float offset = -0.2f;


	// In roder to recognize the gesture a minimum angle should be done by the hand movement
	[Range (-5f, 0f)]
	public float threshold = -0.5f;

	//no more than K previous frames are taken into account
	[Range (0, 50)]
	public int K = 10;

	[Range (0, 10)]
	public int x_movement = 1;

	public HandController hc;



	private LinkedList<float> pitch_list = new LinkedList<float> ();
	private LinkedList<float> yaw = new LinkedList<float> ();

	//public bool ninety_deg_hand, one_hundred_and_eighty_hand;

	Drive_Phase current_phase;
	Drive_Phase previous_phase;

	int index = 0;
	//TODO get this fro tuning as the zero position;
	float tuning_pitch = 0f;

	// Use this for initialization
	void Start ()
	{
		current_phase = Drive_Phase.NoHands_phase;
		/*ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;*/
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		index++;
		Debug.Log (current_phase.ToString () + " " + index.ToString ());

		switch (current_phase) {


		case Drive_Phase.NoHands_phase:
			if (hc.GetFixedFrame ().Hands.Count == 1) {
				if (previous_phase == Drive_Phase.Null) {
					current_phase = Drive_Phase.Check_gesture_phase;
				} else {
					current_phase = previous_phase;
				}
			}
			break;
		
		case Drive_Phase.Check_gesture_phase:

			previous_phase = current_phase;

			if (hc.GetFixedFrame ().Hands.Count == 1) {
				if (pitch_list.Count >= K) {
					// don't worry about leftmost, there is only one hand!!

					StartCoroutine (CheckPitchDriveGesture (hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch, index));
					pitch_list.RemoveFirst ();
				}
				pitch_list.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Pitch);
			} else {
				current_phase = Drive_Phase.NoHands_phase;
			}

			break;
		
		case Drive_Phase.Gesture_happening_phase:
			


				//wait that coroutine ends and do nothing
		

			break;
		}
	}


	IEnumerator CheckPitchDriveGesture (float current_pitch, int i)
	{
		

		float max_pitch = pitch_list.Max ();
		float min_pitch = pitch_list.Min ();

		//down gesture --> see yaw description
		if ((current_pitch - max_pitch) < threshold && current_pitch < offset) {

			current_phase = Drive_Phase.Gesture_happening_phase;

			Vector3 new_position = transform.position - new Vector3 (x_movement, 0, 0);

			transform.position = new_position;
			Debug.Log ("pushed left car " + i.ToString ());
	
			pitch_list.Clear ();

			yield return new WaitForSeconds (2f);

		} else if ((current_pitch - min_pitch) > (-threshold) && current_pitch > (-offset)) {
			
			current_phase = Drive_Phase.Gesture_happening_phase;

			Vector3 new_position = transform.position + new Vector3 (x_movement, 0, 0);

			transform.position = new_position;
			Debug.Log ("pushed right car " + i.ToString ());

			pitch_list.Clear ();

			yield return new WaitForSeconds (1f);
		}

		yield return new WaitForSeconds (0f);
		current_phase = Drive_Phase.Check_gesture_phase;
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
