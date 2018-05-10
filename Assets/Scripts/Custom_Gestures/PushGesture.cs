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


	// In roder to recognize the gesture a minimum angle should be done by the hand movement
	[Range (-5f, 0f)]
	public float threshold = -0.5f;

	//no more than K previous frames are taken into account
	[Range (0, 50)]
	public int K = 10;

	//the hand controller prefab in the scene
	public GameObject handController;


	private HandController hc;

	//save the past pitch angles of left and right hand in the previous K frames
	private LinkedList<float> left_pitch = new LinkedList<float> ();
	private LinkedList<float> right_pitch = new LinkedList<float> ();


	// Use this for initialization
	void Start ()
	{
		hc = handController.GetComponent<HandController> ();
		if (hc.GetFrame ().Hands.Count == 2) {
			if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
				left_pitch.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
			}
			if (hc.GetFrame ().Hands.Rightmost.IsRight) {
				right_pitch.AddLast (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
			}
		}

		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (hc.GetFrame ().Hands.Count == 2) {
			//save left hand pitch and check left push gesture
			if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
				if (left_pitch.Count >= K) {
					CheckLeftPushGesture (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
					left_pitch.RemoveFirst ();
				}
				left_pitch.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
			}

			//save right hand pitch and check right push gesture
			if (hc.GetFrame ().Hands.Rightmost.IsRight) {
				if (right_pitch.Count >= K) {
					CheckRightPushGesture (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
					right_pitch.RemoveFirst ();
				}
				right_pitch.AddLast (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
			}
				
		}
		//TODO fare i controlli se le due mani non sono viste o ne è vista una sola



	}

	void CheckLeftPushGesture (float pitch)
	{
		//search for the max pitch in the previous K frames
		float max_pitch = left_pitch.Max ();
		if ((pitch - max_pitch) < threshold && pitch < offset) {
			Debug.Log ("left push" + " max " + max_pitch.ToString ());
			if (MusicGameManager.Instance.left_trigger) {
				MusicGameManager.Instance.AddPoints ();
			}
		}
	}

	void CheckRightPushGesture (float pitch)
	{
		//search for the max pitch in the previous K frames
		float max_pitch = right_pitch.Max ();
		if ((pitch - max_pitch) < threshold && pitch < offset) {
			Debug.Log ("right push" + " max " + max_pitch.ToString ());

			if (MusicGameManager.Instance.right_trigger) {
				MusicGameManager.Instance.AddPoints ();
			}
		}
		
	}


}
