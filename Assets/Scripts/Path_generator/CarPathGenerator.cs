using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPathGenerator : MonoBehaviour
{
	public GameObject diamond;

	float trajectory_length = 1f;

	int n_items = 5;

	float amplitude = 1.5f;

	//values: -1, 0, 1
	int[] curve_position = new int[] { 0, 1, -1 };

	// Use this for initialization
	void Start ()
	{
		//diamond coordinates
		float y = 0;
		float x = 0;

		for (int h = 0; h < curve_position.Length; h++) {
			for (int i = 0; i < n_items; i++) {
				y = i * (trajectory_length / n_items);
				x = amplitude *
				Mathf.Sin (((Mathf.PI * 2) / trajectory_length) * y + ((Mathf.PI / 2) * (-curve_position [h])))
				+ curve_position [h];

				Instantiate (diamond, new Vector3 (x, y, 0), Quaternion.identity);
			
			}
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
