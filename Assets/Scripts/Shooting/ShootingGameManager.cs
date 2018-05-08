using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingGameManager : MonoBehaviour
{

	public GameObject m_background;

	public GameObject m_wait_background;
	public Text m_wait_text;

	[Header ("Loading time for Level")]
	[Range (0f, 4f)]
	public float m_loading_time = 0.5f;

	public GameObject player;

	public HandController hc;

	public GameMenuScript menu_GUI;

	//gameobject array containing the ducks whose position indicates the level path
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
		ClearScreens ();
		m_background.SetActive (true);
		//reset car position and deactivates car gameObj 
		ResetPlayer ();

		player.SetActive (false);

		//the scene begins with the game main menu
		//menu_GUI.menu = true;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown ("escape") && is_playing) {
			PauseLevel ();
		}

		if (Input.GetKeyDown ("space")) {
			ChooseLevel ("Na");
		}

		if (GameObject.FindGameObjectsWithTag ("Duck").Length == 0 && is_playing) {
			is_playing = false;
			StartCoroutine ("WinCoroutine");
		}
		
	}

	public void ResetPlayer ()
	{
		player.transform.position = new Vector3 (0, 0, 0);
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

		//deactivate BG screen
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


		m_wait_background.SetActive (false);


		//reset car position
		ResetPlayer ();


		Debug.Log ("end LoadLevel");




	}



	//called when the player pauses the game
	void PauseLevel ()
	{

		is_playing = false;

		player.SetActive (false);

		//menu_GUI.pause = true;

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


	IEnumerator WinCoroutine ()
	{

		//menu_GUI.win = true;

		yield return new WaitForSeconds (0.5f);

		m_background.SetActive (true);

		Debug.Log ("you win");

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


	public int GetScore ()
	{
		return score;
	}



	public void AddPoints ()
	{
		score = score + 10;

	}

	void ClearScreens ()
	{
		if (m_background != null)
			m_background.SetActive (false);

		if (m_wait_background != null)
			m_wait_background.SetActive (false);
	}
}
