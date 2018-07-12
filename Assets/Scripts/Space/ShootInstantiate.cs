using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootInstantiate : MonoBehaviour
{


	public GameObject shot;

	float spawn_time;

	float delta_spawn_time = 2f;

	// Use this for initialization
	void Start ()
	{
		Instantiate (shot, this.transform.position, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//if (GameManager.Instance.Get_Is_Playing ()) {
		if (Time.time > spawn_time) {
			Instantiate (shot, transform.position, Quaternion.identity);
			spawn_time = spawn_time + delta_spawn_time;
		}


		//}



	}
}
