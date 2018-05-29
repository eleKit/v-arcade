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

	public GameObject m_win_screen;
	public Text m_win_text;

	//only for car game
	public GameObject m_mode_screen;


	public Button[] m_level_button;

	public bool car, music, shooting;

	//retreive the name levels from file into list, convert the list into a new array
	string[] name_levels;
	int index_of_current_level_screen;

	string directoryPath;

	string[] game_paths;

	string music_dataPath = "Assets/MusicTexts";

	void Awake ()
	{
		car = false;
		shooting = false;
		music = false;
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
		}

	}


	public void LoadMainMenu ()
	{
		SceneManager.LoadSceneAsync ("Main_Menu");
	}

	/* use this only from Instruction Screen and from Level Screen
	 * otherwise use FromGameToMenu()
	 */
	public void LoadMenu ()
	{

		ClearScreens ();
		m_main_screen.SetActive (true);
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



	public void LoadGameLevel (int button_index)
	{
		ClearScreens ();

		if (car) {
			CarManager.Instance.ChooseLevel (game_paths [button_index + index_of_current_level_screen]);
		}
		if (shooting) {
			ShootingManager.Instance.ChooseLevel (game_paths [button_index + index_of_current_level_screen]);
		}
		if (music) {
			MusicGameManager.Instance.ChooseLevel (game_paths [button_index + index_of_current_level_screen]);
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
		if (index_of_current_level_screen + (m_level_button.Length * direction) < name_levels.Length
		    && index_of_current_level_screen + (m_level_button.Length * direction) >= 0) {
			index_of_current_level_screen = index_of_current_level_screen + (m_level_button.Length * direction);
			LoadNameButtons ();
		} else if (index_of_current_level_screen + (m_level_button.Length * direction) < 0) {
			if (car) {
				LoadModeScreen ();
			} else {
				LoadMenu ();
			}
		}
	}

	public void LoadFirstLevels ()
	{
		index_of_current_level_screen = 0;
		LoadNameButtons ();
	}


	void LoadNameButtons ()
	{
		//TODO retreive name buttons
		MakeButonsInteractable ();

		for (int i = 0; i < m_level_button.Length; i++) {
			if (i + index_of_current_level_screen < name_levels.Length) {
				m_level_button [i].GetComponentInChildren<Text> ().text = name_levels [i + index_of_current_level_screen];
			} else {
				//if there are no more levels
				m_level_button [i].GetComponentInChildren<Text> ().text = "";
				m_level_button [i].interactable = false;
			}
		}

	}


	void LoadLevelNames (GameMatch.GameType game_type)
	{
		if (!music) {
			directoryPath = 
			Path.Combine (Application.persistentDataPath, game_type.ToString ());
			game_paths = Directory.GetFiles (directoryPath);

			name_levels = new string[game_paths.Length];
			Char delimiter = '_';
			string file_name_level = "";
			for (int i = 0; i < game_paths.Length; i++) {

				name_levels [i] = Path.GetFileName (game_paths [i]).Split (delimiter) [1];
			}
		} else {
			string[] game_paths = Directory.GetFiles (music_dataPath);


			//TODO ho un problema perchè vede il .meta
			name_levels = new string[game_paths.Length];
			Char delimiter = '.';
			string file_name_level = "";
			for (int i = 0; i < game_paths.Length; i++) {

				name_levels [i] = Path.GetFileName (game_paths [i]).Split (delimiter) [0];
			}
		}


	}

	void MakeButonsInteractable ()
	{
		for (int i = 0; i < m_level_button.Length; i++) {
			m_level_button [i].interactable = true;
		}
	}
}























