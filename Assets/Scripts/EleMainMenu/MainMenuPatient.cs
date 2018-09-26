using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

public class MainMenuPatient : MonoBehaviour
{


	string[] scenes_names = 
		new string[] {
			"Car_game",
			"Music_game",
			"Shooting_game",
			"Space_game",
			"Tuning_scene"
		};


	void Start ()
	{
		MusicManager.Instance.PlayMusic ("MainMenuMusic");
	}

	void Update ()
	{
		
	}

	public void LoadNextScene (int i)
	{
		//everytime a new scene is loaded the MusicManager must stop 
		MusicManager.Instance.StopAll ();

		SceneManager.LoadSceneAsync (scenes_names [i]);
	}



	public void LoadMainScene ()
	{
		//everytime a new scene is loaded the MusicManager must stop 
		MusicManager.Instance.StopAll ();

		SceneManager.LoadSceneAsync ("Insert_player_name");
	}

}
