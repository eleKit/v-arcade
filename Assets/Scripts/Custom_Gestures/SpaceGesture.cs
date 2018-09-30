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



	//no more than K previous frames are taken into account
	[Range (10, 100)]
	public int K_yaw = 10;


	[Range (10, 100)]
	public int K_lost = 30;


	[Range (0f, 30f)]
	public float speed = 10f;


	[Range (0, 20)]
	public int num_frames_in_yaw_average_list = 6;


	[Range (0, 10)]
	public float x_movement = 1;


	public HandController hc;

	private LinkedList<float> yaw_list = new LinkedList<float> ();
	private LinkedList<float> yaw_average = new LinkedList<float> ();


	//# min frames between a gesture recongnized and the next
	const int N = 60;


	private int frames_since_last_reconnection;


	// In order to recognize the gesture a minimum angle should be done by the hand movement in RAD
	private float left_yaw_threshold = -15f;
	private float right_yaw_threshold = -15f;

	//The spaceship has a minimum position and a maximum one, it must not escape from the screen
	private float x_max_player_position = 16f;
	private float x_min_player_posiion = -16f;

	// Use this for initialization
	void Start ()
	{
		frames_since_last_reconnection = 0;
		left_yaw_threshold = -GlobalPlayerData.globalPlayerData.player_data.left_yaw_scale;
		right_yaw_threshold = -GlobalPlayerData.globalPlayerData.player_data.right_yaw_scale;

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

			yaw_average.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw);


			//check gestures if lists are full of hand angle data and if a gesture has not been done just before this update
			if (yaw_list.Count >= K_yaw && frames_since_last_reconnection >= K_lost) {
				CheckMoveSpaceshipGesture (hc.GetFixedFrame ().Hands.Leftmost.IsLeft);

				//change pointer colour
				if (gameObject.GetComponent<SpriteRenderer> ().color.Equals (Color.grey)) {
					gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
				}

			}

			//save new data
			if (yaw_list.Count >= K_yaw)
				yaw_list.RemoveFirst ();

			yaw_list.AddLast (hc.GetFixedFrame ().Hands.Leftmost.Direction.Yaw);

		} else {
			//if no hand is visible change colour in black
			gameObject.GetComponent<SpriteRenderer> ().color = Color.black;

			yaw_list.Clear ();
			yaw_average.Clear ();

			frames_since_last_reconnection = 0;
		}
	}




	void CheckMoveSpaceshipGesture (bool is_left)
	{
		float threshold;

		if (is_left) {
			threshold = left_yaw_threshold;
		} else {
			threshold = right_yaw_threshold;
		}

		float max_yaw = yaw_list.Max ();
		float min_yaw = yaw_list.Min ();

		float current_yaw = yaw_average.Average ();

		//move left
		if (current_yaw < threshold) {


			if ((transform.position.x + (Vector3.left * Time.deltaTime * speed).x) >= x_min_player_posiion) {

				transform.Translate (Vector3.left * Time.deltaTime * speed);
			}

			//move right
		} else if (current_yaw > (-threshold)) {
			

			if ((transform.position.x + (Vector3.right * Time.deltaTime * speed).x) <= x_max_player_position) {

				transform.Translate (Vector3.right * Time.deltaTime * speed);
			}


		}
	}


}
