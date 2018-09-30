using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class LoadPathsFromWeb : MonoBehaviour
{
	public Text m_welcome_text;
	//the list where all the web addresses of file paths are saved
	List<string> file_n = new List<string> ();
	string directoryPath;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (LoadFilenames ());

		directoryPath = Path.Combine (Application.persistentDataPath, "Paths");
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	string FromFilenameToName (string name)
	{
		return name.Replace ("-", " ");

	}

	string FromNameToFilename (string name)
	{
		return name.Replace (" ", "-");

	}


	public IEnumerator LoadFilenames ()
	{
		m_welcome_text.text = "Scaricamento dati...";
		string myURL = "http://127.0.0.1/ES2.php?webfilename=";
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
			if (filename.StartsWith ("path_")) {
				file_n.Add (filename);
				Debug.Log (filename);
			}
		}

		yield return StartCoroutine (DownloadAllPaths ());

		Debug.Log (" end downloading paths");
		yield return new WaitForSeconds (0.5f);
		m_welcome_text.text = "Benvenuti!";

	}



	public IEnumerator  DownloadAllPaths ()
	{

		/* on web paths are saved as path_GameType_pathName_TS.json
		 */



		Debug.Log (directoryPath);
		Debug.Log (file_n.Count);

		foreach (string webfile in file_n) {

			string new_directory_path = Path.Combine (directoryPath, webfile.Split ('_') [1]); //get GameType from file name
			Debug.Log (new_directory_path);

			if (!Directory.Exists (new_directory_path)) {
				Directory.CreateDirectory (new_directory_path);
			}

			string tmp = string.Join ("_", webfile.Split ('_').Skip (1).ToArray ());

			string filepath = Path.Combine (new_directory_path, tmp);

			yield return StartCoroutine (DownloadEntireFile (webfile, filepath));

		}
			
		
	}



	public IEnumerator DownloadEntireFile (string webpath, string filepath)
	{
		// As we don't specify a tag, it will download everything
		// within the file 'myFile.txt'.
		Debug.Log ("downloading path " + webpath);
		string myURL = "http://127.0.0.1/ES2.php?webfilename=" + webpath;
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
