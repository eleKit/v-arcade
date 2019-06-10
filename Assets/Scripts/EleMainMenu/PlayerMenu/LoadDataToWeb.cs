using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class LoadDataToWeb : MonoBehaviour
{
	[Header ("Use for debug, if checked the match is saved into test server")]
	public bool debugging_save;

	string directoryPath;

	string[] data_paths;


	public GameObject m_there_are_saved_match;

	public Button m_save_now;

	public Text m_im_saving_text;

	public Text m_there_are_match_text;


	// Use this for initialization
	void Start ()
	{
		directoryPath = Path.Combine (Application.persistentDataPath, "TMP_web_saving");
		if (Directory.GetFiles (directoryPath).Length == 0) {
			m_there_are_saved_match.SetActive (false);
		} else {
			m_there_are_saved_match.SetActive (true);
			m_im_saving_text.text = "";
			m_there_are_match_text.text = "Ci sono " + Directory.GetFiles (directoryPath).Length + " partite non salvate!";
			m_save_now.interactable = true;
		}
	}

	// Update is called once per frame
	void Update ()
	{

	}

	//called from the button save in the Welcome scene
	public void SaveOnWeb ()
	{
		StartCoroutine (Save ());
		Debug.Log ("Salvo");
	}

	IEnumerator Save ()
	{
		m_save_now.interactable = false;
		m_im_saving_text.text = "Sto salvando...";
		yield return this.LoadData ();
		m_im_saving_text.text = "Finito!";
		m_there_are_match_text.text = "";
		m_save_now.interactable = true;
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
				Debug.Log ("saving " + data_paths [i]);
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
