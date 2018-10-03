using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManageGestureRecognizer : MonoBehaviour
{

	public HandController hc;

	DriveYawGesture drive_yaw_gesture;
	CarControllerScript car_controller_script;
	DrivePitchGesture drive_pitch_gesture;

	bool ninety_deg_hand, one_hundred_and_eighty_hand, roll_hand;


	// Use this for initialization
	void Start ()
	{
		ResetGesturesBool ();

		drive_yaw_gesture = this.GetComponent<DriveYawGesture> ();
		car_controller_script = this.GetComponent<CarControllerScript> ();
		drive_pitch_gesture = this.GetComponent<DrivePitchGesture> ();
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (ninety_deg_hand) {
			drive_pitch_gesture.PitchFixedUpdate ();
		} else if (one_hundred_and_eighty_hand) {
			drive_yaw_gesture.YawFixedUpdate ();
		} else if (roll_hand) {
			car_controller_script.RollFixedUpdate ();
		}
	}


	public void NinetyTrue ()
	{
		ninety_deg_hand = true;
		drive_pitch_gesture.PitchStart (hc);
		one_hundred_and_eighty_hand = false;
		roll_hand = false;
	}

	public void OneHundredEightyTrue ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = true;
		drive_yaw_gesture.YawStart (hc);
		roll_hand = false;
	}

	public void RollTrue ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;
		roll_hand = true;
		car_controller_script.RollStart (hc);
	}

	public void ResetGesturesBool ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;
		roll_hand = false;
	}
}
