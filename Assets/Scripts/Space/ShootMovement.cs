﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMovement : MonoBehaviour
{

	[Range (0, 10)]
	public float y_movement = 1;

	public bool enemy_attack = false;

	float max_boundary = 10f;


	// Use this for initialization
	void Start ()
	{
		if (enemy_attack) {
			y_movement = y_movement * (-1f);
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameManager.Instance.Get_Is_Playing ()) {
			Vector3 new_position = transform.position + new Vector3 (0, y_movement, 0);

			transform.position = new_position;

			if (transform.position.y > max_boundary) {
				Destroy (gameObject);
			}
		}
	}
}
