using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;
using System.Linq;

public class ReplayManagerUI : Singleton<ReplayManagerUI>
{

	public GameMatch.GameType gameType;

	public GameObject replay_list_screen;
	public GameObject pause_screen;
	public GameObject end_screen;

	public Button[] replay_list_buttons;

	string directoryPath;

	ReplayNamesOfPaths[] names_of_replays;

	int index_of_current_replay_screen;

	bool there_are_no_replay;



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


		if (gameType.Equals (GameMatch.GameType.Car)) {
			CarManager.Instance.ChooseLevel (names_of_replays [button_index + index_of_current_replay_screen]);
		}

		/*if (shooting) {
			ShootingReplayManager.Instance.ChooseLevel (names_of_replays [button_index + index_of_current_replay_screen]);
			+ loadreplay
			}
		
		if (music) {
			MusicGameReplayManager.Instance.ChooseLevel (names_of_replays [button_index + index_of_current_replay_screen]);
			 loadreplay
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
					replay_list_buttons [i].GetComponentInChildren<Text> ().text = names_of_replays [i + index_of_current_replay_screen].button_name;
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

			//save the hand data files
			string[] replay_paths = Directory.GetFiles (directoryPath, "*_hand_data.json");


			names_of_replays = new ReplayNamesOfPaths[replay_paths.Length];

			for (int i = 0; i < names_of_replays.Length; i++) {

				/* every time i instantiate a new Class[] array
				 * i have also to instantiate every element of the array [i] as a new Class
				 */

				names_of_replays [i] = new ReplayNamesOfPaths ();
		
				// save hand data file path 
				names_of_replays [i].hand_data_path = replay_paths [i];


				// using the 
				string match_date = Path.GetFileName (replay_paths [i]).Split ('_') [1];

				names_of_replays [i].match_data_path = Path.Combine (directoryPath, gameType.ToString () + "_" + match_date + ".json");
				Debug.Log ("complete match_path " + names_of_replays [i].match_data_path);
			




				/*convert the TimeStamp FileTimeUtc into a uman readable string
				 * the readable TS string is saved into the button_name attribute 
				*/
				char[] date_chars = Path.GetFileName (replay_paths [i]).Split ('_') [1].Split ('T') [0].ToCharArray ();

				char[] time_chars = Path.GetFileName (replay_paths [i]).Split ('_') [1].Split ('T') [1].ToCharArray ();

				for (int j = 0; j < date_chars.Length; j++) {
					if (j < 4 || !(j % 2 == 0)) {
						names_of_replays [i].button_name = names_of_replays [i].button_name + date_chars [j];
					} else {
						names_of_replays [i].button_name = names_of_replays [i].button_name + "/" + date_chars [j];
					}
				}
					
				names_of_replays [i].button_name = names_of_replays [i].button_name + "\n";

				for (int j = 0; j < time_chars.Length; j++) {
					if (!(j % 2 == 0) || j == 0) {
						names_of_replays [i].button_name = names_of_replays [i].button_name + time_chars [j];
					} else {
						names_of_replays [i].button_name = names_of_replays [i].button_name + ":" + time_chars [j];
					}
				}

				/* end conversion */


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
