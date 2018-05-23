using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

public class FisioDuckPathGenerator : Singleton<FisioDuckPathGenerator>
{

	DuckPath ducks_front_position;

	DuckPath ducks_middle_position;

	DuckPath ducks_back_position;


	public Toggle[] front_buttons = new Toggle[DuckPath.N];
	public Toggle[] middle_buttons = new Toggle[DuckPath.N];
	public Toggle[] back_buttons = new Toggle[DuckPath.N];


	string name_path = "";



	// Use this for initialization
	void Start ()
	{
		ducks_front_position = new DuckPath ();
		ducks_front_position.section = DuckPath.DuckSection.Front;

		ducks_middle_position = new DuckPath ();
		ducks_middle_position.section = DuckPath.DuckSection.Middle;

		ducks_back_position = new DuckPath ();
		ducks_back_position.section = DuckPath.DuckSection.Back;



		Reset ();
			
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Reset ()
	{
		
		for (int i = 0; i < DuckPath.N; i++) {

			ducks_front_position.ducks [i] = false;
			front_buttons [i].isOn = false;
	

			ducks_middle_position.ducks [i] = false;
			middle_buttons [i].isOn = false;
	

			ducks_back_position.ducks [i] = false;
			back_buttons [i].isOn = false;
		}


	}


	public void SaveDuckBool ()
	{
		
		for (int i = 0; i < DuckPath.N; i++) {
			
			ducks_front_position.ducks [i] = front_buttons [i].isOn;
	
			ducks_middle_position.ducks [i] = middle_buttons [i].isOn;
	
			ducks_back_position.ducks [i] = back_buttons [i].isOn;
		}
		


	}



	public void SetPathName (string name)
	{
		name_path = name;
		Debug.Log (name_path);
		
	}


	public void SavePath ()
	{

		//TODO parte di salvataggio



		SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().name);
	}
}
