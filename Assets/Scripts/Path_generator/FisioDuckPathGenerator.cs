using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;
using System;
using System.IO;

public class FisioDuckPathGenerator : Singleton<FisioDuckPathGenerator>
{
	[Header ("Check this bool to debug without saving online")]
	public bool no_save;
	[Header ("Use for debug, if checked the match is saved into test server")]
	public bool debugging_save;

	DuckPath duck_path;

	public Toggle[] front_buttons = new Toggle[DuckSection.N];
	public Toggle[] middle_buttons = new Toggle[DuckSection.N];
	public Toggle[] back_buttons = new Toggle[DuckSection.N];


	string name_path = "";



	// Use this for initialization
	void Start ()
	{
		duck_path = new DuckPath ();

		duck_path.back = new DuckSection ();
		duck_path.back.section = DuckSection.DuckGameSection.Back;

		duck_path.middle = new DuckSection ();
		duck_path.middle.section = DuckSection.DuckGameSection.Middle;

		duck_path.front = new DuckSection ();
		duck_path.front.section = DuckSection.DuckGameSection.Front;



		Reset ();
			
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Reset ()
	{
		
		for (int i = 0; i < DuckSection.N; i++) {

			duck_path.back.ducks [i] = false;
			front_buttons [i].isOn = false;
	

			duck_path.middle.ducks [i] = false;
			middle_buttons [i].isOn = false;
	

			duck_path.front.ducks [i] = false;
			back_buttons [i].isOn = false;
		}


	}


	public void SaveDuckBool ()
	{
		
		for (int i = 0; i < DuckSection.N; i++) {
			
			duck_path.back.ducks [i] = back_buttons [i].isOn;

			duck_path.middle.ducks [i] = middle_buttons [i].isOn;

			duck_path.front.ducks [i] = front_buttons [i].isOn;

		}
		


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

	public void SavePath ()
	{
		bool found_equal_name = false;

		//TODO if the paths are divided into doctors substitute GameMatch.GameType.Shooting.ToString () with duck_path.doctorName
		string directoryPath = Path.Combine (Application.persistentDataPath,
			                       Path.Combine ("Paths", GameMatch.GameType.Shooting.ToString ()));

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
			duck_path.timestamp = gameDate.ToFileTimeUtc ();
			duck_path.doctorName = GlobalDoctorData.globalDoctorData.doctor;

			duck_path.id_path = name_path;


			Directory.CreateDirectory (directoryPath);
			string filePath = Path.Combine (
				                  directoryPath,
				                  GameMatch.GameType.Shooting.ToString () + "_"
				                  + FromNameToFilename (name_path) + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json"
			                  );

			string jsonString = JsonUtility.ToJson (duck_path);
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
		string webfilename = "path_" + GameMatch.GameType.Shooting.ToString () + "_" + FromNameToFilename (name_path) + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json";

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


	string FromNameToFilename (string name)
	{
		return name.Replace (" ", "-");

	}
}

