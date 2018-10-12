using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceManageGestureRecognizer : MonoBehaviour
{

	public GameObject m_hand_controller;

	private HandController hc;

	SpaceGesture space_yaw_gesture;
	//SpacePitchGesture space_pitch_gesture;

	bool ninety_deg_hand, one_hundred_and_eighty_hand;


	// Use this for initialization
	void Awake ()
	{
		hc = m_hand_controller.GetComponent<HandController> ();

		ResetGesturesBool ();

		space_yaw_gesture = this.GetComponent<SpaceGesture> ();
		//space_pitch_gesture = this.GetComponent<SpacePitchGesture> ();


	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (ninety_deg_hand) {
			//space_pitch_gesture.PitchFixedUpdate ();
		} else if (one_hundred_and_eighty_hand) {
			space_yaw_gesture.YawFixedUpdate ();
		}
	}


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
		space_yaw_gesture.SetHandController (hc);
		GameManager.Instance.SetGameMathcHandAngle (GameMatch.HandAngle.One_hundred);

	}


	public void ResetGesturesBool ()
	{
		ninety_deg_hand = false;
		one_hundred_and_eighty_hand = false;
	}
}

