using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;
using POLIMIGameCollective;

public class DoctorMenuUI : MonoBehaviour
{

	[Header ("Use for debug, if checked the match is saved into test server")]
	public bool debugging_save;

	public GameObject load_and_save_data_on_web;

	public GameObject m_actions_screen;
	public GameObject m_manage_patients_screen;

	public GameObject m_patients_list_screen;
	public Text[] m_text_patients_list;



	public GameObject m_patiens_replay_screen;
	public Button[] m_button_patients_list;

	public GameObject m_patient_saved_correctly;
	public GameObject m_wrong_patient_not_saved;

	public GameObject m_replay_games_screen;

	//attributes used for retreiving and saving new patients
	string m_new_patient_name = "";

	string directoryPath;
	string filePath;

	PatientsList all_saved_patients;

	int index_of_current_patients_screen;
	int index_of_current_replay_patients_screen;
	private string[] patients;

	// Use this for initialization
	void Start ()
	{
		

		directoryPath = Path.Combine (Application.persistentDataPath, "Patient_List");
		if (!Directory.Exists (directoryPath)) {
			Directory.CreateDirectory (directoryPath);
		}

		StartCoroutine (LoadNick ());

		//filePath = Path.Combine (directoryPath, "patients_list.json");


	}

	IEnumerator LoadNick ()
	{
		yield return FindObjectOfType<LoadNicknamesFromWeb> ().LoadFileOfNicknames ();
		yield return FindObjectOfType<LoadPathsFromWeb> ().LoadFilenames ();
		load_and_save_data_on_web.SetActive (true);
		filePath = Path.Combine (directoryPath, "patients_list.json");
		LoadActionsMenu ();

		patients = RetreivePatientsName ();

		all_saved_patients = new PatientsList ();

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

	void ClearTexts ()
	{
		if (m_patient_saved_correctly != null)
			m_patient_saved_correctly.SetActive (false);

		if (m_wrong_patient_not_saved != null)
			m_wrong_patient_not_saved.SetActive (false);


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
		ClearTexts ();
	}

	public void LoadPatienstListScreen ()
	{
		ClearScreens ();
		m_patients_list_screen.SetActive (true);
		patients = RetreivePatientsName ();
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
		MusicManager.Instance.StopAll ();
		SceneManager.LoadSceneAsync ("Car_replay");
	}

	public void LoadShootingReplayScene ()
	{
		MusicManager.Instance.StopAll ();
		SceneManager.LoadSceneAsync ("Shooting_replay");
	}

	public void LoadSpaceReplayScene ()
	{
		MusicManager.Instance.StopAll ();
		SceneManager.LoadSceneAsync ("Space_replay");
	}

	public void LoadMusicReplayScene ()
	{
		MusicManager.Instance.StopAll ();
		SceneManager.LoadSceneAsync ("Music_replay");
		
	}


	public void SetPatientNameForReplay (int index)
	{
		//TODO recuperare qui tutti i dati di tuning da inserire nel globar player data e usare lo stesso metodo nella futura scena di tuning threshold selections fatte dal medico
		GlobalPlayerData.globalPlayerData.player = patients [index + index_of_current_replay_patients_screen];
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





	/*string[] RetreivePatientsName ()
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
	*/

	//Retreive patients name from the list of names
	string[] RetreivePatientsName ()
	{
		
		if (Directory.Exists (directoryPath)) {

			string patients_list = File.ReadAllText (filePath);

			all_saved_patients = JsonUtility.FromJson<PatientsList> (patients_list);

			string[] patients_names = new string[all_saved_patients.patients.Count]; 

			for (int i = 0; i < all_saved_patients.patients.Count; i++) {
				patients_names [i] = all_saved_patients.patients [i];
			}

			return patients_names;
		} else {
			return new string[] { " " };
		}
			
	}


	public void SaveNewPatientName (string n)
	{
		m_new_patient_name = n;
	}


	public void SaveNewPatient ()
	{
		ClearTexts ();

		bool found_equal_name = false;

		//if directory exists check for duplicated files
		if (Directory.Exists (directoryPath)) {
			string patients_list = File.ReadAllText (filePath);

			all_saved_patients = JsonUtility.FromJson<PatientsList> (patients_list);
			foreach (string name in all_saved_patients.patients) {
				if (m_new_patient_name.Equals (name)) {
					found_equal_name = true;
				}
			}		
		}
		if (m_new_patient_name.Equals ("") || found_equal_name || m_new_patient_name.Contains (" ") || m_new_patient_name.Contains ("_")) {
			m_wrong_patient_not_saved.SetActive (true);
		} else {

			//if the directory does not exist the name is added into an empty list
			all_saved_patients.patients.Add (m_new_patient_name);

			Directory.CreateDirectory (directoryPath);

			string jsonString = JsonUtility.ToJson (all_saved_patients);
			File.WriteAllText (filePath, jsonString);


			StartCoroutine (SaveNewPatientCoroutine (filePath, jsonString));
			m_patient_saved_correctly.SetActive (true);
		}
	}


	IEnumerator SaveNewPatientCoroutine (string namesfilePath, string namesString)
	{
		string address; 
		string webfilename = "patients_list" + ".json";

		if (debugging_save) {
			address = "http://127.0.0.1/ES2.php?webfilename=";
			Debug.Log ("Debugging save");
		} else {
			address = "http://data.polimigamecollective.org/demarchi/ES2.php?webfilename=";
		}

		string myURL = address + webfilename;
		// Upload the entire local file to the server.
		ES2Web web = new ES2Web (myURL);


		yield return StartCoroutine (web.UploadFile (namesfilePath));

		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
			string directoryPath = Path.Combine (Application.persistentDataPath, "TMP_web_saving");
			Directory.CreateDirectory (directoryPath);
			string path = Path.Combine (directoryPath, webfilename);
			File.WriteAllText (path, namesString);
		}
	}
}
