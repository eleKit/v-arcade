using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class ShootInstantiate : MonoBehaviour
{


	public GameObject shot;

	public GameObject[] loading_bar;

	[Range (0f, 5f)]
	public float delta_t = 2f;

	float spawn_time = 0f;

	int index = 0;
	float one_step_value;
	float steps_counter = 0;

	bool start_game;

	// Use this for initialization
	void Start ()
	{
		start_game = false;
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
			if (GameManager.Instance.Get_Is_Playing () &&
			    !(GetComponent<SpriteRenderer> ().color.Equals (Color.black)
			    || GetComponent<SpriteRenderer> ().color.Equals (Color.gray))) {

				spawn_time += Time.deltaTime;

				if (spawn_time > steps_counter) {
					index++;
					steps_counter = one_step_value * (index + 1);
					if (index < loading_bar.Length) {
						ClearLoadingBar ();
						loading_bar [index].SetActive (true);
					}
				}

				if (spawn_time > delta_t) {
					Instantiate (shot, transform.position, Quaternion.identity);
					SfxManager.Instance.Play ("laser");
					spawn_time = 0f;

					ClearLoadingBar ();
					ResetLoadingBar ();
				}
			} 
		}



	}


	void StartShootTimer ()
	{
		spawn_time = 0f;
	}

	void ClearLoadingBar ()
	{
		for (int i = 0; i < loading_bar.Length; i++) {
			if (loading_bar [i] != null) {
				loading_bar [i].SetActive (false);
			}
		}
			
	}

	void ResetLoadingBar ()
	{
		index = 0;
		steps_counter = one_step_value;
		loading_bar [index].SetActive (true);
	}



}
