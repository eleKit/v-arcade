using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using POLIMIGameCollective;

public class CarGameManager : Singleton<CarGameManager>
{

	[Header ("Cheat Flag")]
	public bool cheat;



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

	//name of the path chosen
	private string current_path = "";


	//bool to deactivate player if the game is paused
	private bool is_playing = false;


	//score of the game
	private int score;

	// Use this for initialization
	void Start ()
	{

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

	//This method is used to set the point where the game ends
	public void SetWinningPosition (Vector3 coord)
	{
		winning_Point.transform.position = coord;
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
		current_path = n;
		StartCoroutine (LoadLevel (name));

	}

	IEnumerator LoadLevel (string name)
	{

		yield return new WaitForSeconds (m_loading_time);
		//TODO generate the path

		is_playing = true;

		Debug.Log ("inside LoadLevel");

		//this must be before setting the position
		player.SetActive (true);
		//reset car position
		player.transform.position = player_initial_pos;

		Debug.Log ("end LoadLevel");





	}

	//called when the player reaches the end of the level
	public void WinLevel ()
	{
		is_playing = false;
		Debug.Log ("you win");
		StartCoroutine ("WinCoroutine");
	}


	IEnumerator WinCoroutine ()
	{
		
		menu_GUI.win = true;

		yield return new WaitForSeconds (0.5f);

		player.SetActive (false);

		EndLevel ();




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

	//called when the player pauses the game
	void PauseLevel ()
	{
		
		is_playing = false;

		player.SetActive (false);

		menu_GUI.pause = true;

	}


	//triggered by the button "continue" in the pause screen
	public void ResumeLevel ()
	{
		is_playing = true;
	
		player.SetActive (true);

	}

	public void RestartLevel ()
	{
		Debug.Log ("Load Level call");
		StartCoroutine (LoadLevel (current_path));



	}


	public int GetScore ()
	{
		return score;
	}

	public void AddPoints ()
	{
		score = score + 10;
		
	}
}
