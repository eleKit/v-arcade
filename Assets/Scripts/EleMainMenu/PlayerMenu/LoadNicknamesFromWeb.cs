using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadNicknamesFromWeb : MonoBehaviour
{

	[Header ("Use for debug, if checked the match is saved into test server")]
	public bool debugging_save;

	string file_n;
	string directoryPath;
	string filePath;

	// Use this for initialization
	void Start ()
	{
		directoryPath = Path.Combine (Application.persistentDataPath, "Patient_List");
		filePath = Path.Combine (directoryPath, "patients_list.json");
		StartCoroutine (LoadFileOfNicknames ());

		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public IEnumerator LoadFileOfNicknames ()
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
			if (filename.Equals ("patients_list.json")) {
				file_n = filename;
				Debug.Log (filename);
			}
		}

		yield return StartCoroutine (DownloadEntireFile ());

	

	}


	public IEnumerator DownloadEntireFile ()
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

		string myURL = address + file_n;
		ES2Web web = new ES2Web (myURL);

		// Start downloading our data and wait for it to finish.
		yield return StartCoroutine (web.Download ());

		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}

		if (!Directory.Exists (directoryPath)) {
			Directory.CreateDirectory (directoryPath);
		}

		// Now save our data to file so we can use ES2.Load to load it.
		web.SaveToFile (filePath);
	}



}
