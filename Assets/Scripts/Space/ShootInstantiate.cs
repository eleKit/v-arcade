using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootInstantiate : MonoBehaviour
{


	public GameObject shot;

	float spawn_time = 0f;

	float delta_spawn_time = 2f;

	bool start_game;

	// Use this for initialization
	void Start ()
	{
		start_game = false;
	}



	// Update is called once per frame
	void Update ()
	{
		if (GameManager.Instance.Get_Is_Playing () && !start_game) {
			StartShootTimer ();
			start_game = true;
		}

		if (start_game) {
			if (GameManager.Instance.Get_Is_Playing () &&
			    !(GetComponent<SpriteRenderer> ().color.Equals (Color.black)
			    || GetComponent<SpriteRenderer> ().color.Equals (Color.gray))) {
				if (Time.time > spawn_time) {
					Instantiate (shot, transform.position, Quaternion.identity);
					spawn_time = spawn_time + delta_spawn_time;
				}
			} else {
				if (Time.time > spawn_time)
					spawn_time = spawn_time + delta_spawn_time;
			}
		}



	}


	void StartShootTimer ()
	{
		spawn_time = Time.time + delta_spawn_time;
	}
}
