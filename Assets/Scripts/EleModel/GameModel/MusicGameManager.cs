using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;
using System.IO;

public class MusicGameManager : Singleton<MusicGameManager>
{
	[Range (0f, 5f)]
	public float m_button_movement_offset = 0.1f;

	public bool right_trigger;

	public bool left_trigger;

	public GameObject m_left_yeah;
	public GameObject m_right_yeah;

	public GameObject left_hand_to_delete;

	public GameObject right_hand_to_delete;

	public bool no_more_hands;

	//attribute used to load a level in a match
	FileNamesOfPaths loaded_path = new FileNamesOfPaths ();

	//attribute used to load a replay
	ReplayNamesOfPaths path_to_replay = new ReplayNamesOfPaths ();

	string current_music_name = "";


	// Use this for initialization
	void Start ()
	{

		GameManager.Instance.BaseStart ("MusicMenuMusic", GameMatch.GameType.Music);

		/* set the bool of the current game in the game manager 
		 * and in the GUI manager
		 */



		GameManager.Instance.player.SetActive (false);
		ClearScreens ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameManager.Instance.BaseUpdate ();

		if (GameManager.Instance.Get_Is_Playing ()) {

			foreach (GameObject button in GameObject.FindGameObjectsWithTag ("LeftButton")) {
				button.transform.position = new Vector3 (
					button.transform.position.x + (Time.deltaTime * m_button_movement_offset),
					button.transform.position.y,
					button.transform.position.z);
			}

			foreach (GameObject button in GameObject.FindGameObjectsWithTag ("RightButton")) {
				button.transform.position = new Vector3 (
					button.transform.position.x + (Time.deltaTime * m_button_movement_offset),
					button.transform.position.y,
					button.transform.position.z);
			}


			if (GameObject.FindGameObjectsWithTag ("RightButton").Length == 0
			    && GameObject.FindGameObjectsWithTag ("LeftButton").Length == 0
			    && no_more_hands && !MusicManager.Instance.isPlaying (current_music_name)) {
				WinLevel ();		
			}
		} else {
			//blocca mani e musica
		}

		//TODO if music is finished the game ends

		
	}

	public void ChooseLevel (FileNamesOfPaths path)
	{
		no_more_hands = false;

		loaded_path = path;
		current_music_name = loaded_path.name;

		MusicPathGenerator.Instance.SetupMusicPath (path.file_path);
		GameManager.Instance.BaseChooseLevel (path.name);

	}


	//@Overload for the Replay of a match
	public void ChooseLevel (ReplayNamesOfPaths path)
	{
		path_to_replay = path;
		MatchDataExtractor extractor = GetComponent<MatchDataExtractor> ();
		SetGestureThresholds thresholds_setter = GetComponent<SetGestureThresholds> ();

		current_music_name = extractor.FromMatchDataToLevelName (path.match_data_path);

		GameManager.Instance.BaseChooseLevel (path, extractor.FromMatchDataToLevelName (path.match_data_path));
		ResetPath ();

		//load the level from the GameMatch data extracted from the ReplayNamesOfPaths class element
		MusicPathGenerator.Instance.SetupMusicPath (extractor.FromMatchDataToMusicFilePath (path.match_data_path));


		/* the extractor.FromMatchDataSetGlobalPlayerData(path.match_data_path) sets the Thresholds in the GlobalPlayerData instance
		 * 
		 * the thresholds_setter.SetThresholds () call the function in the GestureRecognizer script 
		 * that takes the thresholds from the GlobalPlayerData instance and writes them in the thesholds private attributes
		 */
		extractor.FromMatchDataSetGlobalPlayerData (path.match_data_path);
		thresholds_setter.SetThresholds ();

	}


	public void ToMenu ()
	{
		ResetPath ();
		GameManager.Instance.BaseToMenu ();
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

	void ResetPath ()
	{

		foreach (GameObject button in GameObject.FindGameObjectsWithTag ("LeftButton")) {
			Destroy (button);
		}

		foreach (GameObject button in GameObject.FindGameObjectsWithTag ("RightButton")) {
			Destroy (button);
		}
	}

	public void WinLevel ()
	{
		MusicManager.Instance.StopAll ();
		MusicManager.Instance.PlayMusic ("MusicMenuMusic");
		GameManager.Instance.BaseWinLevel ();
	}


	public int GetScore ()
	{
		return GameManager.Instance.BaseGetScore ();
	}


	/* the trigger OnTriggerEnter of CoinCollectScript sets the bool left|right_trigger that is checked by the PushGesture script
	 * if both the button is inside the trigger and the player has done the push gesture
	 * the PushGesture script calls the AddPoints() funcion
	 */

	public void AddPoints (bool left)
	{
		GameManager.Instance.BaseAddPoints (10);
		StartCoroutine (Yeah (left));

	}

	IEnumerator Yeah (bool left)
	{
		if (!left) {
			m_right_yeah.SetActive (true);	
			right_trigger = false;
			if (right_hand_to_delete != null) {
				right_hand_to_delete.SetActive (false);
			}
		} else {
			m_left_yeah.SetActive (true);
			left_trigger = false;
			if (left_hand_to_delete != null) {
				left_hand_to_delete.SetActive (false);
			}

		}

		yield return new WaitForSeconds (0.5f);
		ClearScreens ();
	}


	void ClearScreens ()
	{
		if (m_left_yeah != null)
			m_left_yeah.SetActive (false);
		if (m_right_yeah != null)
			m_right_yeah.SetActive (false);
		
	}
}
