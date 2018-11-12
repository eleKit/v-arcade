using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;
using System.IO;
using UnityEngine.UI;

public class SpaceGameManager : Singleton<SpaceGameManager>
{

	[Range (0f, 200f)]
	public float m_time_of_Timer = 200f;

	public Text m_timer_text;

	float timer_of_game;

	//attribute used to load a level in a match
	FileNamesOfPaths loaded_path = new FileNamesOfPaths ();
	//attribute used to load a replay
	ReplayNamesOfPaths path_to_replay = new ReplayNamesOfPaths ();


	//gameobject of the player used to check if the leap sees the hand so the timer can go on
	GameObject pl;

	//value never changed
	Vector3 initial_player_pos = new Vector3 (0, -7f, 0);


	void Awake ()
	{
		pl = GameObject.FindGameObjectWithTag ("Player");
	}
	// Use this for initialization
	void Start ()
	{
		//set timer
		timer_of_game = m_time_of_Timer;
		int timer = (int)Mathf.Round (timer_of_game);
		int min = timer / 60;
		int sec = timer % 60;
		m_timer_text.text = min.ToString () + ":" + sec.ToString ();

		GameManager.Instance.player_initial_pos = initial_player_pos;
		GameManager.Instance.BaseStart ("SpaceGameMusic", GameMatch.GameType.Space);

		/* set the bool of the current game in the game manager 
		 * and in the GUI manager
		 */


		GameManager.Instance.player.transform.position = 
			GameManager.Instance.player_initial_pos;

		GameManager.Instance.player.SetActive (false);
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		GameManager.Instance.BaseUpdate ();


		//Timer text update
		if (GameManager.Instance.Get_Is_Playing () &&
		    !(pl.GetComponent<SpriteRenderer> ().color.Equals (pl.GetComponent<SpaceGesturesManager> ().transparent_white)
		    || pl.GetComponent <SpriteRenderer> ().color.Equals (pl.GetComponent<SpaceGesturesManager> ().medium_white))) {
			timer_of_game -= Time.deltaTime;
			int timer = (int)Mathf.Round (timer_of_game);
			int min = timer / 60;
			int sec = timer % 60;
			m_timer_text.text = min.ToString () + ":" + sec.ToString ();
		}

		//Win conditions check
		if ((GameObject.FindGameObjectsWithTag ("Enemy").Length == 0 || timer_of_game < 0) &&
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
		timer_of_game = m_time_of_Timer;

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


		/* the Yaw and Pitch Thresholds must be set in the GlobalPlayerData instance (xusing extractor.FromMatchDataSetGlobalPlayerData)
		 * BEFORE the angle_setter.SetHandAngleInGestureRecognizer call
		 * because this function calls the YawStart() in the GestureRecongizerManager
		 * that set the Thresholds that taken from the GlobalPlayerData instance 
		 * in the GestureRecognizer script  
		 */
		extractor.FromMatchDataSetGlobalPlayerData (path.match_data_path);
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

	public void AddPoints (int points)
	{
		GameManager.Instance.BaseAddPoints (points);

	}

	public bool GetIsPlaying ()
	{
		return GameManager.Instance.Get_Is_Playing ();
	}

	public float GetTimer ()
	{
		return timer_of_game;
	}
}
