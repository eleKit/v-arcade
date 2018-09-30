using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;
using System;
using System.IO;


public class FisioSpacePathGenerator :Singleton<FisioSpacePathGenerator>
{
	[Header ("Check this bool to debug without saving online")]
	public bool no_save;
	[Header ("Use for debug, if checked the match is saved into test server")]
	public bool debugging_save;

	//num of possible curve positions for each section: START, MIDDLE, FINAL
	const int CURVES = 4;


	public Button[] m_start_buttons = new Button[CURVES - 1];

	public Button[] m_middle_buttons = new Button[CURVES];

	public Button[] m_final_buttons = new Button[CURVES];

	public Slider[] enemies_slider = new Slider[SECTIONS];
	public Text[] enemies_text = new Text[SECTIONS];


	public Slider amplitude_slider;
	public Text amplitude_text;

	SpacePath space_path;
	string name_path = "";


	//Attributes used to activate|deactivate go_on button
	public Button button_go_on;

	//The Path is correct if one button of each section is pressed
	bool start_ok;
	bool middle_ok;
	bool final_ok;

	//the save button must be not interactable when no correct path has been chosen
	bool activated;


	//num of scetions
	const int INITIAL = 0;
	const int MIDDLE = 1;
	const int FINAL = 2;
	const int SECTIONS = 3;

	/* direction of each curve, it can be:
	 * Left cuve value -1
	 * Right curve value 1
	 * Centre curve value 0
	 * Stop value -2
	 */
	int[] curve_values = new int[SECTIONS];

	// Use this for initialization
	void Start ()
	{
		space_path = new SpacePath ();

		Reset ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		//activates the save button since the path chosen is accepted as correct
		if (start_ok && middle_ok && final_ok && !activated) {
			activated = true;
			button_go_on.interactable = true;
		}
		
	}

	public void Reset ()
	{ 
		//reset check path bool
		start_ok = false;
		middle_ok = false;
		final_ok = false;

		//reset save button as not interactable
		activated = false;
		button_go_on.interactable = false;

		//reset amplitude slider
		amplitude_slider.value = amplitude_slider.minValue;
		amplitude_text.text = "Ampiezza curva: " + amplitude_slider.value.ToString ();

		//reset curve buttons of each section
		for (int i = 0; i < m_start_buttons.Length; i++) {
			m_start_buttons [i].interactable = true;
		}

		for (int i = 0; i < m_middle_buttons.Length; i++) {
			m_middle_buttons [i].interactable = true;
		}

		for (int i = 0; i < m_final_buttons.Length; i++) {
			m_final_buttons [i].interactable = true;
		}


		//reset enemies slider
		for (int i = 0; i < enemies_slider.Length; i++) {
			enemies_slider [i].value = enemies_slider [i].minValue;
			enemies_slider [i].interactable = true;
			enemies_text [i].text = enemies_slider [i].minValue.ToString ();
		}

		for (int i = 0; i < curve_values.Length; i++) {
			curve_values [i] = 0;
		}


	}


	/*it is passed i as the idex of button pressed inside the m_start_buttons array  :
	 * VALID FOR StartPressed (int i)
	 * 0 Left curve button index 
	 * 1 Centre curve button index 
	 * 2 Right curve button index
	 */
	public void StartPressed (int i)
	{
		start_ok = true;


		for (int h = 0; h < m_start_buttons.Length; h++) {
			if (h != i)
				m_start_buttons [h].interactable = true;
		}

		m_start_buttons [i].interactable = false;

	}


	/*it is passed i as the idex of button pressed inside the m_start_buttons array:
	 * VALID FOR MiddlePressed (int i) and FinalPressed (int i)
	 * 0 Stop button index 
	 * 1 Left curve button index 
	 * 2 Centre curve button index
	 * 3 Right
	 */
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

	//see MiddlePressed(int i)
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
					for (int k = 0; k < enemies_slider.Length; k++) {
						if (!enemies_slider [k].interactable)
							enemies_slider [k].interactable = true;
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

		for (int h = 0; h < enemies_slider.Length; h++) {
			if (!enemies_slider [h].interactable)
				enemies_slider [h].interactable = true;
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



		for (int h = 0; h < enemies_slider.Length; h++) {
			if (h >= diamond_index)
				enemies_slider [h].interactable = false;
		}

	}

	public void ManageSlider (int index)
	{
		enemies_text [index].text = 
			Mathf.RoundToInt (enemies_slider [index].value)
				.ToString ();
	}

	public void ManageAmplitudeSlider (float value)
	{
		amplitude_text.text = "Ampiezza curva: " + Mathf.RoundToInt (value).ToString ();
	}

	/* From here there are the path curve index scripts
	 * 
	 * these scripts are used on each Curve Button in couple with the
	 * StartPressed(int i) or MiddlePressed(int i) or FinalPressed(int i)
	 */

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



	public void SetPathName (string name)
	{
		name_path = name;
		Debug.Log (name_path);

	}

	public void SaveSpaceData ()
	{
		bool path_already_ended = false;


		for (int i = 0; i < curve_values.Length; i++) {
			if (curve_values [i] == -2 && !path_already_ended) {
				space_path.space_sections = new SpaceSection[i];
				path_already_ended = true;
			}
		}

		if (!path_already_ended) {
			space_path.space_sections = new SpaceSection[SECTIONS];
		}

		for (int i = 0; i < space_path.space_sections.Length; i++) {
			space_path.space_sections [i] = new SpaceSection ();
		}


		for (int i = 0; i < space_path.space_sections.Length; i++) {
			space_path.space_sections [i].curve_position = curve_values [i];
			space_path.space_sections [i].num_enemies = Mathf.RoundToInt (enemies_slider [i].value);

		}

		space_path.curve_amplitude = amplitude_slider.value;


	}


	public void SavePath ()
	{
		bool found_equal_name = false;

		string directoryPath = Path.Combine (Application.persistentDataPath,
			                       Path.Combine ("Paths", GameMatch.GameType.Space.ToString ()));
		
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
			space_path.timestamp = gameDate.ToFileTimeUtc ();
			space_path.doctorName = GlobalDoctorData.globalDoctorData.doctor;

			space_path.id_path = name_path;

			
			Directory.CreateDirectory (directoryPath);
			string filePath = Path.Combine (
				                  directoryPath,
				                  GameMatch.GameType.Space.ToString () + "_"
				                  + FromNameToFilename (name_path) + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json"
			                  );

			string jsonString = JsonUtility.ToJson (space_path);
			File.WriteAllText (filePath, jsonString);

			if (!no_save) {
				StartCoroutine (SavePathDataCoroutine (filePath, gameDate, jsonString));
			} else {
				Debug.Log ("not saving!");
			}

			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().name);
		}
	}

	IEnumerator SavePathDataCoroutine (string filePath, DateTime gameDate, string pathString)
	{
		string address;
		string webfilename = "path_" + GameMatch.GameType.Space.ToString () + "_" + FromNameToFilename (name_path) + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json";

		if (debugging_save) {
			address = "http://127.0.0.1/ES2.php?webfilename=";
			Debug.Log ("Debugging save");
		} else {
			address = "http://data.polimigamecollective.org/demarchi/ES2.php?webfilename=";
		}
		string myURL = address + webfilename;
		// Upload the entire local file to the server.
		ES2Web web = new ES2Web (myURL);


		yield return StartCoroutine (web.UploadFile (filePath));

		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
			string directoryPath = Path.Combine (Application.persistentDataPath, "TMP_web_saving");
			Directory.CreateDirectory (directoryPath);
			string path = Path.Combine (directoryPath, webfilename);
			File.WriteAllText (path, pathString);
		}
	}


	//Scripts used to conver file name string

	string FromNameToFilename (string name)
	{
		return name.Replace (" ", "-");

	}

	string FromFilenameToName (string name)
	{
		return name.Replace ("-", " ");

	}




}
