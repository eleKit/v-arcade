using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

public class WelcomeMenuUI : Singleton<WelcomeMenuUI>
{

	public GameObject m_welcome_canvas;
	public GameObject m_insert_name_canvas;

	[Header ("Set this bool if the build is the one with the non repeatable name insertion")]
	public bool not_repeatable;

	/* Fetch the value from the PlayerPrefs.
	 * PlayerPrefts.GetInt("First_Access", 0) if no int of this name exists, the default is 0.
	 * After the first access to he game the value in PlayerPrefts is set to 1
	 * 
	 * NB: the int first_access, the PlayerPrefs.GetInt(p_prefs_name, 0) and PlayerPrefs.SetInt (p_prefs_name, 1)
	 * are defined only inside the if(not_repeatable) condition, these are never used outside it!
	 * 
	 */
	private int first_access;
	private string p_prefs_name = "First_Access";


	// Use this for initialization
	void Start ()
	{
		ClearScreens ();
		if (not_repeatable) {
			first_access = PlayerPrefs.GetInt (p_prefs_name, 0);
		}
		m_welcome_canvas.SetActive (true);

	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	void ClearScreens ()
	{
		if (m_welcome_canvas != null) {
			m_welcome_canvas.SetActive (false);
		}
		if (m_insert_name_canvas != null) {
			m_insert_name_canvas.SetActive (false);
		}
	}


	public void LoadNextMenu ()
	{

		ClearScreens ();

		//If the not_repeatable is true the name of the player can be loaded only one time forever
		if (not_repeatable) {
			if (first_access == 0) {
				//save new player
				m_insert_name_canvas.SetActive (true);
			} else {
				//load existing player
				GlobalPlayerData.globalPlayerData.LoadPlayer ();
				SceneManager.LoadSceneAsync ("Tuning_scene");
			}
		} else {
			/* save new player (repeatable): 
			 * in case it is possible to insert different players in the application
			 * the player is always saved as new in the PlayerPrefs at the value "Name" 
			 * (see GlobalPlayerData.player_prefs_name_child)
			 */
			m_insert_name_canvas.SetActive (true);
		}
		
	}

	/* this function is called by the InsertPlayerName method Save () 
	 * only if the name player save has worked well 
	 * (i.e. only when the "Salva" button is pressed and TODO only if the name is in the doctor list)
	 */
	public void AccessDone ()
	{
		if (not_repeatable) {
			//the insertion has been done
			PlayerPrefs.SetInt (p_prefs_name, 1);
		} else {
			//Do nothing
		}
	}


	//use this only for debugging!!
	public void DebuggingResetPlayerPrefs ()
	{
		PlayerPrefs.SetInt (p_prefs_name, 0);
		// PlayerPrefs.DeleteAll();
	}
		
}
