using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class MusicGameManager : Singleton<MusicGameManager>
{
	[Range (0f, 0.5f)]
	public float m_button_movement_offset = 0.1f;

	public bool right_trigger;

	public bool left_trigger;

	public GameObject m_left_yeah;
	public GameObject m_right_yeah;

	public GameObject left_hand_to_delete;

	public GameObject right_hand_to_delete;

	public bool no_more_hands;

	// Use this for initialization
	void Start ()
	{

		GameManager.Instance.BaseStart ("MusicMenuMusic");

		/* set the bool of the current game in the game manager 
		 * and in the GUI manager
		 */
		//GameManager.Instance.menu_GUI.music = true;
		GameManager.Instance.music = true;


		GameManager.Instance.player.SetActive (false);
		ClearScreens ();
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		GameManager.Instance.BaseUpdate ();

		if (GameManager.Instance.Get_Is_Playing ()) {

			foreach (GameObject button in GameObject.FindGameObjectsWithTag ("LeftButton")) {
				button.transform.position = new Vector3 (
					button.transform.position.x + m_button_movement_offset,
					button.transform.position.y,
					button.transform.position.z);
			}

			foreach (GameObject button in GameObject.FindGameObjectsWithTag ("RightButton")) {
				button.transform.position = new Vector3 (
					button.transform.position.x + m_button_movement_offset,
					button.transform.position.y,
					button.transform.position.z);
			}


			if (GameObject.FindGameObjectsWithTag ("RightButton").Length == 0
			    && GameObject.FindGameObjectsWithTag ("LeftButton").Length == 0
			    && no_more_hands) {
				WinLevel ();		
			}
		} else {
			//blocca mani e musica
		}

		//TODO if music is finished the game ends

		
	}

	public void ChooseLevel (string name)
	{
		no_more_hands = false;

		//the path must be read before launching base choose level!!
		MusicPathGenerator.Instance.ReadPath ();

		MusicManager.Instance.StopAll ();
		//TODO here must be played the game music chosen by the player

		GameObject.Find ("MusicPathGenerator").GetComponent<MusicPathGenerator> ().SetupMusicPath ();

		GameManager.Instance.BaseChooseLevel (name);


	}

	//function called after pause the game
	public void ResumeLevel ()
	{
		GameManager.Instance.BaseResumeLevel ();
	}

	public void RestartLevel ()
	{
		GameManager.Instance.m_wait_background.SetActive (true);

		foreach (GameObject button in GameObject.FindGameObjectsWithTag ("LeftButton")) {
			Destroy (button);
		}

		foreach (GameObject button in GameObject.FindGameObjectsWithTag ("RightButton")) {
			Destroy (button);
		}

		ChooseLevel (GameManager.Instance.current_path);
	}



	public void WinLevel ()
	{
		GameManager.Instance.BaseWinLevel ();
	}


	public int GetScore ()
	{
		return GameManager.Instance.BaseGetScore ();
	}


	/* the trigger OnTriggerEnter of CoinCollectScript sets the bool left|right_trigger that is checked by the PushGesture script
	 * if both the button is inside the trigger and the player has done the push gesture
	 * the PushGesture script calls the AddPoints() funcion
	 */

	public void AddPoints ()
	{
		GameManager.Instance.BaseAddPoints ();
		StartCoroutine (Yeah ());

	}

	IEnumerator Yeah ()
	{
		if (right_trigger) {
			m_right_yeah.SetActive (true);	
			right_trigger = false;
			if (right_hand_to_delete != null) {
				right_hand_to_delete.SetActive (false);
			}
		}

		if (left_trigger) {
			m_left_yeah.SetActive (true);
			left_trigger = false;
			if (left_hand_to_delete != null) {
				left_hand_to_delete.SetActive (false);
			}

		}

		yield return new WaitForSeconds (0.5f);
		ClearScreens ();
	}


	void ClearScreens ()
	{
		if (m_left_yeah != null)
			m_left_yeah.SetActive (false);
		if (m_right_yeah != null)
			m_right_yeah.SetActive (false);
		
	}
}
