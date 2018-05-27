using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuScript : MonoBehaviour
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

	public bool car;

	//retreive the name levels from file into list, convert the list into a new array
	string[] name_levels;
	int index_of_current_level_screen;

	// Use this for initialization
	void Start ()
	{
		LoadLevelNames ();
		LoadFirstLevels ();
		LoadMenu ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
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

	public void LoadMenu ()
	{
		ClearScreens ();
		m_main_screen.SetActive (true);
	}

	public void LoadInstructions ()
	{
		ClearScreens ();
		m_info_screen.SetActive (true);
	}

	public void LoadModeMenu ()
	{
		ClearScreens ();
		m_mode_screen.SetActive (true);
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


	void LoadLevelNames ()
	{
		name_levels = new string[4]{ "a", "b", "c", "d" };
	}

	void MakeButonsInteractable ()
	{
		for (int i = 0; i < m_level_button.Length; i++) {
			m_level_button [i].interactable = true;
		}
	}
}























