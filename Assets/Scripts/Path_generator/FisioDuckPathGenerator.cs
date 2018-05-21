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


	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < N; i++) {
			front [i] = false;
			middle [i] = false;
			back [i] = false;
		}
			
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void SetBackBool (int i)
	{
		if (i < N) {
			back [i] = back_buttons [i].isOn;
			Debug.Log ("back: " + i.ToString () + " " + back [i].ToString ());
		}
	}

	public void SetMiddleBool (int i)
	{
		if (i < N) {
			middle [i] = middle_buttons [i].isOn;
			Debug.Log ("middle: " + i.ToString () + " " + middle [i].ToString ());
		}
	}

	public void SetFrontBool (int i)
	{
		if (i < N) {
			front [i] = front_buttons [i].isOn;
			Debug.Log ("front: " + i.ToString () + " " + front [i].ToString ());
		}

	}
}
