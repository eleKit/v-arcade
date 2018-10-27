using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using POLIMIGameCollective;
using System;
using System.IO;
using Leap;

public class CarManager : Singleton<CarManager>
{
	//attribute used to set the winning point coordinates
	public GameObject winning_Point;

	Vector3 initial_pos = new Vector3 (0f, -5.7f, 0f);

	//attribute used to load a level in a match
	FileNamesOfPaths loaded_path = new FileNamesOfPaths ();

	//attribute used to load a replay
	ReplayNamesOfPaths path_to_replay = new ReplayNamesOfPaths ();


	// Use this for initialization
	void Start ()
	{
		GameManager.Instance.player_initial_pos = initial_pos;

		/* set the bool of the current game in the game manager passing the GameType
		 * and in the UI manager
		 */

		GameManager.Instance.BaseStart ("CarGameMusic", GameMatch.GameType.Car);


		ResetPlayer ();
		GameManager.Instance.player.SetActive (false);
		
	}

	// Update is called once per frame
	void Update ()
	{
		GameManager.Instance.BaseUpdate ();
		
	}


	public void ToMenu ()
	{
		ResetPath ();
		ResetPlayer ();
		GameManager.Instance.BaseToMenu ();
	}

	public void ResetPlayer ()
	{
		GameManager.Instance.player.transform.position = GameManager.Instance.player_initial_pos;

	}

	void ResetPath ()
	{
		foreach (GameObject diamond in GameObject.FindGameObjectsWithTag ("Diamond")) {
			Destroy (diamond);
		}
	}

	//method used to load the level of the game
	public void ChooseLevel (FileNamesOfPaths path)
	{
		loaded_path = path;

		GameManager.Instance.BaseChooseLevel (path.name);
		ResetPlayer ();
		CarPathGenerator.Instance.LoadPath (path.file_path);

	}

	//@Overload for the Replay of a match
	public void ChooseLevel (ReplayNamesOfPaths path)
	{
		path_to_replay = path;
		MatchDataExtractor extractor = GetComponent<MatchDataExtractor> ();
		SetReplayHandAngle angle_setter = GetComponent<SetReplayHandAngle> ();

		GameManager.Instance.BaseChooseLevel (path, extractor.FromMatchDataToLevelName (path.match_data_path));
		ResetPlayer ();
		//load the level from the GameMatch data extracted from the ReplayNamesOfPaths class element
		CarPathGenerator.Instance.LoadPath (extractor.FromMatchDataToLevelFilePath (path.match_data_path, GameMatch.GameType.Car));

		//Set the hand angle in the gesture recognizer to use the correct Custom_Gesture recognizer
		angle_setter.SetHandAngleInGestureRecognizer (extractor.FromMatchDataToHandAngle (path.match_data_path));
		
	}

	//This method is used to set the point where the game ends
	public void SetWinningPosition (Vector3 coord)
	{
		winning_Point.transform.position = coord;
	}

	//function called after pause the game
	public void ResumeLevel ()
	{
		GameManager.Instance.BaseResumeLevel ();
	}

	public void RestartLevel ()
	{
		GameManager.Instance.m_wait_background.SetActive (true);
		ResetPath ();
		ChooseLevel (loaded_path);
	}

	public void WinLevel ()
	{
		GameManager.Instance.BaseWinLevel ();

	}


	public int GetScore ()
	{
		return GameManager.Instance.BaseGetScore ();
	}

	public void AddPoints ()
	{
		GameManager.Instance.BaseAddPoints ();

	}


	public bool GetIsPlaying ()
	{
		return GameManager.Instance.Get_Is_Playing ();

	}



}
