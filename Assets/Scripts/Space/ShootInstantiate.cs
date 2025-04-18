﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class ShootInstantiate : MonoBehaviour
{


	public GameObject shot;


	public Color[] m_loading_bar_color;
	public Color m_loading_bar_shoot_color;

	public GameObject[] loading_bar;




	[Range (0f, 5f)]
	public float delta_t = 2f;

	float spawn_time = 0f;

	int index = 0;
	float one_step_value;
	float steps_counter = 0;

	bool start_game, coroutine_started;

	/*player gameobject used to instantiate the shot in the correct position even if the game is paused but the Shoot () instantiation 
	 * Coroutine is already started
	 */
	GameObject pl;

	void Awake ()
	{
		pl = GameObject.FindGameObjectWithTag ("Player");
	}

	// Use this for initialization
	void Start ()
	{
		start_game = false;
		coroutine_started = false;
		one_step_value = delta_t / loading_bar.Length;


	}



	// Update is called once per frame
	void Update ()
	{
		if (GameManager.Instance.Get_Is_Playing () && !start_game) {
			StartShootTimer ();
			start_game = true;

			ClearLoadingBar ();
			ResetLoadingBar ();

		}

		if (start_game) {
			if (GameManager.Instance.Get_Is_Playing ()) {
			
				if (!(pl.GetComponent<SpriteRenderer> ().color.Equals (pl.GetComponent<SpaceGesturesManager> ().transparent_white)
				    || pl.GetComponent<SpriteRenderer> ().color.Equals (pl.GetComponent<SpaceGesturesManager> ().medium_white))) {

					spawn_time += Time.deltaTime;

					if (spawn_time > steps_counter) {
					
						if (index < loading_bar.Length) {
							//ClearLoadingBar ();
							loading_bar [index].GetComponent<SpriteRenderer> ().color = m_loading_bar_color [index];
						}
						index++;
						steps_counter = one_step_value * (index + 1);
					}

					if (spawn_time > delta_t && !coroutine_started) {

						StartCoroutine (Shoot ());
					}
				}
			}
		}



	}


	IEnumerator Shoot ()
	{
		coroutine_started = true;

		if (index < loading_bar.Length) {
			//ClearLoadingBar ();
			loading_bar [index].GetComponent<SpriteRenderer> ().color = m_loading_bar_color [index];
		}

		yield return new WaitForSeconds (0.5f);

		ColorLoadingBarShoot ();


		//if (pl != null) {
		Transform player_pos = pl.GetComponent<Transform> ();
		Instantiate (shot, player_pos.position, Quaternion.identity);
		SfxManager.Instance.Play ("laser");
		//}


		yield return new WaitForSeconds (0.5f);


		spawn_time = 0f;
		ClearLoadingBar ();
		ResetLoadingBar ();
		coroutine_started = false;
	}


	void StartShootTimer ()
	{
		spawn_time = 0f;
	}

	void ClearLoadingBar ()
	{
		
		for (int i = 0; i < loading_bar.Length; i++) {
			loading_bar [i].GetComponent<SpriteRenderer> ().color = Color.white;

		}
			
	}

	void ResetLoadingBar ()
	{
		index = 0;
		steps_counter = one_step_value;
		//loading_bar [index].GetComponent<SpriteRenderer> ().color = m_loading_bar_color;
	}


	void ColorLoadingBarShoot ()
	{
		for (int i = 0; i < loading_bar.Length; i++) {
			loading_bar [i].GetComponent<SpriteRenderer> ().color = m_loading_bar_shoot_color;

		}
	}



}
