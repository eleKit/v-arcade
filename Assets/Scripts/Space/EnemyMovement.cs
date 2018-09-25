using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	[Range (1, 100)]
	public int m_num_of_enemies_movements = 3;


	[Range (-10, 0)]
	public float y_movement = 1;

	//delta time between enemies movements
	float delta_t = 10f;

	//time of the match, it corresponds to the initial value of the game timer
	float t0;

	//time of next movement
	float t1;

	//minimum y after that line the object is destroyed
	float min_boundary = -10f;

	bool already_going_down;


	// Use this for initialization
	void Start ()
	{
		t0 = SpaceGameManager.Instance.m_time_of_Timer;
		delta_t = t0 / m_num_of_enemies_movements;
		Debug.Log ("inital delta t " + delta_t.ToString ());
		t1 = t0 - delta_t;
		already_going_down = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (SpaceGameManager.Instance.GetTimer () < t1 && !already_going_down && !GetComponent<EnemyShot> ().is_shot) {

			already_going_down = true;

			Vector3 next_pos = transform.position + new Vector3 (0f, y_movement, 0f); 
			transform.position = next_pos;
			t1 = t1 - delta_t;

			already_going_down = false;

			if (transform.position.y < min_boundary) {
				Destroy (gameObject);
			}


		}

	}
}
