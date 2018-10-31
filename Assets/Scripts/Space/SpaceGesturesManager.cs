using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGesturesManager : MonoBehaviour
{


	public GameObject m_hand_controller;

	private HandController hc;

	SpaceGesture space_yaw_gesture;
	//TODO SpacePitchGesture space_pitch_gesture;

	bool ninety_deg_hand, one_hundred_and_eighty_hand;

	// Use this for initialization
	//Awake is called even if the GO in the scene is not active like in this case)
	void Awake ()
	{
		hc = m_hand_controller.GetComponent<HandController> ();

		ResetGesturesBool ();

		space_yaw_gesture = this.GetComponent<SpaceGesture> ();
		//space_pitch_gesture = this.GetComponent<SpacePitchGesture> ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameManager.Instance.Get_Is_Playing ()) {
			if (ninety_deg_hand) {
				//space_pitch_gesture.PitchFixedUpdate ();
			} else if (one_hundred_and_eighty_hand) {
				space_yaw_gesture.YawUpdate ();
			}
		}
		
	}

	/* these functions are called 
	 * Game Scene: by a button in the UI menu screen
	 * Replay Scene: inside the ChooseLevel() by the angle_setter.SetHandAngleInGestureRecognizer(...) of the SetReplayHandAngle.cs
	 */

	public void NinetyTrue ()
	{
		ninety_deg_hand = true;
		//space_pitch_gesture.SetHandController (hc);
		GameManager.Instance.SetGameMathcHandAngle (GameMatch.HandAngle.Ninety);

		one_hundred_and_eighty_hand = false;
	}

	public void OneHundredEightyTrue ()
	{
		ninety_deg_hand = false;

		one_hundred_and_eighty_hand = true;
		space_yaw_gesture.YawStart (hc);
		GameManager.Instance.SetGameMathcHandAngle (GameMatch.HandAngle.One_hundred);

	}

	public void ResetGesturesBool ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;
	}
}
