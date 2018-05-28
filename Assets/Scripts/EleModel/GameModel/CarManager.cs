using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using POLIMIGameCollective;
using System;
using System.IO;
using Leap;

public class CarManager : Singleton<CarManager>
{
	//attribute used to set the winning point coordinates
	public GameObject winning_Point;



	// Use this for initialization
	void Start ()
	{
		GameManager.Instance.player_initial_pos = new Vector3 (0f, -5.7f, 0f);
		GameManager.Instance.BaseStart ("CarGameMusic");

		/* set the bool of the current game in the game manager 
		 * and in the UI manager
		 */
		GameMenuScript.Instance.car = true;
		GameManager.Instance.car = true;



		ResetPlayer ();
		GameManager.Instance.player.SetActive (false);
		
	}

	// Update is called once per frame
	void Update ()
	{
		GameManager.Instance.BaseUpdate ();
		
	}

	public void ResetPlayer ()
	{
		GameManager.Instance.player.GetComponent<CarControllerScript> ().SetGravityToZero ();
		GameManager.Instance.player.transform.position = GameManager.Instance.player_initial_pos;

	}

	public void ChooseLevel (string name)
	{
		GameManager.Instance.BaseChooseLevel (name);
		ResetPlayer ();
		GameObject.Find ("CarPathGenerator").GetComponent<CarPathGenerator> ().LoadPath (name);

	}

	//This method is used to set the point where the game ends
	public void SetWinningPosition (Vector3 coord)
	{
		winning_Point.transform.position = coord;
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

	public void WinLevel ()
	{
		GameManager.Instance.BaseWinLevel ();
		GameMenuScript.Instance.LoadWinScreen (GetScore ());
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
