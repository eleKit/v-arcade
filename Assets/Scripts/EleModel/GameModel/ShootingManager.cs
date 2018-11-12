using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;
using System.IO;
using UnityEngine.UI;

public class ShootingManager : Singleton<ShootingManager>
{
	//timer attributes
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

	//CONSTANT (recall vector3 cannot be declared costant
	Vector3 initial_player_pos = new Vector3 (0, 0.75f, 0);

	void Awake ()
	{
		pl = GameObject.FindGameObjectWithTag ("Player");
	}
	// Use this for initialization
	void Start ()
	{
		//set the timer
		timer_of_game = m_time_of_Timer;
		int timer = (int)Mathf.Round (timer_of_game);
		int min = timer / 60;
		int sec = timer % 60;
		m_timer_text.text = min.ToString () + ":" + sec.ToString ();

		GameManager.Instance.player_initial_pos = initial_player_pos;
		GameManager.Instance.BaseStart ("DuckGameMusic", GameMatch.GameType.Shooting);

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


		if (GameManager.Instance.Get_Is_Playing () &&
		    !(pl.GetComponent<SpriteRenderer> ().color.Equals (pl.GetComponent<ShootingGesture> ().transparent_white)
		    || pl.GetComponent <SpriteRenderer> ().color.Equals (pl.GetComponent<ShootingGesture> ().medium_white))) {
			timer_of_game -= Time.deltaTime;
			int timer = (int)Mathf.Round (timer_of_game);
			int min = timer / 60;
			int sec = timer % 60;
			m_timer_text.text = min.ToString () + ":" + sec.ToString ();
		}


		if ((GameObject.FindGameObjectsWithTag ("Duck").Length == 0 || timer_of_game < 0) &&
		    GameManager.Instance.Get_Is_Playing ()) {
			WinLevel ();
		}
	}

	public void ToMenu ()
	{
		ResetPath ();
		GameManager.Instance.BaseToMenu ();
	}

	void ResetPath ()
	{
		//reset timer
		timer_of_game = m_time_of_Timer;

		foreach (GameObject duck in GameObject.FindGameObjectsWithTag ("Duck")) {
			Destroy (duck);
		}
	}

	public void WinLevel ()
	{
		ResetPath ();
		//this function starts win jingle and then calls the win function
		GameManager.Instance.BaseWinLevel ();
	}

	public void ChooseLevel (FileNamesOfPaths path)
	{
		loaded_path = path;
		GameManager.Instance.BaseChooseLevel (path.name);

		//set player initial position
		GameManager.Instance.player.transform.position = 
			GameManager.Instance.player_initial_pos;

		DuckPathGenerator.Instance.LoadPath (path.file_path);
		
	}


	//@Overload for the Replay of a match
	public void ChooseLevel (ReplayNamesOfPaths path)
	{
		path_to_replay = path;
		MatchDataExtractor extractor = GetComponent<MatchDataExtractor> ();
		SetGestureThresholds thresholds_setter = GetComponent<SetGestureThresholds> ();

		GameManager.Instance.BaseChooseLevel (path, extractor.FromMatchDataToLevelName (path.match_data_path));

		//set player initial position
		GameManager.Instance.player.transform.position = GameManager.Instance.player_initial_pos;
		
		ResetPath ();

		//load the level from the GameMatch data extracted from the ReplayNamesOfPaths class element
		DuckPathGenerator.Instance.LoadPath (extractor.FromMatchDataToLevelFilePath (path.match_data_path, GameMatch.GameType.Shooting));

		/* the extractor.FromMatchDataSetGlobalPlayerData(path.match_data_path) sets the Thresholds in the GlobalPlayerData instance
		 * 
		 * the thresholds_setter.SetThresholds () call the function in the GestureRecognizer script 
		 * that takes the thresholds from the GlobalPlayerData instance and writes them in the thesholds private attributes
		 */
		extractor.FromMatchDataSetGlobalPlayerData (path.match_data_path);
		thresholds_setter.SetThresholds ();


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
}
