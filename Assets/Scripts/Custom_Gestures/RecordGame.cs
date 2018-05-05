using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class RecordGame : Singleton<RecordGame>
{

	public HandController hc;

	// Use this for initialization
	void Start ()
	{
		hc.Record ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void StartPlayback ()
	{
		hc.StopRecording ();
		CarGameManager.Instance.ChooseLevel ("");
		hc.GetLeapRecorder ().SaveToNewFile ("/Users/kit93/Desktop/Leap.json");
		hc.PlayRecording ();
	}
}
