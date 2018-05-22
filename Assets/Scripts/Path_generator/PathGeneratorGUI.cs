using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathGeneratorGUI : MonoBehaviour
{




	/* initial screen*/


	public GameObject m_initial_BG;

	public GameObject m_save_duck_screen;

	public GameObject m_save_car_screen;

	public GameObject m_duck_screen;

	public GameObject m_duck_path_screen;

	public GameObject m_car_path_screen;

	public GameObject m_car_screen;



	public GameObject m_instructions_screen;
	public GameObject m_car_instruction_screen;
	public GameObject m_duck_instruction_screen;
	public GameObject m_instruction_menu;



	// Use this for initialization
	void Start ()
	{
		
		ClearScreens ();
		m_initial_BG.SetActive (true);

	}
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void LoadMenu ()
	{
		ClearScreens ();
		m_initial_BG.SetActive (true);
	}


	/* load instruction screen script */

	public void LoadInstructions ()
	{
		ClearScreens ();
		m_instructions_screen.SetActive (true);
		ResetInstructionScreen ();
		m_instruction_menu.SetActive (true);

	}

	public void LoadCarInstructions ()
	{
		
		ResetInstructionScreen ();
		m_car_instruction_screen.SetActive (true);
	}

	public void LoadDuckInstructions ()
	{
		ResetInstructionScreen ();
		m_duck_instruction_screen.SetActive (true);
	}


	/* load path generator screen script */

	public void LoadCar ()
	{
		ClearScreens ();
		m_car_screen.SetActive (true);
		ResetCarScreens ();
		m_car_path_screen.SetActive (true);
	}



	public void LoadDuck ()
	{
		ClearScreens ();
		m_duck_screen.SetActive (true);
		ResetDuckScreens ();
		m_duck_path_screen.SetActive (true);
	}




	public void LoadMainMenu ()
	{
		SceneManager.LoadSceneAsync ("Main_Menu");
	}


	/* save screen scripts */



	public void CarPathSaveScreen ()
	{
		m_save_car_screen.SetActive (true);
		
	}

	public void GoBackToCar ()
	{
		m_save_car_screen.SetActive (false);
	}

	public void DuckPathSaveScreen ()
	{
		m_save_duck_screen.SetActive (true);

	}

	public void GoBackToDucks ()
	{
		m_save_duck_screen.SetActive (false);
	}



	/* reset screen scripts */

	void ClearScreens ()
	{
		if (m_initial_BG != null)
			m_initial_BG.SetActive (false);

		if (m_instructions_screen != null)
			m_instructions_screen.SetActive (false);

		if (m_duck_screen != null)
			m_duck_screen.SetActive (false);

		if (m_car_screen != null)
			m_car_screen.SetActive (false);


	}

	void ResetDuckScreens ()
	{
		if (m_duck_screen != null) {
			if (m_save_duck_screen != null)
				m_save_duck_screen.SetActive (false);
			if (m_duck_path_screen != null)
				m_duck_path_screen.SetActive (false);
		}
	}

	void ResetCarScreens ()
	{
		if (m_car_screen != null) {
			if (m_save_car_screen != null)
				m_save_car_screen.SetActive (false);
			if (m_car_path_screen != null)
				m_car_path_screen.SetActive (false);
			
		}
	}

	void ResetInstructionScreen ()
	{
		if (m_instructions_screen != null) {
			if (m_car_instruction_screen != null)
				m_car_instruction_screen.SetActive (false);
			if (m_duck_instruction_screen != null)
				m_duck_instruction_screen.SetActive (false);
			if (m_instruction_menu != null)
				m_instruction_menu.SetActive (false);
		}
		
	}





}
