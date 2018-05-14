using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathGeneratorGUI : MonoBehaviour
{

	/* car screen elements */


	int curve_amplitude = 1;


	/* shooting screen elements */
	const int N = 3;
	const int M = 6;
	public bool[,] m_ducks_position = new bool[N, M];



	/* initial screen*/


	public GameObject m_initial_BG;

	public GameObject m_save_screen;

	public GameObject m_duck_screen;

	public GameObject m_duck_instruction_screen;

	public GameObject m_duck_path_screen;

	public GameObject m_car_screen;




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


	public void LoadCar ()
	{
		ClearScreens ();
		m_car_screen.SetActive (true);
	}

	public void LoadDuck ()
	{
		ClearScreens ();
		m_duck_screen.SetActive (true);
		ResetScreens ();
	}

	public void LoadMenu ()
	{
		ClearScreens ();
		m_initial_BG.SetActive (true);
	}

	public void LoadMainMenu ()
	{
		SceneManager.LoadSceneAsync ("Main_Menu");
	}

	/* car screen scripts */


	public void AddCarCurveAmplitude ()
	{
		curve_amplitude++;
	}

	public void CarPathSave ()
	{
		m_save_screen.SetActive (true);
		
	}

	public void DuckPathSave ()
	{
		m_save_screen.SetActive (true);

	}


	void ClearScreens ()
	{
		if (m_initial_BG != null)
			m_initial_BG.SetActive (false);

		if (m_duck_screen != null)
			m_duck_screen.SetActive (false);

		if (m_car_screen != null)
			m_car_screen.SetActive (false);

		if (m_save_screen != null)
			m_save_screen.SetActive (false);
	}

	void ResetScreens ()
	{
		if (m_duck_screen != null) {
			m_duck_instruction_screen.SetActive (false);
			m_duck_path_screen.SetActive (false);
			StartCoroutine (LoadDucks ());
		}
	}

	IEnumerator LoadDucks ()
	{
		m_duck_instruction_screen.SetActive (true);

		yield return new WaitForSeconds (3f);

		m_duck_instruction_screen.SetActive (false);

		m_duck_path_screen.SetActive (true);
	}



}
