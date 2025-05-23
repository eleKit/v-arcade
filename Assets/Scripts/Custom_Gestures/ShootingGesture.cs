﻿using System.Collections;
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
	* 
	*/

	[Range (-5f, 0f)]
	public float pitch_offset = -0.2f;

	[Range (-5f, 0f)]
	public float yaw_offset = -0.2f;


	// In order to recognize the gesture a minimum angle should be done by the hand movement DEG
	[Range (-30f, 0f)]
	public float pitch_threshold = -15f;

	[Range (-30f, 0f)]
	public float yaw_threshold = -15f;




	//no more than K previous frames are taken into account
	[Range (10, 100)]
	public int K_pitch = 10;

	[Range (10, 100)]
	public int K_yaw = 10;


	[Range (10, 100)]
	public int K_lost = 30;


	[Range (0, 20)]
	public int num_frames_in_pitch_average_list = 6;

	[Range (0, 20)]
	public int num_frames_in_yaw_average_list = 6;


	[Range (0, 10)]
	public float x_movement = 1;

	[Range (0, 10)]
	public float y_movement = 1;

	public HandController hc;


	[Header ("Colors used when leap does/does not see the hands")]
	public Color transparent_white = new Color (1f, 1f, 1f, 0.2f);
	public Color medium_white = new Color (1f, 1f, 1f, 0.5f);
	public Color solid_white = new Color (1f, 1f, 1f, 1f);



	private LinkedList<float> pitch_list = new LinkedList<float> ();
	private LinkedList<float> pitch_average = new LinkedList<float> ();

	private LinkedList<float> yaw_list = new LinkedList<float> ();
	private LinkedList<float> yaw_average = new LinkedList<float> ();


	//# min frames between a gesture recongnized and the next
	const int N = 60;


	int frames_since_last_reconnection;



	//TODO get this from tuning as the zero position;
	float pitch_tuning_offset = -Mathf.Deg2Rad * 10f;
	float yaw_tuning_offset = 0;


	float y_max_player_position = 4.5f;
	float y_min_player_position = -2.7f;
	float x_max_player_position = 6.3f;
	float x_min_player_posiion = -6.3f;

	// Use this for initialization
	void Start ()
	{
		
		frames_since_last_reconnection = 0;
		SetShootingThresholds ();


	}

	// Update is called once per frame
	void Update ()
	{
		if (GameManager.Instance.Get_Is_Playing ()) {
			var current_frame = GameManager.Instance.GetCurrentFrame ();
			if (current_frame.Hands.Count == 1) {


				//change pointer colour
				if (gameObject.GetComponent<SpriteRenderer> ().color.Equals (transparent_white)) {
					gameObject.GetComponent<SpriteRenderer> ().color = medium_white;
				}

				frames_since_last_reconnection++;	


				//save average list
				if (pitch_average.Count >= num_frames_in_pitch_average_list) {
					pitch_average.RemoveFirst ();
				}

				if (yaw_average.Count >= num_frames_in_yaw_average_list) {
					yaw_average.RemoveFirst ();
				}

				pitch_average.AddLast (current_frame.Hands.Leftmost.Direction.Pitch + pitch_tuning_offset);
				yaw_average.AddLast (current_frame.Hands.Leftmost.Direction.Yaw + yaw_tuning_offset);


				//check gestures if lists are full of hand angle data and if a gesture has not been done just before this update
				if (pitch_list.Count >= K_pitch && yaw_list.Count >= K_yaw
				    && frames_since_last_reconnection >= K_lost) {
					CheckShootGesture ();

					//change pointer colour
					if (gameObject.GetComponent<SpriteRenderer> ().color.Equals (medium_white)) {
						gameObject.GetComponent<SpriteRenderer> ().color = solid_white;
					}

				} 

				//save new data
				if (pitch_list.Count >= K_pitch)
					pitch_list.RemoveFirst ();
				if (yaw_list.Count >= K_yaw)
					yaw_list.RemoveFirst ();
			
				pitch_list.AddLast (current_frame.Hands.Leftmost.Direction.Pitch + pitch_tuning_offset);
				yaw_list.AddLast (current_frame.Hands.Leftmost.Direction.Yaw + yaw_tuning_offset);

			} else {
				//if no hand is visible change colour in black
				gameObject.GetComponent<SpriteRenderer> ().color = transparent_white;

				pitch_list.Clear ();
				pitch_average.Clear ();
				yaw_list.Clear ();
				yaw_average.Clear ();

				frames_since_last_reconnection = 0;
			}
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
		if (current_pitch < pitch_threshold) {



			if (transform.position.y + (Vector3.down * Time.deltaTime * 3f).y >= y_min_player_position) {
				transform.Translate (Vector3.down * Time.deltaTime * 3f);
			}


		} else if (current_pitch > (-pitch_threshold)) {



			if (transform.position.y + (Vector3.up * Time.deltaTime * 3f).y <= y_max_player_position) {
				transform.Translate (Vector3.up * Time.deltaTime * 3f);
			}


		}

		if (current_yaw < yaw_threshold) {



			if (transform.position.x + (Vector3.left * Time.deltaTime * 3f).x >= x_min_player_posiion) {
				transform.Translate (Vector3.left * Time.deltaTime * 3f);
			}


		} else if (current_yaw > (-yaw_threshold)) {



			if (transform.position.x + (Vector3.right * Time.deltaTime * 3f).x <= x_max_player_position) {
				transform.Translate (Vector3.right * Time.deltaTime * 3f);
			}


		}
	}




	public void SetShootingThresholds ()
	{
		pitch_threshold = -GlobalPlayerData.globalPlayerData.player_data.left_pitch_scale;
		yaw_threshold = -GlobalPlayerData.globalPlayerData.player_data.left_yaw_scale;

	}


}
