using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;
using System.IO;
using UnityEngine.UI;

public class SpaceGameManager : Singleton<SpaceGameManager>
{


	//attribute used to load a level in a match
	FileNamesOfPaths loaded_path = new FileNamesOfPaths ();
	//attribute used to load a replay
	ReplayNamesOfPaths path_to_replay = new ReplayNamesOfPaths ();




	//value never changed
	Vector3 initial_player_pos = new Vector3 (0, -7f, 0);



	// Use this for initialization
	void Start ()
	{
		
		/* set the GameType of the current game in the GameManager.cs 
		 * (N.B. the UI manager that takes data from the gameManager.cs)
		 */

		GameManager.Instance.player_initial_pos = initial_player_pos;
		GameManager.Instance.BaseStart ("SpaceGameMusic", GameMatch.GameType.Space);

	
		// set the position of Player and then Deactivate it
		GameManager.Instance.player.transform.position = GameManager.Instance.player_initial_pos;
		GameManager.Instance.player.SetActive (false);
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		GameManager.Instance.BaseUpdate ();



		//Win conditions check
		if ((GameObject.FindGameObjectsWithTag ("Enemy").Length == 0 || TimerManager.Instance.GetTimer () < 0) &&
		    GameManager.Instance.Get_Is_Playing ()) {
			WinLevel ();
		}
		
	}


	public void WinLevel ()
	{
		//this function starts win jingle and then calls the win function
		ResetPath ();
		GameManager.Instance.BaseWinLevel ();
	}


	public void ToMenu ()
	{
		ResetPath ();
		GameManager.Instance.BaseToMenu ();
	}

	void ResetPath ()
	{
		//Reset the timer of the game
		TimerManager.Instance.ResetTimer ();

		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag ("Enemy")) {
			Destroy (enemy);
		}

		foreach (GameObject shot in GameObject.FindGameObjectsWithTag ("Shot")) {
			Destroy (shot);
		}
	}


	public void ChooseLevel (FileNamesOfPaths path)
	{
		loaded_path = path;
		GameManager.Instance.BaseChooseLevel (path.name);

		GameManager.Instance.player.transform.position = 
			GameManager.Instance.player_initial_pos;

		SpacePathGenerator.Instance.LoadPath (path.file_path);

	}

	//@Overload for the Replay of a match
	public void ChooseLevel (ReplayNamesOfPaths path)
	{
		path_to_replay = path;
		MatchDataExtractor extractor = GetComponent<MatchDataExtractor> ();
		SetReplayHandAngle angle_setter = GetComponent<SetReplayHandAngle> ();

		GameManager.Instance.BaseChooseLevel (path, extractor.FromMatchDataToLevelName (path.match_data_path));

		GameManager.Instance.player.transform.position = 
			GameManager.Instance.player_initial_pos;
		
		ResetPath ();
		//load the level from the GameMatch data extracted from the ReplayNamesOfPaths class element
		SpacePathGenerator.Instance.LoadPath (extractor.FromMatchDataToLevelFilePath (path.match_data_path, GameMatch.GameType.Space));

		//Set the hand angle in the gesture recognizer to use the correct Custom_Gesture recognizer
		angle_setter.SetHandAngleInGestureRecognizer (extractor.FromMatchDataToHandAngle (path.match_data_path));

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
