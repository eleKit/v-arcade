using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerData : MonoBehaviour
{
	//The player name
	public string player;

	public const string player_prefs_name_child = "Name";

	public GlobalPlayer player_data = new GlobalPlayer ();

	public static GlobalPlayerData globalPlayerData;

	void Awake ()
	{
		if (globalPlayerData == null) {
			DontDestroyOnLoad (gameObject);
			globalPlayerData = this;
		} else if (globalPlayerData != this) {
			Destroy (gameObject);
		}
	}



	//use this for the first input of the player name
	public void InitPlayer (string pl)
	{
		player = pl;
		player_data.name = pl;
		PlayerPrefs.SetString (GlobalPlayerData.player_prefs_name_child, pl);

	}


	//use this to load an existing player name
	public void LoadPlayer ()
	{
		string pl = PlayerPrefs.GetString (GlobalPlayerData.player_prefs_name_child);
		player = pl;
		player_data.name = pl;
		
	}
}
