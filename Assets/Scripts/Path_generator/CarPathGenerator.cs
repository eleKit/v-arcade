using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPathGenerator : MonoBehaviour
{
	public GameObject diamond;


	public GameObject goal;


	//this is the whole length of the game must not be changed
	float trajectory_length;

	[Range (1, 50)]
	public int coeff_trajectory_lenght = 30;

	int[] n_items = new int[] { 10, 25, 5 };

	//5f is the maximum amplitude possible
	float amplitude = 5f;

	//values: -1, 0, 1
	int[] curve_position = new int[] { -1, -1, 0 };


	//size of n_items and curve_position arrays
	//the game is divided into 3 portions: start, middle, end
	const int N = 3;

	// Use this for initialization
	void Start ()
	{
		trajectory_length = Mathf.PI * coeff_trajectory_lenght;
		goal.transform.position = new Vector3 (0, (trajectory_length * 3) + 10, 0);
		
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

		float y_start = y;

		if (curve_position.Length == n_items.Length) {

			for (int h = 0; h < curve_position.Length; h++) {

				if (curve_position [h] != (-2)) {

					for (int i = 0; i < n_items [h]; i++) {
						y = i * (trajectory_length / n_items [h]);
						x = amplitude *
						(Mathf.Sin (((Mathf.PI * 2) / trajectory_length) * y - ((Mathf.PI / 2) * curve_position [h]))
						+ curve_position [h]);

						//y starts from the previous end curve + 2f of offset
						y = y + y_start + 2f;

						Instantiate (diamond, new Vector3 (x, y, 0), Quaternion.identity);

					}
					y_start = y + 3f;
				} else {
					goal.transform.position = new Vector3 (0, y_start + 7, 0);
					break;
				}
			}
		}
	}
}
