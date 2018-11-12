using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManageGestureRecognizer : MonoBehaviour
{

	public GameObject m_hand_controller;

	private HandController hc;

	DriveYawGesture drive_yaw_gesture;
	DriveRollGesture car_controller_script;
	DrivePitchGesture drive_pitch_gesture;

	bool ninety_deg_hand, one_hundred_and_eighty_hand, roll_hand;


	// Use this for initialization
	void Start ()
	{
		hc = m_hand_controller.GetComponent<HandController> ();
		ResetGesturesBool ();

		drive_yaw_gesture = this.GetComponent<DriveYawGesture> ();
		car_controller_script = this.GetComponent<DriveRollGesture> ();
		drive_pitch_gesture = this.GetComponent<DrivePitchGesture> ();

		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameManager.Instance.Get_Is_Playing ()) {
			if (ninety_deg_hand) {
				drive_pitch_gesture.PitchUpdate ();
			} else if (one_hundred_and_eighty_hand) {
				drive_yaw_gesture.YawUpdate ();
			} else if (roll_hand) {
				car_controller_script.RollUpdate ();
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
		drive_pitch_gesture.PitchStart (hc);
		GameManager.Instance.SetGameMathcHandAngle (GameMatch.HandAngle.Ninety);

		one_hundred_and_eighty_hand = false;
		roll_hand = false;
	}

	public void OneHundredEightyTrue ()
	{
		ninety_deg_hand = false;

		one_hundred_and_eighty_hand = true;
		drive_yaw_gesture.YawStart (hc);
		GameManager.Instance.SetGameMathcHandAngle (GameMatch.HandAngle.One_hundred);

		roll_hand = false;
	}

	public void RollTrue ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;

		roll_hand = true;
		car_controller_script.RollStart (hc);
		GameManager.Instance.SetGameMathcHandAngle (GameMatch.HandAngle.Roll);
	}

	public void ResetGesturesBool ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;
		roll_hand = false;
	}
}
