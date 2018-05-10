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

	// Use this for initialization
	void Start ()
	{
		GameManager.Instance.BaseStart ();
		GameManager.Instance.menu_GUI.music = true;
		GameManager.Instance.player.SetActive (false);
		ClearScreens ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameManager.Instance.BaseUpdate ();

		if (GameManager.Instance.Get_Is_Playing ()) {

			foreach (GameObject button in GameObject.FindGameObjectsWithTag ("Button")) {
				button.transform.position = new Vector3 (
					button.transform.position.x + m_button_movement_offset,
					button.transform.position.y,
					button.transform.position.z);
			}
		} else {
			//blocca mani e musica
		}

		
	}

	public void ChooseLevel (string name)
	{
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

	public void AddPoints ()
	{
		GameManager.Instance.BaseAddPoints ();
		StartCoroutine (Yeah ());

	}

	IEnumerator Yeah ()
	{
		if (right_trigger) {
			m_right_yeah.SetActive (true);	
		}

		if (left_trigger) {
			m_left_yeah.SetActive (true);

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
