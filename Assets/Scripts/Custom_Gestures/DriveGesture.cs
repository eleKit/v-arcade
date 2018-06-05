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
	[Range (-5f, 0f)]
	public float threshold = -0.5f;

	//no more than K previous frames are taken into account
	[Range (0, 50)]
	public int K = 10;

	[Range (0, 10)]
	public int x_movement = 1;

	public HandController hc;



	private LinkedList<float> pitch = new LinkedList<float> ();
	private LinkedList<float> yaw = new LinkedList<float> ();

	//public bool ninety_deg_hand, one_hundred_and_eighty_hand;

	bool pushed;

	// Use this for initialization
	void Start ()
	{
		pushed = false;

		/*ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;*/
	}
	
	// Update is called once per frame
	void Update ()
	{
		/*GetComponent<Rigidbody2D> ().AddForce (new Vector3 (0, 9.8f * 2, 0));*/

		if (hc.GetFrame ().Hands.Count == 1) {


			if (!pushed) {
				pushed = true;
				if (pitch.Count >= K) {
					// don't worry about leftmost, there is only one hand!!

					CheckPitchDriveGesture (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
					pitch.RemoveFirst ();
				}
				pitch.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
			} 

			pushed = false;

		}
		
	}


	void CheckPitchDriveGesture (float current_pitch)
	{
		
		
		float max_pitch = pitch.Max ();
		float min_pitch = pitch.Min ();

		//down gesture --> see yaw description
		if ((current_pitch - max_pitch) < threshold && current_pitch < offset) {
			
			Vector3 new_position = transform.position - new Vector3 (x_movement, 0, 0);

			transform.position = new_position;
			Debug.Log ("pushed car");


		} //up gesture --> see yaw description
		else if ((current_pitch - min_pitch) > (-threshold) && current_pitch > (-offset)) {
			Vector3 new_position = transform.position + new Vector3 (x_movement, 0, 0);

			transform.position = new_position;
			Debug.Log ("pushed car");
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
