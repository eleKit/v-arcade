using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class SaveTuningManager : MonoBehaviour
{
	
	[Header ("Use for debug, if checked the match is saved into test server")]
	public bool debugging_save;

	[Header ("Use for debug, if checked the match is not saved")]
	public bool no_save;

	[Header ("Use to save the values in the PersistentDataPath")]
	public TuningManager tuning_manager;


	[Header ("Left result sliders and text")]
	public Slider left_flexion_extension_slider;
	public Text left_flexion_extension_text;
	public Slider left_ulnar_radial_slider;
	public Text left_ulnar_radial_text;



	[Header ("Right result sliders and text")]
	public Slider right_flexion_extension_slider;
	public Text right_flexion_extension_text;
	public Slider right_ulnar_radial_slider;
	public Text right_ulnar_radial_text;


	[Header ("Save&Go to main menu button")]
	public GameObject m_menu_button;
	[Header ("Repeat tuning button")]
	public GameObject m_repeat_tuning_button;
	[Header ("Skip tuning button")]
	public GameObject m_skip_tuning_button;


	// Use this for initialization
	void Awake ()
	{
		left_flexion_extension_text.text = left_flexion_extension_slider.value.ToString ("N2");
		left_ulnar_radial_text.text = left_ulnar_radial_slider.value.ToString ("N2");

		right_flexion_extension_text.text = right_flexion_extension_slider.value.ToString ("N2");
		right_ulnar_radial_text.text = right_ulnar_radial_slider.value.ToString ("N2");
		

		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}




	public void LeftFlexionExtensionSlider ()
	{
		left_flexion_extension_text.text = left_flexion_extension_slider.value.ToString ("N2");
	}



	public void LeftUlnarRadialSlider ()
	{
		left_ulnar_radial_text.text = left_ulnar_radial_slider.value.ToString ("N2");
	}

	public void RightFlexionExtensionSlider ()
	{
		right_flexion_extension_text.text = right_flexion_extension_slider.value.ToString ("N2");
	}



	public void RightUlnarRadialSlider ()
	{
		right_ulnar_radial_text.text = right_ulnar_radial_slider.value.ToString ("N2");
	}


	public void SaveDataAndLoadMain ()
	{
		m_menu_button.GetComponent<Button> ().interactable = false;
		m_repeat_tuning_button.GetComponent<Button> ().interactable = false;
		m_skip_tuning_button.GetComponent<Button> ().interactable = false;

		StartCoroutine (LoadMain ());
	}


	IEnumerator LoadMain ()
	{
		//wait for saving data before loading the next scene
		yield return StartCoroutine (SaveTuningData ());
		SceneManager.LoadSceneAsync ("Main_Menu_Patient");

	}

	IEnumerator SaveTuningData ()
	{
		//save data in the global player object
		GlobalPlayerData.globalPlayerData.player_data.left_pitch_scale = left_flexion_extension_slider.value * Mathf.Deg2Rad;
		GlobalPlayerData.globalPlayerData.player_data.left_yaw_scale = left_ulnar_radial_slider.value * Mathf.Deg2Rad;

		GlobalPlayerData.globalPlayerData.player_data.right_pitch_scale = right_flexion_extension_slider.value * Mathf.Deg2Rad;
		GlobalPlayerData.globalPlayerData.player_data.right_yaw_scale = right_ulnar_radial_slider.value * Mathf.Deg2Rad;

		//GlobalPlayerData.globalPlayerData.player_data.ComputeGesturesDeltas ();

		/* Save Data on the PErsistentDataPath and online 
		 * 
		 * save data on file (and web) as TuningSession class type 
		 */
		TuningSession s = new TuningSession ();

		DateTime gameDate = DateTime.UtcNow;
		s.patientName = GlobalPlayerData.globalPlayerData.player;
		s.timestamp = gameDate.ToFileTimeUtc ();

		s.pitch_left_max = tuning_manager.data_left_extension;
		s.pitch_left_min = tuning_manager.data_left_flexion;
		s.pitch_right_max = tuning_manager.data_right_extension;
		s.pitch_right_min = tuning_manager.data_right_flexion;

		s.yaw_left_max = tuning_manager.data_left_radial;
		s.yaw_left_min = tuning_manager.data_left_ulnar;
		s.yaw_right_max = tuning_manager.data_right_ulnar;
		s.yaw_right_min = tuning_manager.data_right_radial;

		string directoryPath = Path.Combine (Application.persistentDataPath,
			                       Path.Combine ("Tunings", s.patientName));

		Directory.CreateDirectory (directoryPath);

		string filePath = Path.Combine (
			                  directoryPath,
			                  s.patientName + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json");

		string jsonString = JsonUtility.ToJson (s);
		File.WriteAllText (filePath, jsonString);


		//save match data on web
		if (no_save) {
			Debug.Log ("not saving!! no_save is active");
		} else {
			yield return StartCoroutine (SaveTuningDataCoroutine (filePath, s, gameDate, jsonString));
		}

		yield return new WaitForSeconds (0f);

	}


	IEnumerator SaveTuningDataCoroutine (string filePath, TuningSession s, DateTime gameDate, string tuningString)
	{
		string address; 
		string webfilename = "tuning_" + s.patientName + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json";

		if (debugging_save) {
			address = "http://127.0.0.1/ES2.php?webfilename=";
			Debug.Log ("Debugging save");
		} else {
			address = "http://data.polimigamecollective.org/demarchi/ES2.php?webfilename=";
		}

		string myURL = address + webfilename;

		// Upload the entire local file to the server.
		ES2Web web = new ES2Web (myURL);

		Debug.Log ("file saved " + myURL);

		yield return StartCoroutine (web.UploadFile (filePath));

		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
			string directoryPath = Path.Combine (Application.persistentDataPath, "TMP_web_saving");
			Directory.CreateDirectory (directoryPath);
			string path = Path.Combine (directoryPath, webfilename);
			File.WriteAllText (path, tuningString);
		}
	}
}
