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

	private LinkedList<float> pitch_average = new LinkedList<float> ();

	/* THRESHOLD: In roder to recognize the gesture a minimum angle should be done by the hand movement in RAD
	 * since the hands have different abilities each hand has it own threshold and 
	 * before checking the gesture it is checked what hand is in use
	 */


	private float pitch_left_threshold = -15f;
	private float pitch_right_threshold = -15f;


	//The spaceship has a minimum position and a maximum one, it must not escape from the screen
	private float x_max_player_position = 11.5f;
	private float x_min_player_posiion = -11.5f;




	// Use this for initialization
	public void PitchStart (HandController handController)
	{
		
		pitch_left_threshold = -GlobalPlayerData.globalPlayerData.player_data.left_pitch_scale;
		pitch_right_threshold = -GlobalPlayerData.globalPlayerData.player_data.right_pitch_scale;
		hc = handController;
	}

	// Update is called once per frame
	public void PitchFixedUpdate ()
	{
		if (hc.GetFixedFrame ().Hands.Count == 1) {

			transform.position = transform.position + new Vector3 (0f, 0.1f, 0f);





			if (pitch_average.Count >= num_frames_in_average_list) {
				pitch_average.RemoveFirst ();
			}
			pitch_average.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);

	
			CheckPitchPushGesture (hc.GetFixedFrame ().Hands.Leftmost.IsLeft);


		}

	}








	void CheckPitchPushGesture (bool is_left)
	{

		float threshold;

		if (is_left) {
			threshold = pitch_left_threshold;
		} else {
			threshold = pitch_right_threshold;
		}
		float current_pitch = pitch_average.Average ();


		if (current_pitch < threshold) {


			if ((transform.position.x + (Vector3.right * Time.deltaTime * speed).x) <= x_max_player_position) {
				transform.Translate (Vector3.right * Time.deltaTime * speed);
			}


		} else if (current_pitch > (-threshold)) {

			
			if ((transform.position.x + (Vector3.left * Time.deltaTime * speed).x) >= x_min_player_posiion) {

				transform.Translate (Vector3.left * Time.deltaTime * speed);
			}


		}


	}


}


