using System.Collections;
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


	// In roder to recognize the gesture a minimum angle should be done by the hand movement RAD
	[Range (-5f, 0f)]
	public float threshold = -0.5f;

	//no more than K previous frames are taken into account
	[Range (10, 50)]
	public int K = 10;

	[Range (0, 10)]
	public int num_frames_in_average_list = 6;

	//the hand controller prefab in the scene
	[Header ("Hand conroller")]
	public HandController hc;


	float tuning_offset = -Mathf.Deg2Rad * 10f;


	//save the past pitch angles of left and right hand in the previous K frames
	private LinkedList<float> left_pitch = new LinkedList<float> ();
	private LinkedList<float> left_pitch_average = new LinkedList<float> ();

	private LinkedList<float> right_pitch = new LinkedList<float> ();
	private LinkedList<float> right_pitch_average = new LinkedList<float> ();


	// Use this for initialization
	void Start ()
	{
		threshold = -GlobalPlayerData.globalPlayerData.player_data.pitch_scale;
		Debug.Log ("New pitch scale" + threshold);

		if (hc.GetFrame ().Hands.Count == 2) {
			if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
				left_pitch.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch + tuning_offset);
			}
			if (hc.GetFrame ().Hands.Rightmost.IsRight) {
				right_pitch.AddLast (hc.GetFrame ().Hands.Rightmost.Direction.Pitch + tuning_offset);
			}
		}

		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (hc.GetFrame ().Hands.Count == 2) {


			//save left hand pitch and check left push gesture
			if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
				
				if (left_pitch_average.Count >= num_frames_in_average_list) {
					left_pitch_average.RemoveFirst ();
				}
				left_pitch_average.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch + tuning_offset);

				if (left_pitch.Count >= K) {
					CheckLeftPushGesture ();
					left_pitch.RemoveFirst ();
				}
				left_pitch.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch + tuning_offset);
			}



			//save right hand pitch and check right push gesture
			if (hc.GetFrame ().Hands.Rightmost.IsRight) {

				if (right_pitch_average.Count >= num_frames_in_average_list) {
					right_pitch_average.RemoveFirst ();
				}
				right_pitch_average.AddLast (hc.GetFrame ().Hands.Rightmost.Direction.Pitch + tuning_offset);

				if (right_pitch.Count >= K) {
					CheckRightPushGesture ();
					right_pitch.RemoveFirst ();
				}
				right_pitch.AddLast (hc.GetFrame ().Hands.Rightmost.Direction.Pitch + tuning_offset);
			}
				
		} else {
			left_pitch.Clear ();
			left_pitch_average.Clear ();
			right_pitch.Clear ();
			right_pitch_average.Clear ();
		}

	}


	void CheckLeftPushGesture ()
	{
		//search for the max pitch in the previous K frames
		float max_pitch = left_pitch.Max ();
		float pitch_average = left_pitch_average.Average ();


		if ((pitch_average - max_pitch) < threshold && pitch_average < offset) {
			Debug.Log ("left push" + " max " + max_pitch.ToString ());
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

		if ((pitch_average - max_pitch) < threshold && pitch_average < offset) {
			Debug.Log ("right push" + " max " + max_pitch.ToString ());

			if (MusicGameManager.Instance.right_trigger) {
				MusicGameManager.Instance.AddPoints (false);
			}
		}
		
	}


}
