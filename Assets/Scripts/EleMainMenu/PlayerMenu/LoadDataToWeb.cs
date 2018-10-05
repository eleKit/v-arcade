using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class LoadDataToWeb : MonoBehaviour
{
	[Header ("Use for debug, if checked the match is saved into test server")]
	public bool debugging_save;

	string directoryPath;

	string[] data_paths;

	// Use this for initialization
	void Start ()
	{
		directoryPath = Path.Combine (Application.persistentDataPath, "TMP_web_saving");
	}

	// Update is called once per frame
	void Update ()
	{

	}


	//this is called from the WebConnectionController
	public IEnumerator LoadData ()
	{
		if (Directory.Exists (directoryPath)) {
			data_paths = Directory.GetFiles (directoryPath, "*.json");
		} else {
			data_paths = new string[] { "" };
		}

		for (int i = 0; i < data_paths.Length; i++) {
			if (!data_paths [i].Equals ("")) {
				yield return StartCoroutine (SaveDataCoroutine (data_paths [i]));
			}
		}

		yield return new WaitForSeconds (0f);


	}


	IEnumerator SaveDataCoroutine (string data_path)
	{
		string address; 
		string webfilename = Path.GetFileName (data_path);

		if (debugging_save) {
			address = "http://127.0.0.1/ES2.php?webfilename=";
			Debug.Log ("Debugging save");
		} else {
			address = "http://data.polimigamecollective.org/demarchi/ES2.php?webfilename=";
		}
		string myURL = address + webfilename;
		// Upload the entire local file to the server.
		ES2Web web = new ES2Web (myURL);


		yield return StartCoroutine (web.UploadFile (data_path));

		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);

		} else {
			File.Delete (data_path);
		}
	}
}
