using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

	[Range (0f, 100f)]
	public float delta_t = 10f;

	float t0;
	float t1;
	bool already_going_down;


	// Use this for initialization
	void Start ()
	{
		t0 = SpaceGameManager.Instance.m_time_of_Timer;
		Debug.Log ("inital timer in enemies " + t0.ToString ());
		t1 = t0 - delta_t;
		already_going_down = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (SpaceGameManager.Instance.GetTimer () < t1 && !already_going_down) {

			already_going_down = true;

			Debug.Log ("enemy going down");
			Vector3 next_pos = transform.position + new Vector3 (0f, -5f, 0f); 
			transform.position = next_pos;
			t1 = t1 - delta_t;

			already_going_down = false;
		}

	}
}
