using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetReplayHandAngle : MonoBehaviour
{



	public CarManageGestureRecognizer car_g_recognizer;
	//TODO public SpaceManageGestureRecognizer space_g_recognizer;

	// Use this for initialization
	void Start ()
	{
		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void SetHandAngleInGestureRecognizer (GameMatch.HandAngle hand_angle)
	{

		if (car_g_recognizer != null) {
			switch (hand_angle) {
			case GameMatch.HandAngle.Ninety:
				car_g_recognizer.NinetyTrue ();
				break;
			case GameMatch.HandAngle.One_hundred:
				car_g_recognizer.OneHundredEightyTrue ();
				break;
			case GameMatch.HandAngle.Roll:
				car_g_recognizer.RollTrue ();
				break;
			case GameMatch.HandAngle.None:
				//Do nothing, there is an error
				Debug.LogError ("wrong HandAngle!");
				break;
			}
		} /* TODO else if (space_g_recognizer != null) {
				switch (hand_angle) {
			case GameMatch.HandAngle.Ninety:
				space_g_recognizer.NinetyTrue ();
				break;
			case GameMatch.HandAngle.One_hundred:
				space_g_recognizer.OneHundredEightyTrue ();
				break;
			case GameMatch.HandAngle.Roll:
				//Do nothing, there is an error
				Debug.LogError ("wrong HandAngle!");
				break;
			case GameMatch.HandAngle.None:
				//Do nothing, there is an error
				Debug.LogError ("wrong HandAngle!");
				break;
			}
		}*/
	}
}
