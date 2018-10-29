using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using POLIMIGameCollective;

public class TimerManager : Singleton<TimerManager>
{
	[Header ("game timer")]
	[Range (0f, 200f)]
	public float m_time_of_Timer = 200f;

	public Text m_timer_text;

	float timer_of_game;

	//gameobject of the player used to check if the leap sees the hand so the timer can go on
	GameObject pl;

	void Awake ()
	{
		pl = GameObject.FindGameObjectWithTag ("Player");
	}


	// Use this for initialization
	void Start ()
	{
		ResetTimer ();

	}
	
	// Update is called once per frame
	void Update ()
	{

		//Timer text update
		if (GameManager.Instance.Get_Is_Playing () &&
		    !(pl.GetComponent<SpriteRenderer> ().color.Equals (pl.GetComponent<SpaceGesture> ().transparent_white)
		    || pl.GetComponent <SpriteRenderer> ().color.Equals (pl.GetComponent<SpaceGesture> ().medium_white))) {
			timer_of_game -= Time.deltaTime;
			int timer = (int)Mathf.Round (timer_of_game);
			int min = timer / 60;
			int sec = timer % 60;
			m_timer_text.text = min.ToString () + ":" + sec.ToString ();
		}
		
	}


	public float GetTimer ()
	{
		return timer_of_game;
	}

	public void ResetTimer ()
	{

		//set timer
		timer_of_game = m_time_of_Timer;
		int timer = (int)Mathf.Round (timer_of_game);
		int min = timer / 60;
		int sec = timer % 60;
		m_timer_text.text = min.ToString () + ":" + sec.ToString ();
	}
}
