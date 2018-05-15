using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPathGenerator : MonoBehaviour
{
	public GameObject diamond;

	//this is the whole length of the game must not be changed
	float trajectory_length = 140f;

	int n_items = 50;

	float amplitude = 10f;

	//values: -1, 0, 1
	int[] curve_position = new int[] { -1, 1, -1 };

	// Use this for initialization
	void Start ()
	{
		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void LoadDiamonds ()
	{
		//diamond coordinates
		float y = 1f;
		float x = 0;

		int y_start = Mathf.RoundToInt (y);

		for (int h = 0; h < curve_position.Length; h++) {
			for (int i = y_start; i < n_items; i++) {
				y = i * (trajectory_length / n_items);
				x = amplitude *
				Mathf.Sin (((Mathf.PI * 2) / trajectory_length) * y + ((Mathf.PI / 2) * (-curve_position [h])))
				+ curve_position [h];

				Instantiate (diamond, new Vector3 (x, y + 1, 0), Quaternion.identity);

			}
			y_start = Mathf.RoundToInt (y);
		}
	}
}
