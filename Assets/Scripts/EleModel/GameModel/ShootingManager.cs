using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class ShootingManager : Singleton<ShootingManager>
{

	// Use this for initialization
	void Start ()
	{
		GameManager.Instance.player_initial_pos = Vector3.zero;
		GameManager.Instance.BaseStart ();
		GameManager.Instance.menu_GUI.shooting = true;

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

	public void WinLevel ()
	{
		GameManager.Instance.BaseWinLevel ();
	}

	public void ChooseLevel (string name)
	{

		GameManager.Instance.BaseChooseLevel (name);

		GameManager.Instance.player.transform.position = 
			GameManager.Instance.player_initial_pos;
		
	}

	//function called after pause the game
	public void ResumeLevel ()
	{
		GameManager.Instance.BaseResumeLevel ();
	}

	public void RestartLevel ()
	{
		GameManager.Instance.m_wait_background.SetActive (true);
		ChooseLevel (GameManager.Instance.current_path);
	}

	public int GetScore ()
	{
		return GameManager.Instance.BaseGetScore ();
	}

	public void AddPoints ()
	{
		GameManager.Instance.BaseAddPoints ();

	}
}
