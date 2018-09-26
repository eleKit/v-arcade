using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;
using System;
using System.IO;

public class FisioCarPathGenerator : Singleton<FisioCarPathGenerator>
{
	

	//num of possible curve positions for each section
	const int CURVES = 4;


	public Button[] m_start_buttons = new Button[CURVES - 1];

	public Button[] m_middle_buttons = new Button[CURVES];

	public Button[] m_final_buttons = new Button[CURVES];

	public Slider[] diamonds_slider = new Slider[SECTIONS];
	public Text[] diamonds_text = new Text[SECTIONS];


	public Slider amplitude_slider;
	public Text amplitude_text;

	public CarPath car_path;


	string name_path = "";



	//Attributes used to activate|deactivate go_on button
	public Button button_go_on;

	bool start_ok;

	bool middle_ok;

	bool final_ok;

	bool activated;


	//num of scetions
	const int INITIAL = 0;
	const int MIDDLE = 1;
	const int FINAL = 2;
	const int SECTIONS = 3;

	int[] curve_values = new int[SECTIONS];

	// Use this for initialization
	void Start ()
	{ 
		car_path = new CarPath ();

		ResetAll ();
		
	}

	// Update is called once per frame
	void Update ()
	{
		if (start_ok && middle_ok && final_ok && !activated) {
			activated = true;
			button_go_on.interactable = true;
		}
		
	}


	public void ResetAll ()
	{
		start_ok = false;

		middle_ok = false;

		final_ok = false;

		activated = false;

		amplitude_slider.value = amplitude_slider.minValue;
		amplitude_text.text = "Ampiezza curva: " + amplitude_slider.value.ToString ();

		for (int i = 0; i < m_start_buttons.Length; i++) {
			m_start_buttons [i].interactable = true;
		}

		for (int i = 0; i < m_middle_buttons.Length; i++) {
			m_middle_buttons [i].interactable = true;
		}

		for (int i = 0; i < m_final_buttons.Length; i++) {
			m_final_buttons [i].interactable = true;
		}



		for (int i = 0; i < diamonds_slider.Length; i++) {
			diamonds_slider [i].value = diamonds_slider [i].minValue;
			diamonds_slider [i].interactable = true;
			diamonds_text [i].text = diamonds_slider [i].minValue.ToString ();
		}

		for (int i = 0; i < curve_values.Length; i++) {
			curve_values [i] = 0;
		}

		button_go_on.interactable = false;
	}



	public void StartPressed (int i)
	{
		start_ok = true;


		for (int h = 0; h < m_start_buttons.Length; h++) {
			if (h != i)
				m_start_buttons [h].interactable = true;
		}

		m_start_buttons [i].interactable = false;

	}


	public void MiddlePressed (int i)
	{
		middle_ok = true;


		for (int h = 0; h < m_middle_buttons.Length; h++) {

			//if the button is the "Target" button
			if (m_middle_buttons [h].tag.Equals ("Goal")) {

				if (h == i) {                                         //if "Target" has been pressed now
					DeactivateMiddleButtons (MIDDLE);
				} else if (!m_middle_buttons [h].interactable) {      //if "Target" has been pressed before
					ActivateButtons ();
				} else {
					//do nothing
				}
			}
			m_middle_buttons [h].interactable = true;
		}

		m_middle_buttons [i].interactable = false;

	}


	public void FinalPressed (int i)
	{
		final_ok = true;


		for (int h = 0; h < m_final_buttons.Length; h++) {

			//if the button is the "Target" button
			if (m_final_buttons [h].tag.Equals ("Goal")) {
				if (h == i) {                                         //if "Target" has been pressed now
					DeactivateFinalButtons (FINAL);
				} else if (!m_final_buttons [h].interactable) {      //if "Target" has been pressed before

					//reactivate the non iteractable item slider
					for (int k = 0; k < diamonds_slider.Length; k++) {
						if (!diamonds_slider [k].interactable)
							diamonds_slider [k].interactable = true;
					}
				} else {
					//do nothing
				}
					
			}
				
			m_final_buttons [h].interactable = true;

		}

		m_final_buttons [i].interactable = false;
	}


	void ActivateButtons ()
	{

		for (int h = 0; h < m_final_buttons.Length; h++) {
			m_final_buttons [h].interactable = true;
		}

		for (int h = 0; h < diamonds_slider.Length; h++) {
			if (!diamonds_slider [h].interactable)
				diamonds_slider [h].interactable = true;
		}
			

		//deactivate the "Continua" button
		final_ok = false;

		button_go_on.interactable = false;

		activated = false;
	}


	void DeactivateMiddleButtons (int diamond_index)
	{
		middle_ok = true;

		for (int h = 0; h < m_final_buttons.Length; h++) {
			m_final_buttons [h].interactable = false;
		}

		DeactivateFinalButtons (diamond_index);



	}

	void DeactivateFinalButtons (int diamond_index)
	{
		final_ok = true;



		for (int h = 0; h < diamonds_slider.Length; h++) {
			if (h >= diamond_index)
				diamonds_slider [h].interactable = false;
		}
		
	}




	public void ManageSlider (int index)
	{
		diamonds_text [index].text = 
			Mathf.RoundToInt (diamonds_slider [index].value)
				.ToString ();
	}

	public void ManageAmplitudeSlider (float value)
	{
		amplitude_text.text = "Ampiezza curva: " + Mathf.RoundToInt (value).ToString ();
	}


	//From here there are the path curve index scripts


	public void OnStartSectionClick (int value)
	{
		curve_values [INITIAL] = value;
	}

	public void OnMiddleSectionClick (int value)
	{
		curve_values [MIDDLE] = value;
	}

	public void OnFinalSectionClick (int value)
	{
		curve_values [FINAL] = value;
	}





	public void SaveCarData ()
	{
		bool path_already_ended = false;

		for (int i = 0; i < curve_values.Length; i++) {
			if (curve_values [i] == -2 && !path_already_ended) {
				car_path.car_sections = new CarSection[i];
				path_already_ended = true;
			}
		}

		if (!path_already_ended) {
			car_path.car_sections = new CarSection[SECTIONS];
		}

		for (int i = 0; i < car_path.car_sections.Length; i++) {
			car_path.car_sections [i] = new CarSection ();
		}
			

		for (int i = 0; i < car_path.car_sections.Length; i++) {
			car_path.car_sections [i].curve_position = curve_values [i];
			car_path.car_sections [i].num_items = Mathf.RoundToInt (diamonds_slider [i].value);

			Debug.Log ("Curva all'indice " + i.ToString () + " valore: " + car_path.car_sections [i].curve_position.ToString ());
			Debug.Log ("Items all'indice " + i.ToString () + " valore: " + car_path.car_sections [i].num_items.ToString ());
		}

		car_path.curve_amplitude = amplitude_slider.value;
		Debug.Log ("amplitude " + car_path.curve_amplitude.ToString ());
		
		
	}


	public void SetPathName (string name)
	{
		name_path = name;
		Debug.Log (name_path);

	}


	string FromFilenameToName (string name)
	{
		return name.Replace ("-", " ");

	}

	public void SaveCarPath ()
	{
		//different paths with the same name are avoided!

		bool found_equal_name = false;

		//TODO if the paths are divided into doctors substitute GameMatch.GameType.Car.ToString () with car_path.doctorName
		string directoryPath = Path.Combine (Application.persistentDataPath,
			                       Path.Combine ("Paths", GameMatch.GameType.Car.ToString ()));
		

		if (Directory.Exists (directoryPath)) {
			string[] game_paths = Directory.GetFiles (directoryPath, "*.json");

			for (int i = 0; i < game_paths.Length; i++) {
				if (FromFilenameToName (name_path).Equals 
					(FromFilenameToName 
						(Path.GetFileName (game_paths [i]).Split ('_') [1]))) {
					found_equal_name = true;
				}
			}
		}

		if (name_path.Equals ("") || found_equal_name) {
			//do nothing
		} else {


			DateTime gameDate = DateTime.UtcNow;
			car_path.timestamp = gameDate.ToFileTimeUtc ();
			car_path.doctorName = GlobalDoctorData.globalDoctorData.doctor;

			car_path.id_path = name_path;


			Directory.CreateDirectory (directoryPath);
			string filePath = Path.Combine (
				                  directoryPath,
				                  GameMatch.GameType.Car.ToString () + "_"
				                  + FromNameToFilename (name_path) + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json"
			                  );

			string jsonString = JsonUtility.ToJson (car_path);
			File.WriteAllText (filePath, jsonString);

			StartCoroutine (SavePathDataCoroutine (filePath, gameDate));


			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().name);
		}
		
	}



	IEnumerator SavePathDataCoroutine (string filePath, DateTime gameDate)
	{

		string myURL = "http://127.0.0.1/ES2.php?webfilename="
		               + "path_" + GameMatch.GameType.Car.ToString () + "_" + FromNameToFilename (name_path) + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json;";
		// Upload the entire local file to the server.
		ES2Web web = new ES2Web (myURL);


		yield return StartCoroutine (web.UploadFile (filePath));

		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}
	}

	string FromNameToFilename (string name)
	{
		return name.Replace (" ", "-");
		
	}


}
