using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;
using System.IO;

public class ShootingManager : Singleton<ShootingManager>
{
	FileNamesOfPaths loaded_path = new FileNamesOfPaths ();

	Vector3 initial_player_pos = new Vector3 (0, 0.75f, 0);
	// Use this for initialization
	void Start ()
	{

		GameManager.Instance.player_initial_pos = initial_player_pos;
		GameManager.Instance.BaseStart ("DuckGameMusic", GameMatch.GameType.Shooting);

		/* set the bool of the current game in the game manager 
		 * and in the GUI manager
		 */




		GameManager.Instance.player.transform.position = 
			GameManager.Instance.player_initial_pos;

		GameManager.Instance.player.SetActive (false);

		
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameManager.Instance.BaseUpdate ();

		if (GameObject.FindGameObjectsWithTag ("Duck").Length == 0 &&
		    GameManager.Instance.Get_Is_Playing ()) {
			WinLevel ();
		}
	}

	public void ToMenu ()
	{
		ResetPath ();
		GameManager.Instance.BaseToMenu ();
	}

	void ResetPath ()
	{
		foreach (GameObject duck in GameObject.FindGameObjectsWithTag ("Duck")) {
			Destroy (duck);
		}
	}

	public void WinLevel ()
	{
		//this function starts win jingle and then calls the win function
		SfxManager.Instance.Play ("win_jigle");
		GameManager.Instance.BaseWinLevel ();
	}

	public void ChooseLevel (FileNamesOfPaths path)
	{
		loaded_path = path;
		GameManager.Instance.BaseChooseLevel (path.name);

		GameManager.Instance.player.transform.position = 
			GameManager.Instance.player_initial_pos;

		DuckPathGenerator.Instance.LoadPath (path.file_path);
		
	}

	//function called after pause the game
	public void ResumeLevel ()
	{
		GameManager.Instance.BaseResumeLevel ();
	}

	public void RestartLevel ()
	{
		GameManager.Instance.m_wait_background.SetActive (true);
		ResetPath ();
		ChooseLevel (loaded_path);
	}

	public int GetScore ()
	{
		return GameManager.Instance.BaseGetScore ();
	}

	public void AddPoints ()
	{
		GameManager.Instance.BaseAddPoints ();

	}

	public bool GetIsPlaying ()
	{
		return GameManager.Instance.Get_Is_Playing ();
	}
}
