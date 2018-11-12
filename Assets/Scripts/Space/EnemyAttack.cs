using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class EnemyAttack : MonoBehaviour
{

	public GameObject shot;
	//player GO
	GameObject pl = null;


	[Range (0f, 5f)]
	public float delta_t = 3f;

	float spawn_time = 0f;

	bool coroutine_started;



	// Use this for initialization
	void Start ()
	{
		spawn_time = 0f;
		coroutine_started = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (pl == null) {
			pl = GameObject.FindGameObjectWithTag ("Player");
		} else {
			if (GameManager.Instance.Get_Is_Playing ()) {
				if (!(pl.GetComponent<SpriteRenderer> ().color.Equals (pl.GetComponent<SpaceGesturesManager> ().transparent_white)
				    || pl.GetComponent<SpriteRenderer> ().color.Equals (pl.GetComponent<SpaceGesturesManager> ().medium_white))) {

					spawn_time += Time.deltaTime;
				}

				if (spawn_time > delta_t && !coroutine_started) {

					StartCoroutine (Shoot ());
				}
		
			}
		}
	}


	IEnumerator Shoot ()
	{
		coroutine_started = true;

		Instantiate (shot, this.transform.position, Quaternion.identity);
		SfxManager.Instance.Play ("laser");


		yield return new WaitForSeconds (0.5f);

		spawn_time = 0f;

		coroutine_started = false;
	}
}
