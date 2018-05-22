using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

public class FisioDuckPathGenerator : Singleton<FisioDuckPathGenerator>
{
	public const int N = 6;


	bool[] front = new bool[N];
	bool[] middle = new bool[N];
	bool[] back = new bool[N];


	public Toggle[] front_buttons = new Toggle[N];
	public Toggle[] middle_buttons = new Toggle[N];
	public Toggle[] back_buttons = new Toggle[N];


	string name_path = "";

	// Use this for initialization
	void Start ()
	{
		Reset ();
			
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Reset ()
	{
		
		for (int i = 0; i < N; i++) {
			front [i] = false;
			middle [i] = false;
			back [i] = false;

			front_buttons [i].isOn = false;
			middle_buttons [i].isOn = false;
			back_buttons [i].isOn = false;
		}
	}


	public void SaveDuckBool ()
	{
		for (int i = 0; i < N; i++) {
			front [i] = front_buttons [i].isOn;
			middle [i] = middle_buttons [i].isOn;
			back [i] = back_buttons [i].isOn;
			Debug.Log (front [i].ToString ());
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
