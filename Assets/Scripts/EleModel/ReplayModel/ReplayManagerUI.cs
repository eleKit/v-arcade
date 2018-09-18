using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

public class ReplayManagerUI : Singleton<ReplayManagerUI>
{

	public GameMatch.GameType gameType;

	public GameObject replay_list_screen;
	public GameObject pause_screen;
	public GameObject end_screen;

	public Button[] replay_list_buttons;

	string directoryPath;

	FileNamesOfPaths[] names_of_replays;

	int index_of_current_replay_screen;

	bool there_are_no_replay;

	bool car, music, shooting;


	void Awake ()
	{
		
		directoryPath = Path.Combine (Application.persistentDataPath, 
			Path.Combine ("Patients", Path.Combine (GlobalReplayData.globalReplayData.patient_folder_name, gameType.ToString ())));

		there_are_no_replay = false;
	}



	// Use this for initialization
	void Start ()
	{
		LoadReplayUI ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}



	void ClearScreens ()
	{
		if (replay_list_screen != null)
			replay_list_screen.SetActive (false);

		if (pause_screen != null)
			pause_screen.SetActive (false);

		if (end_screen != null)
			end_screen.SetActive (false);
	}


	public void LoadReplayUI ()
	{


		ClearScreens ();

		//TODO load replay from web

		LoadReplayNames ();
		LoadReplayListScreen ();
	}

	public void LoadDoctorMenu ()
	{
		SceneManager.LoadSceneAsync ("Main_Menu_Doctor");
	}

	public void LoadReplayListScreen ()
	{
		ClearScreens ();
		replay_list_screen.SetActive (true);
		LoadFirstReplayButtons ();
	}



	public void LoadPauseScren ()
	{
		ClearScreens ();
		pause_screen.SetActive (true);
	}

	public void LoadEndScreen ()
	{
		ClearScreens ();
		end_screen.SetActive (true);
	}




	public void LoadReplayLevel (int button_index)
	{


		ClearScreens ();

		/*if (car) {
			CarReplayManager.Instance.ChooseLevel (names_of_replays [button_index + index_of_current_replay_screen]);
		}
		if (shooting) {
			ShootingReplayManager.Instance.ChooseLevel (names_of_replays [button_index + index_of_current_replay_screen]);
		}
		if (music) {
			MusicGameReplayManager.Instance.ChooseLevel (names_of_replays [button_index + index_of_current_replay_screen]);
		}*/
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
		if (names_of_replays != null) {
			if (index_of_current_replay_screen + (replay_list_buttons.Length * direction) < names_of_replays.Length
			    && index_of_current_replay_screen + (replay_list_buttons.Length * direction) >= 0) {
				index_of_current_replay_screen = index_of_current_replay_screen + (replay_list_buttons.Length * direction);
				LoadReplayButtons ();
			} else if (index_of_current_replay_screen + (replay_list_buttons.Length * direction) < 0) {

				LoadDoctorMenu ();
			}
		} else if (direction == -1) {
			there_are_no_replay = true;
			LoadDoctorMenu ();

		}
	}

	void LoadReplayButtons ()
	{
		if (names_of_replays != null) {
			
			MakeButtonsInteractable ();

			for (int i = 0; i < replay_list_buttons.Length; i++) {
				if (i + index_of_current_replay_screen < names_of_replays.Length) {
					replay_list_buttons [i].GetComponentInChildren<Text> ().text = names_of_replays [i + index_of_current_replay_screen].name;
				} else {
					//if there are no more replays
					replay_list_buttons [i].GetComponentInChildren<Text> ().text = "";
					replay_list_buttons [i].interactable = false;
				}
			}
		}

	}

	public void LoadFirstReplayButtons ()
	{
		index_of_current_replay_screen = 0;
		LoadReplayButtons ();
	}


	void LoadReplayNames ()
	{
		if (Directory.Exists (directoryPath)) {
			string[] game_paths = Directory.GetFiles (directoryPath, "*_hand_data.json");


			names_of_replays = new FileNamesOfPaths[game_paths.Length];

			for (int i = 0; i < names_of_replays.Length; i++) {
				names_of_replays [i] = new FileNamesOfPaths ();
				names_of_replays [i].file_path = game_paths [i];

				//DateTime replay_date = DateTime.FromFileTimeUtc (Int64.Parse (Path.GetFileName (game_paths [i]).Split ('_') [1])).ToLocalTime ();

				//names_of_replays [i].name = replay_date.ToLongDateString () + " " + replay_date.ToLongTimeString ();

				names_of_replays [i].name = Path.GetFileName (game_paths [i]).Split ('_') [1];
			}

		}
	
	}






	void MakeButtonsInteractable ()
	{
		for (int i = 0; i < replay_list_buttons.Length; i++) {
			replay_list_buttons [i].interactable = true;
		}
	}


		

}
