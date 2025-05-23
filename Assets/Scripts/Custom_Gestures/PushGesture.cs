﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.Linq;

public class PushGesture : MonoBehaviour
{

	/* Push gesture: a tapping movement made by all the fingers and the palm,
	 * substitutes the old jump gesture. 
	 * The wrist is not moved and the palm goes down and up
	 * only the angle made by the hand movement is checked
	 */


	/* the hand should return around the original position after a push gesture, 
	 * no more gestures are accepted in case this offset condition is not respected
	 */
	[Range (-5f, 0f)]
	public float offset = -0.2f;


	//no more than K previous frames are taken into account
	[Range (10, 50)]
	public int K = 10;

	[Range (0, 10)]
	public int num_frames_in_average_list = 6;

	//the hand controller prefab in the scene
	[Header ("Hand conroller")]
	public HandController hc;


	//the pitch of every sensor has not the 0 in the correct point but at 10 DEG up
	private float tuning_offset = -Mathf.Deg2Rad * 10f;

	// In roder to recognize the gesture a minimum angle should be done by the hand movement RAD
	private float left_threshold = -0.5f;
	private float right_threshold = -0.5f;

	//save the past pitch angles of left and right hand in the previous K frames
	private LinkedList<float> left_pitch = new LinkedList<float> ();
	private LinkedList<float> left_pitch_average = new LinkedList<float> ();

	private LinkedList<float> right_pitch = new LinkedList<float> ();
	private LinkedList<float> right_pitch_average = new LinkedList<float> ();


	// Use this for initialization
	void Start ()
	{
		SetPushThresholds ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameManager.Instance.Get_Is_Playing ()) {
			var current_frame = GameManager.Instance.GetCurrentFrame ();
			if (current_frame.Hands.Count == 2) {


				//save left hand pitch and check left push gesture
				if (current_frame.Hands.Leftmost.IsLeft) {
				
					if (left_pitch_average.Count >= num_frames_in_average_list) {
						left_pitch_average.RemoveFirst ();
					}
					left_pitch_average.AddLast (current_frame.Hands.Leftmost.Direction.Pitch + tuning_offset);

					if (left_pitch.Count >= K) {
						CheckLeftPushGesture ();
						left_pitch.RemoveFirst ();
					}
					left_pitch.AddLast (current_frame.Hands.Leftmost.Direction.Pitch + tuning_offset);
				}



				//save right hand pitch and check right push gesture
				if (current_frame.Hands.Rightmost.IsRight) {

					if (right_pitch_average.Count >= num_frames_in_average_list) {
						right_pitch_average.RemoveFirst ();
					}
					right_pitch_average.AddLast (current_frame.Hands.Rightmost.Direction.Pitch + tuning_offset);

					if (right_pitch.Count >= K) {
						CheckRightPushGesture ();
						right_pitch.RemoveFirst ();
					}
					right_pitch.AddLast (current_frame.Hands.Rightmost.Direction.Pitch + tuning_offset);
				}
				
			} else {
				left_pitch.Clear ();
				left_pitch_average.Clear ();
				right_pitch.Clear ();
				right_pitch_average.Clear ();
			}
		}

	}


	void CheckLeftPushGesture ()
	{
		//search for the max pitch in the previous K frames
		float max_pitch = left_pitch.Max ();
		float pitch_average = left_pitch_average.Average ();


		if ((pitch_average - max_pitch) < left_threshold && pitch_average < offset) {
			if (MusicGameManager.Instance.left_trigger) {
				MusicGameManager.Instance.AddPoints (true);
			}
		}
	}

	void CheckRightPushGesture ()
	{
		//search for the max pitch in the previous K frames
		float max_pitch = right_pitch.Max ();
		float pitch_average = right_pitch_average.Average ();

		if ((pitch_average - max_pitch) < right_threshold && pitch_average < offset) {
			if (MusicGameManager.Instance.right_trigger) {
				MusicGameManager.Instance.AddPoints (false);
			}
		}
		
	}






	public void SetPushThresholds ()
	{
		left_threshold = -GlobalPlayerData.globalPlayerData.player_data.left_pitch_scale;
		right_threshold = -GlobalPlayerData.globalPlayerData.player_data.right_pitch_scale;


	}


}
