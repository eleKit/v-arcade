using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;


public class InsertPlayerName : MonoBehaviour
{


	public  string name_player;
	public GameObject button;
	string filePath;
	string directoryPath;

	PatientsList patients_list;

	// Use this for initialization
	void Start ()
	{

		directoryPath = Path.Combine (Application.persistentDataPath, "Patient_List");
		filePath = Path.Combine (directoryPath, "patients_list.json");
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	//this is called by the Save() function
	void LoadNamesList ()
	{
		if (!Directory.Exists (directoryPath)) {
			// do nothing
		} else {
			Debug.Log (filePath.ToString ());

			string pl = File.ReadAllText (filePath);

			patients_list = JsonUtility.FromJson<PatientsList> (pl);
		}

	}


	public void SetPathName (string name)
	{
		string tmp = FromNameToFilename (name);

		name_player = RemoveUnderscore (tmp);

	}


	public void Save ()
	{
		if (!name_player.Equals ("")) {

			LoadNamesList ();
			bool found_correspondent_name = false;

			foreach (string name in patients_list.patients) {
				if (name.Equals (name_player)) {
					found_correspondent_name = true;
				}
			}


			if (found_correspondent_name) {
			
				/* NB InitPlayer sets only th player name, 
				 * all the player values are taken from the Tuning that is always the sceneafter this one
				 */
				GlobalPlayerData.globalPlayerData.InitPlayer (name_player);

				WelcomeMenuUI.Instance.AccessDone ();

				button.GetComponent<Button> ().interactable = false;
				SceneManager.LoadSceneAsync ("Main_Menu_Patient");
			}

		}
	}

	void CheckExistingName ()
	{
		
	}


	string FromNameToFilename (string name)
	{
		return name.Replace (" ", "-");

	}


	string RemoveUnderscore (string name)
	{
		return name.Replace ("_", "-");

	}
}
