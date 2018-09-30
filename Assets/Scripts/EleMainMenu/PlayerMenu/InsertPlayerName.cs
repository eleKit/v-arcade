using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class InsertPlayerName : MonoBehaviour
{


	public  string name_player;
	public GameObject button;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void SetPathName (string name)
	{
		string tmp = FromNameToFilename (name);

		name_player = RemoveUnderscore (tmp);

	}


	public void Save ()
	{

		if (!name_player.Equals ("")) {
			

			GlobalPlayerData.globalPlayerData.InitPlayer (name_player);
			/* NB InitPlayer sets only th player name, 
			 * all the player values are taken from the Tuning that is always the sceneafter this one
			 */
			WelcomeMenuUI.Instance.AccessDone ();

			button.GetComponent<Button> ().interactable = false;
			SceneManager.LoadSceneAsync ("Tuning_scene");

		}
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
