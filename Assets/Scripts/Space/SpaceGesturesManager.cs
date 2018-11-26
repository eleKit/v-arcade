using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGesturesManager : MonoBehaviour
{
	[Header ("Colors used when leap does/does not see the hands")]
	public Color transparent_white = new Color (1f, 1f, 1f, 0.2f);
	public Color medium_white = new Color (1f, 1f, 1f, 0.5f);
	public Color solid_white = new Color (1f, 1f, 1f, 1f);


	public GameObject m_hand_controller;

	private HandController hc;

	SpaceGesture space_yaw_gesture;
	SpacePitchGesture space_pitch_gesture;
	SpaceRollGesture space_roll_gesture;

	bool ninety_deg_hand, one_hundred_and_eighty_hand, roll_hand;

	// Use this for initialization
	//Awake is called even if the GO in the scene is not active like in this case)
	void Awake ()
	{
		hc = m_hand_controller.GetComponent<HandController> ();

		ResetGesturesBool ();

		space_yaw_gesture = this.GetComponent<SpaceGesture> ();
		space_pitch_gesture = this.GetComponent<SpacePitchGesture> ();
		space_roll_gesture = this.GetComponent<SpaceRollGesture> ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameManager.Instance.Get_Is_Playing ()) {
			if (ninety_deg_hand) {
				space_pitch_gesture.PitchUpdate ();
			} else if (one_hundred_and_eighty_hand) {
				space_yaw_gesture.YawUpdate ();
			} else if (roll_hand) {
				space_roll_gesture.RollUpdate ();
			}
		}
		
	}

	/* these functions are called 
	 * Game Scene: by a button in the UI menu screen
	 * Replay Scene: inside the ChooseLevel() by the angle_setter.SetHandAngleInGestureRecognizer(...) of the SetReplayHandAngle.cs
	 */

	public void NinetyTrue ()
	{
		one_hundred_and_eighty_hand = false;
		roll_hand = false;

		ninety_deg_hand = true;
		space_pitch_gesture.PitchStart (hc, transparent_white, medium_white, solid_white);
		GameManager.Instance.SetGameMathcHandAngle (GameMatch.HandAngle.Ninety);


	}

	public void OneHundredEightyTrue ()
	{
		ninety_deg_hand = false;
		roll_hand = false;

		one_hundred_and_eighty_hand = true;
		space_yaw_gesture.YawStart (hc, transparent_white, medium_white, solid_white);
		GameManager.Instance.SetGameMathcHandAngle (GameMatch.HandAngle.One_hundred);

	}

	public void RollTrue ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;

		roll_hand = true;

		space_roll_gesture.RollStart (hc, transparent_white, medium_white, solid_white);
		GameManager.Instance.SetGameMathcHandAngle (GameMatch.HandAngle.Roll);

	}

	public void ResetGesturesBool ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;
		roll_hand = false;
	}
}
