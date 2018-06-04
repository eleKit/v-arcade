using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using POLIMIGameCollective;
using System.IO;
using System;

public class GameMenuScript : Singleton<GameMenuScript>
{
	public GameObject m_info_screen;

	public GameObject m_level_screen;

	public GameObject m_main_screen;

	public GameObject m_pause_screen;


	public GameObject m_no_level_text;

	public GameObject m_win_screen;
	public Text m_win_text;

	//only for car game
	public GameObject m_mode_screen;
	public GameObject m_car_colours_screen;


	public Button[] m_level_button;

	public bool car, music, shooting;

	bool there_are_no_level;

	//retreive the name levels from file into list, convert the list into a new array

	int index_of_current_level_screen;

	string directoryPath;

	string music_dataPath = "Assets/MusicTexts";

	FileNamesOfPaths[] file_names_of_paths;

	void Awake ()
	{
		car = false;
		shooting = false;
		music = false;
		there_are_no_level = false;
	}

	// Use this for initialization
	void Start ()
	{


	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	//this is called once at the start of game manager to set the initial paramethers
	public void LoadUIOfGame (GameMatch.GameType game_type)
	{
		ButtonsNonInteractable ();


		switch (game_type) {
		case GameMatch.GameType.Car:
			car = true;
			break;
		case GameMatch.GameType.Shooting:
			shooting = true;
			break;
		case GameMatch.GameType.Music:
			music = true;
			break;
		}


		LoadLevelNames (game_type);
		LoadFirstLevels ();
		LoadMenu ();
	}

	void ClearScreens ()
	{
		if (m_info_screen != null)
			m_info_screen.SetActive (false);
		
		if (m_no_level_text != null)
			m_no_level_text.SetActive (false);

		if (m_level_screen != null)
			m_level_screen.SetActive (false);

		if (m_main_screen != null)
			m_main_screen.SetActive (false);

		if (m_pause_screen != null)
			m_pause_screen.SetActive (false);

		if (m_win_screen != null)
			m_win_screen.SetActive (false);
		if (car) {
			if (m_mode_screen != null)
				m_mode_screen.SetActive (false);
			if (m_car_colours_screen != null)
				m_car_colours_screen.SetActive (false);
		}

	}


	public void LoadMainMenu ()
	{
		//everytime a new scene is loaded the MusicManager must stop 
		MusicManager.Instance.StopAll ();

		//TODO fix better this bug! 
		Physics2D.gravity = new Vector3 (0f, -9.81f, 0f);

		SceneManager.LoadSceneAsync ("Main_Menu_Patient");
	}

	/* use this only from Instruction Screen and from Level Screen
	 * otherwise use FromGameToMenu()
	 */
	public void LoadMenu ()
	{

		ClearScreens ();
		m_main_screen.SetActive (true);

		if (there_are_no_level)
			m_no_level_text.SetActive (true);
	}

	public void FromGameToMenu ()
	{
		if (car) {
			CarManager.Instance.ToMenu ();
		}
		if (shooting) {
			ShootingManager.Instance.ToMenu ();
		}
		if (music) {
			MusicGameManager.Instance.ToMenu ();
		}
		LoadMenu ();
	}

	public void LoadInstructions ()
	{
		ClearScreens ();
		m_info_screen.SetActive (true);
	}


	public void LoadWinScreen (int score)
	{
		ClearScreens ();
		m_win_screen.SetActive (true);
		m_win_text.text = score.ToString ();
	}

	public void LoadPauseScreen ()
	{
		ClearScreens ();
		m_pause_screen.SetActive (true);
	}

	public void LoadLevelScreen ()
	{
		ClearScreens ();
		m_level_screen.SetActive (true);
		LoadFirstLevels ();
	}

	public void LoadModeScreen ()
	{
		if (car) {
			ClearScreens ();
			m_mode_screen.SetActive (true);
		} else {
			LoadLevelScreen ();
		}
	}

	public void LoadCarColourScreen ()
	{
		ClearScreens ();
		m_car_colours_screen.SetActive (true);
	}


	public void LoadGameLevel (int button_index)
	{
		ClearScreens ();

		if (car) {
			CarManager.Instance.ChooseLevel (file_names_of_paths [button_index + index_of_current_level_screen]);
		}
		if (shooting) {
			ShootingManager.Instance.ChooseLevel (file_names_of_paths [button_index + index_of_current_level_screen]);
		}
		if (music) {
			MusicGameManager.Instance.ChooseLevel (file_names_of_paths [button_index + index_of_current_level_screen]);
		}
	}


	public void RestartGame ()
	{
		ClearScreens ();
		if (car) {
			CarManager.Instance.RestartLevel ();
		}
		if (music) {
			MusicGameManager.Instance.RestartLevel ();
		}
		if (shooting) {
			ShootingManager.Instance.RestartLevel ();
		}

	}


	public void ResumeGame ()
	{
		ClearScreens ();
		if (car) {
			CarManager.Instance.ResumeLevel ();
		}
		if (music) {
			MusicGameManager.Instance.ResumeLevel ();
		}
		if (shooting) {
			ShootingManager.Instance.ResumeLevel ();
		}
	}



	public void PushedOnArrow ()
	{
		PushedArrow (+1);
	}

	public void PushedBackArrow ()
	{
		PushedArrow (-1);
	}

	void PushedArrow (int direction)
	{
		if (file_names_of_paths != null) {
			if (index_of_current_level_screen + (m_level_button.Length * direction) < file_names_of_paths.Length
			    && index_of_current_level_screen + (m_level_button.Length * direction) >= 0) {
				index_of_current_level_screen = index_of_current_level_screen + (m_level_button.Length * direction);
				LoadNameButtons ();
			} else if (index_of_current_level_screen + (m_level_button.Length * direction) < 0) {
				if (car) {
					LoadCarColourScreen ();
				} else {
					LoadMenu ();
				}
			}
		} else if (direction == -1) {
			there_are_no_level = true;
			LoadMenu ();

		}
	}

	public void LoadFirstLevels ()
	{
		index_of_current_level_screen = 0;
		LoadNameButtons ();
	}


	void LoadNameButtons ()
	{
		if (file_names_of_paths != null) {
			//TODO retreive name buttons
			MakeButtonsInteractable ();

			for (int i = 0; i < m_level_button.Length; i++) {
				if (i + index_of_current_level_screen < file_names_of_paths.Length) {
					m_level_button [i].GetComponentInChildren<Text> ().text = file_names_of_paths [i + index_of_current_level_screen].name;
				} else {
					//if there are no more levels
					m_level_button [i].GetComponentInChildren<Text> ().text = "";
					m_level_button [i].interactable = false;
				}
			}
		}

	}


	void LoadLevelNames (GameMatch.GameType game_type)
	{
		if (!music) {
			directoryPath = Path.Combine (Application.persistentDataPath,
				Path.Combine ("Paths", game_type.ToString ()));

			if (Directory.Exists (directoryPath)) {
				string[] game_paths = Directory.GetFiles (directoryPath, "*.json");

				file_names_of_paths = new FileNamesOfPaths[game_paths.Length];

				for (int i = 0; i < file_names_of_paths.Length; i++) {
					file_names_of_paths [i] = new FileNamesOfPaths ();
					file_names_of_paths [i].file_path = game_paths [i];
					file_names_of_paths [i].name = FromFilenameToName (Path.GetFileName (game_paths [i]).Split ('_') [1]);
				}

			}
		} else {
			if (Directory.Exists (music_dataPath)) {
				string[] game_paths = Directory.GetFiles (music_dataPath, "*.txt");


				file_names_of_paths = new FileNamesOfPaths[game_paths.Length];


				for (int i = 0; i < file_names_of_paths.Length; i++) {
					file_names_of_paths [i] = new FileNamesOfPaths ();
					file_names_of_paths [i].file_path = game_paths [i];
					file_names_of_paths [i].name = FromFilenameToName (Path.GetFileName (game_paths [i]).Split ('.') [0]);
				}
			}
		}


	}


	string FromFilenameToName (string name)
	{
		return name.Replace ("-", " ");

	}

	void MakeButtonsInteractable ()
	{
		for (int i = 0; i < m_level_button.Length; i++) {
			m_level_button [i].interactable = true;
		}
	}

	void ButtonsNonInteractable ()
	{
		for (int i = 0; i < m_level_button.Length; i++) {
			m_level_button [i].interactable = false;
			m_level_button [i].GetComponentInChildren<Text> ().text = "";
		}
	}
}























