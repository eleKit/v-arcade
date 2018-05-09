using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using POLIMIGameCollective;
using System;
using System.IO;
using Leap;

public class GameManager : Singleton<GameManager>
{

	public GameObject m_background;

	public GameObject m_wait_background;
	public Text m_wait_text;

	[Header ("Loading time for Level")]
	[Range (0f, 4f)]
	public float m_loading_time = 0.5f;

	public GameObject player;

	public Vector3 player_initial_pos;


	public HandController hc;


	public GameMenuScript menu_GUI;


	//name of the path chosen
	public string current_path = "";


	//gameobject array containing the diamonds whose position indicates the level path
	private GameObject[] m_path;




	//bool to deactivate player if the game is paused
	private bool is_playing = false;


	//score of the game
	private int score;




	//TODO cancellare fa crashare Unity
	private string hand_data_file_path;

	private bool firstTime = true;



	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void BaseStart ()
	{
		ClearScreens ();
		m_background.SetActive (true);
		//reset car position and deactivates car gameObj 

		//the scene begins with the game main menu
		menu_GUI.menu = true;


	}


	public void BaseUpdate ()
	{
		if (Input.GetKeyDown ("escape") && is_playing) {
			BasePauseLevel ();
		}

	}


	void ClearScreens ()
	{
		if (m_background != null)
			m_background.SetActive (false);

		if (m_wait_background != null)
			m_wait_background.SetActive (false);
	}




	public void BaseChooseLevel (string name)
	{
		current_path = name;
		StartCoroutine (BaseLoadLevel (name));
	}

	IEnumerator BaseLoadLevel (string name)
	{
		

		yield return new WaitForSeconds (m_loading_time);

		is_playing = true;

		Debug.Log ("inside LoadLevel");

		//deactivate BG 
		m_background.SetActive (false);

		m_wait_background.SetActive (true);
		m_wait_text.text = "caricamento.";

		yield return new WaitForSeconds (0.5f);


		m_wait_text.text = "caricamento..";

		yield return new WaitForSeconds (0.5f);

		m_wait_text.text = "caricamento...";

		yield return new WaitForSeconds (0.5f);

		//this must be before setting the position
		player.SetActive (true);

		//deactivate wait screen
		m_wait_background.SetActive (false);

		//player.transform.position = player_initial_pos;


	}
		





	//called when the player pauses the game
	void BasePauseLevel ()
	{

		is_playing = false;

		player.SetActive (false);

		menu_GUI.pause = true;

	}

	//triggered by the button "continue" in the pause screen
	public void BaseResumeLevel ()
	{
		is_playing = true;

		player.SetActive (true);

	}



	//called when the player reaches the end of the level
	public void BaseWinLevel ()
	{
		is_playing = false;


		StartCoroutine (WinCoroutine ());
	}


	IEnumerator WinCoroutine ()
	{

		menu_GUI.win = true;

		yield return new WaitForSeconds (0.5f);

		m_background.SetActive (true);

		player.SetActive (false);

		EndLevel ();

		//TODO call savedata and endlevel


	}

	// called to destroy the current level path
	// never called directly by the UI
	void EndLevel ()
	{
		is_playing = false;

		if (m_path != null) {
			for (int i = 0; i < m_path.Length; i++) {
				Destroy (m_path [i]);
			}
		}
	}



	/* these function are in common between all the games
	 * so can be called from here directly 
	 */

	public int GetScore ()
	{
		return score;
	}

	public void AddPoints ()
	{
		score = score + 10;

	}
		

}
