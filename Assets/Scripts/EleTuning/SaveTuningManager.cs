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

	[Header ("Left result sliders and text")]
	public Slider left_extension_slider;
	public Text left_extension_text;
	public Slider left_flexion_slider;
	public Text left_flexion_text;
	public Slider left_ulnar_slider;
	public Text left_ulnar_text;
	public Slider left_radial_slider;
	public Text left_radial_text;


	[Header ("Right result sliders and text")]
	public Slider right_extension_slider;
	public Text right_extension_text;
	public Slider right_flexion_slider;
	public Text right_flexion_text;
	public Slider right_ulnar_slider;
	public Text right_ulnar_text;
	public Slider right_radial_slider;
	public Text right_radial_text;


	[Header ("Save&Go to main menu button")]
	public GameObject m_menu_button;


	// Use this for initialization
	void Awake ()
	{
		left_extension_text.text = left_extension_slider.value.ToString ("N1");
		left_flexion_text.text = left_flexion_slider.value.ToString ("N1");
		left_radial_text.text = left_radial_slider.value.ToString ("N1");
		left_ulnar_text.text = left_ulnar_slider.value.ToString ("n1");

		right_extension_text.text = right_extension_slider.value.ToString ("N1");
		right_flexion_text.text = right_flexion_slider.value.ToString ("N1");
		right_radial_text.text = right_radial_slider.value.ToString ("N1");
		right_ulnar_text.text = right_ulnar_slider.value.ToString ("N1");
		

		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}




	public void LeftExtensionSlider ()
	{
		left_extension_text.text = left_extension_slider.value.ToString ("N1");
	}

	public void LeftFlexionSlider ()
	{
		left_flexion_text.text = left_flexion_slider.value.ToString ("N1");
	}

	public void LeftUlnarSlider ()
	{
		left_ulnar_text.text = left_ulnar_slider.value.ToString ("N1");
	}

	public void LeftRadialSlider ()
	{
		left_radial_text.text = left_radial_slider.value.ToString ("N1");
	}

	public void RightExtensionSlider ()
	{
		right_extension_text.text = right_extension_slider.value.ToString ("N1");
	}

	public void RightFlexionSlider ()
	{
		right_flexion_text.text = right_flexion_slider.value.ToString ("N1");
	}

	public void RightUlnarSlider ()
	{
		right_ulnar_text.text = right_ulnar_slider.value.ToString ("N1");
	}

	public void RightRadialSlider ()
	{
		right_radial_text.text = right_radial_slider.value.ToString ("N1");
	}

	public void SaveDataAndLoadMain ()
	{
		StartCoroutine (LoadMain ());
	}


	IEnumerator LoadMain ()
	{
		m_menu_button.GetComponent<Button> ().interactable = false;
		yield return StartCoroutine (SaveTuningData ());
		SceneManager.LoadSceneAsync ("Main_Menu_Patient");

	}

	IEnumerator SaveTuningData ()
	{
		
		GlobalPlayerData.globalPlayerData.player_data.pitch_left_max = left_extension_slider.value * Mathf.Deg2Rad;
		GlobalPlayerData.globalPlayerData.player_data.pitch_left_min = left_flexion_slider.value * Mathf.Deg2Rad;
		GlobalPlayerData.globalPlayerData.player_data.yaw_left_max = left_radial_slider.value * Mathf.Deg2Rad;
		GlobalPlayerData.globalPlayerData.player_data.yaw_left_min = left_ulnar_slider.value * Mathf.Deg2Rad;

		GlobalPlayerData.globalPlayerData.player_data.pitch_right_max = right_extension_slider.value * Mathf.Deg2Rad;
		GlobalPlayerData.globalPlayerData.player_data.pitch_right_min = right_flexion_slider.value * Mathf.Deg2Rad;
		GlobalPlayerData.globalPlayerData.player_data.yaw_right_max = right_ulnar_slider.value * Mathf.Deg2Rad;
		GlobalPlayerData.globalPlayerData.player_data.yaw_right_min = right_radial_slider.value * Mathf.Deg2Rad;

		GlobalPlayerData.globalPlayerData.player_data.ComputeGesturesDeltas ();


		TuningSession s = new TuningSession ();

		DateTime gameDate = DateTime.UtcNow;
		s.patientName = GlobalPlayerData.globalPlayerData.player;
		s.timestamp = gameDate.ToFileTimeUtc ();

		s.pitch_left_max = left_extension_slider.value * Mathf.Deg2Rad;
		s.pitch_left_min = left_flexion_slider.value * Mathf.Deg2Rad;
		s.pitch_right_max = right_extension_slider.value * Mathf.Deg2Rad;
		s.pitch_right_min = right_flexion_slider.value * Mathf.Deg2Rad;

		s.yaw_left_max = left_radial_slider.value * Mathf.Deg2Rad;
		s.yaw_left_min = left_ulnar_slider.value * Mathf.Deg2Rad;
		s.yaw_right_max = right_ulnar_slider.value * Mathf.Deg2Rad;
		s.yaw_right_min = right_radial_slider.value * Mathf.Deg2Rad;

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
