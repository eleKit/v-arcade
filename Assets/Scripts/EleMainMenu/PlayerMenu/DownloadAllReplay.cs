using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DownloadAllReplay : MonoBehaviour
{


	[Header ("Use for debug, if checked the match is saved into test server")]
	public bool debugging_save;


	List<GameMatch.GameType> game_types = new List<GameMatch.GameType> { GameMatch.GameType.Car, GameMatch.GameType.Music, 
		GameMatch.GameType.Shooting, GameMatch.GameType.Space
	};
	
	List<string> replay_of_all_patients_list = new List<string> ();


	//directory of the Patients folder that contains the hand data divided in folders that are entitled with the PatientNames
	string directoryPatientsFolder;
	//the list where all the web addresses of file replay are saved
	List<string> replay_of_patient_list = new List<string> ();


	//directory of the list with the nicknames of all the patients
	string directoryPatientsList;
	string filePathPatientsList;
	PatientsList patients_list;




	// Use this for initialization
	void Start ()
	{
		patients_list = new PatientsList ();
		//find patients' list on persistent data path
		directoryPatientsList = Path.Combine (Application.persistentDataPath, "Patient_List");
		filePathPatientsList = Path.Combine (directoryPatientsList, "patients_list.json");

		if (!Directory.Exists (directoryPatientsList)) {
			// do nothing, the list in patients_list is empty
		} else {
			Debug.Log (filePathPatientsList.ToString ());

			string pl = File.ReadAllText (filePathPatientsList);

			patients_list = JsonUtility.FromJson<PatientsList> (pl);
		}


		//in this directory the hand data are downloaded, so if it does not exist now it is created
		directoryPatientsFolder = Path.Combine (Application.persistentDataPath, "Patients");
		if (!Directory.Exists (directoryPatientsFolder)) {
			Directory.CreateDirectory (directoryPatientsFolder);
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	//this is called from the LoadAndDownloadDataController
	public IEnumerator LoadReplayFilenames ()
	{


		string address;
		if (debugging_save) {
			address = "http://127.0.0.1/ES2.php?webfilename=";
			Debug.Log ("Debugging save");
		} else {
			address = "http://data.polimigamecollective.org/demarchi/ES2.php?webfilename=";
		}

		string myURL = address;
		ES2Web web = new ES2Web (myURL);

		// Start downloading our filenames and wait for it to finish.
		yield return StartCoroutine (web.DownloadFilenames ());

		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}

		// Now get our filenames as an array ...
		string[] filenames = web.GetFilenames ();


		foreach (string filename in filenames) {
			
			if (filename.StartsWith ("patient_")) {
				replay_of_all_patients_list.Add (filename);
				Debug.Log (filename);

			}
		}

		yield return StartCoroutine (DownloadAllReplays ());


	}


	IEnumerator  DownloadAllReplays ()
	{

		/* on web Replay are saved as patient_nickname_GameType_TS.json and patient_nickname_GameType_TS_hand_data.json
		 */



		foreach (string webfile in replay_of_all_patients_list) {
			
			foreach (string nickname in patients_list.patients) {
				
				if (webfile.StartsWith ("patient_" + nickname + "_")) {

					string new_directory_path = Path.Combine (directoryPatientsFolder, nickname);

					if (!Directory.Exists (new_directory_path)) {
						Directory.CreateDirectory (new_directory_path);
					}
						
					foreach (GameMatch.GameType game_type in game_types)
						if (webfile.StartsWith ("patient_" + nickname + "_" + game_type.ToString () + "_")) {
							
							string complete_directory_path = Path.Combine (new_directory_path, game_type.ToString ());
							if (!Directory.Exists (complete_directory_path)) {
								Directory.CreateDirectory (complete_directory_path);
							}

							string tmp = string.Join ("_", webfile.Split ('_').Skip (1).ToArray ());
							string filepath = Path.Combine (complete_directory_path, tmp);
							if (!File.Exists (filepath)) {
								yield return StartCoroutine (DownloadEntireFile (webfile, filepath));
							} else {
								//if the file already exists do nothing
								Debug.Log ("Already exists");
							}
						}
				}


			}
		}

		yield return new WaitForSeconds (0f);

	
	}


	IEnumerator DownloadEntireFile (string webpath, string filepath)
	{
		// As we don't specify a tag, it will download everything
		// within the file 'myFile.txt'.

		string address;
		if (debugging_save) {
			address = "http://127.0.0.1/ES2.php?webfilename=";
			Debug.Log ("Debugging save");
		} else {
			address = "http://data.polimigamecollective.org/demarchi/ES2.php?webfilename=";
		}

		string myURL = address + webpath;
		ES2Web web = new ES2Web (myURL);

		// Start downloading our data and wait for it to finish.
		yield return StartCoroutine (web.Download ());

		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}

		// Now save our data to file so we can use ES2.Load to load it.
		web.SaveToFile (filepath);
	}
}
