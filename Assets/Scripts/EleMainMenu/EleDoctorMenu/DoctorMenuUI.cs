using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class DoctorMenuUI : MonoBehaviour
{

	public GameObject m_actions_screen;
	public GameObject m_manage_patients_screen;

	public GameObject m_patients_list_screen;
	public Text[] m_text_patients_list;



	public GameObject m_patiens_replay_screen;
	public Button[] m_button_patients_list;


	public GameObject m_replay_games_screen;



	int index_of_current_patients_screen;
	int index_of_current_replay_patients_screen;
	private string[] patients;

	// Use this for initialization
	void Start ()
	{
		LoadActionsMenu ();

		patients = RetreivePatientsName ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	void ClearScreens ()
	{
		if (m_actions_screen != null)
			m_actions_screen.SetActive (false);

		if (m_manage_patients_screen != null)
			m_manage_patients_screen.SetActive (false);

		if (m_patients_list_screen != null)
			m_patients_list_screen.SetActive (false);
		
		if (m_patiens_replay_screen != null)
			m_patiens_replay_screen.SetActive (false);
		
		if (m_replay_games_screen != null)
			m_replay_games_screen.SetActive (false);
	}


	public void LoadActionsMenu ()
	{
		ClearScreens ();
		m_actions_screen.SetActive (true);
	}

	public void LoadManagePatientsScreen ()
	{
		ClearScreens ();
		m_manage_patients_screen.SetActive (true);
	}

	public void LoadPatienstListScreen ()
	{
		ClearScreens ();
		m_patients_list_screen.SetActive (true);
		index_of_current_patients_screen = 0;
		LoadNamePatientsText ();
	}

	public void ToPathGenerator ()
	{
		SceneManager.LoadSceneAsync ("Path_generator");
	}

	public void ToPatientReplayScreen ()
	{
		ClearScreens ();
		m_patiens_replay_screen.SetActive (true);
		index_of_current_replay_patients_screen = 0;
		LoadNamePatientsButtons ();
	}

	public void ToGameReplayScreen ()
	{
		ClearScreens ();
		m_replay_games_screen.SetActive (true);
	}


	public void LoadCarReplayScene ()
	{
		SceneManager.LoadSceneAsync ("Car_replay");
	}

	public void LoadShootingReplayScene ()
	{
		//SceneManager.LoadSceneAsync ("Shooting_replay");
	}

	public void LoadMusicReplayScene ()
	{
		SceneManager.LoadSceneAsync ("Music_replay");
		
	}



	//these are the script used to set the buttons of the replay
	public void PushedOnArrowButtons ()
	{
		PushedArrowButtons (+1);
	}

	public void PushedBackArrowButtons ()
	{
		PushedArrowButtons (-1);
	}

	void PushedArrowButtons (int direction)
	{
		
		if (index_of_current_replay_patients_screen + (m_button_patients_list.Length * direction) < patients.Length
		    && index_of_current_replay_patients_screen + (m_button_patients_list.Length * direction) >= 0) {
			index_of_current_replay_patients_screen = index_of_current_replay_patients_screen + (m_button_patients_list.Length * direction);
			LoadNamePatientsButtons ();
		}
	
	}

	void LoadNamePatientsButtons ()
	{
		if (patients.Length == 1 && patients [0].Equals (" ")) { //if there are no patients
			ButtonsNonInteractable ();
		
		} else {

			MakeButtonsInteractable ();

			for (int i = 0; i < m_button_patients_list.Length; i++) {
				if (i + index_of_current_replay_patients_screen < patients.Length) {
					m_button_patients_list [i].GetComponentInChildren<Text> ().text = patients [i + index_of_current_replay_patients_screen];
				} else {
					//if there are no more patients
					m_button_patients_list [i].GetComponentInChildren<Text> ().text = "";
					m_button_patients_list [i].interactable = false;
				}
			}
		}

	}


	//these are the script used to show the patients list in the "patients manager screen"
	public void PushedOnArrowTexts ()
	{
		PushedArrowText (+1);
	}

	public void PushedBackArrowTexts ()
	{
		PushedArrowText (-1);
	}

	void PushedArrowText (int direction)
	{
		
		if (index_of_current_patients_screen + (m_text_patients_list.Length * direction) < patients.Length
		    && index_of_current_patients_screen + (m_text_patients_list.Length * direction) >= 0) {
			index_of_current_patients_screen = index_of_current_patients_screen + (m_text_patients_list.Length * direction);
			LoadNamePatientsText ();
		}

	}


	void LoadNamePatientsText ()
	{

		for (int i = 0; i < m_text_patients_list.Length; i++) {
			if (i + index_of_current_patients_screen < patients.Length) {
				m_text_patients_list [i].text = patients [i + index_of_current_patients_screen];
			} else {
				//if there are no more patients
				m_text_patients_list [i].text = "";
			}
		}
	}




	void MakeButtonsInteractable ()
	{
		for (int i = 0; i < m_button_patients_list.Length; i++) {
			m_button_patients_list [i].interactable = true;
		}
	}

	void ButtonsNonInteractable ()
	{
		for (int i = 0; i < m_button_patients_list.Length; i++) {
			m_button_patients_list [i].interactable = false;
			m_button_patients_list [i].GetComponentInChildren<Text> ().text = "";
		}
	}





	string[] RetreivePatientsName ()
	{
		string directoryPath = Path.Combine (Application.persistentDataPath, "Patients");

		if (Directory.Exists (directoryPath)) {
			string[] patients_paths = Directory.GetDirectories (directoryPath);
		
			string[] patients_names = new string[patients_paths.Length]; 
			
			for (int i = 0; i < patients_paths.Length; i++) {
				patients_names [i] = Path.GetFileName (patients_paths [i]);
			}

			return patients_names;
		} else {
			return new string[] { " " };
		}
		
	}




}
