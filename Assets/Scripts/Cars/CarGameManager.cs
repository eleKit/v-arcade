using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarGameManager : MonoBehaviour
{

	[Header ("Cheat Flag")]
	public bool cheat;

	[Header ("Score Screen")]
	public GameObject m_score_screen;
	public Text m_score_text;


	[Header ("Loading time for Level")]
	[Range (0f, 4f)]
	public float m_loading_time = 0.5f;

	public GameObject player;


	public GameMenuScript menu_GUI;

	//attribute used to set the winning point coordinates
	public GameObject winning_Point;


	private Vector3 player_initial_pos = new Vector3 (0f, -5.7f, 0f);

	//gameobject array containing the diamonds whose position indicates the level path
	private GameObject[] m_path;


	//bool to deactivate player if the game is paused
	private bool is_playing = false;

	// Use this for initialization
	void Start ()
	{
		//music starts
		//MusicManager.Instance.PlayMusic ("GameplayMusic");

		//cleans from all sound effects
		//SfxManager.Instance.Stop ();

		//reset car position and deactivates car gameObj 
		ResetPlayer ();
		//the scene begins with the game main menu
		menu_GUI.menu = true;

		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown ("escape") && is_playing) {
			PauseLevel ();
		}
		
	}


	public void ResetPlayer ()
	{
		player.GetComponent<CarControllerScript> ().SetGravityToZero ();
		player.transform.position = player_initial_pos;
		player.SetActive (false);
	}

	//called when the user choses one level
	public void ChooseLevel (string n)
	{
		StartCoroutine (LoadLevel (name));

	}

	IEnumerator LoadLevel (string name)
	{

		yield return new WaitForSeconds (m_loading_time);
		//TODO generate the path
		player.SetActive (true);

		is_playing = true;

		m_score_screen.SetActive (true);

		//reset car position
		player.transform.position = player_initial_pos;



	}

	//called when the player reaches the end of the level
	public void WinLevel ()
	{
		is_playing = false;
		StartCoroutine ("WinCoroutine");
	}


	IEnumerator WinCoroutine ()
	{
		

		m_score_screen.SetActive (false);

		//Music Manager mute the main jingle
		//MusicManager.Instance.MuteAll ();

		//lose jingle sound
		//SfxManager.Instance.Play ("win_jingle");

		player.SetActive (false);

		yield return new WaitForSeconds (2.5f);

		EndLevel ();

		menu_GUI.win = true;


	}

	// called to destroy the current level path
	// never called directly by the UI
	void EndLevel ()
	{
		is_playing = false;
		//SfxManager.Instance.Unmute ();
		//SfxManager.Instance.Stop ();

		// destroy the currently allocated level screen when a level ends winning/losing


		//Unmutes Music Manager main jingle
		//MusicManager.Instance.UnmuteAll ();

		for (int i = 0; i < m_path.Length; i++) {
			Destroy (m_path [i]);
		}

	}

	//called when the player pauses the game
	void PauseLevel ()
	{
		//SfxManager.Instance.Mute ();
		is_playing = false;

		player.SetActive (false);

		menu_GUI.pause = true;

	}


	//triggered by the button "continue" in the pause screen
	public void ResumeLevel ()
	{
		is_playing = false;
		//SfxManager.Instance.Unmute ();
		player.SetActive (false);
		;
	}
}
