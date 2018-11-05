using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGestureThresholds : MonoBehaviour
{

	public PushGesture push_gesture;
	public ShootingGesture shooting_gesture;



	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	/* this function is used in the Replay Scene to set the GlobalPlayerData instance with the correct threshold
	 * taken from the GameMatch file (by the MatchDataExtractor function FromMatchDataSetGlobalPlayerData()
	 */
	public void SetThresholds ()
	{

		if (push_gesture != null) {
			push_gesture.SetPushThresholds ();
		} else if (shooting_gesture != null) {
			shooting_gesture.SetShootingThresholds ();
		}
	}
}
