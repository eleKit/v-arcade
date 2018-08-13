using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class ShootInstantiate : MonoBehaviour
{


	public GameObject shot;

	[Range (0f, 5f)]
	public float delta_t = 2f;

	float spawn_time = 0f;


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

				spawn_time += Time.deltaTime;

				if (spawn_time > delta_t) {
					Instantiate (shot, transform.position, Quaternion.identity);
					SfxManager.Instance.Play ("laser");
					spawn_time = 0f;
				}
			} 
		}



	}


	void StartShootTimer ()
	{
		spawn_time = 0f;
	}

	public void ResetShootTimer ()
	{
		start_game = false;
	}
}
