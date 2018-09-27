using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using POLIMIGameCollective;

public class CarPathGenerator : Singleton<CarPathGenerator>
{
	public GameObject diamond;


	public GameObject goal;

	bool yaw = true;

	//this two attributes are the whole length of the game must not be changed
	float trajectory_length;

	[Range (1, 50)]
	public int coeff_trajectory_lenght = 30;



	CarPath car_path;

	//5f is the maximum amplitude possible


	// Use this for initialization
	void Start ()
	{
		trajectory_length = Mathf.PI * coeff_trajectory_lenght;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void TrueYawBool ()
	{
		yaw = true;
	}

	public void FalseYawBool ()
	{
		yaw = false;
	}


	//TODO this is called by UI and used to load the path data
	public void LoadPath (string filePath)
	{
		
		Debug.Log (filePath.ToString ());

		string carPath = File.ReadAllText (filePath);

		car_path = JsonUtility.FromJson<CarPath> (carPath);

		LoadDiamonds ();

	}



	void LoadDiamonds ()
	{
		//diamond coordinates
		float y = 1f;
		float x = 0;

		float y_start = y;



		for (int h = 0; h < car_path.car_sections.Length; h++) {

			for (int i = 0; i < car_path.car_sections [h].num_items; i++) {
				y = i * (trajectory_length / car_path.car_sections [h].num_items);
				x = car_path.curve_amplitude *
				(Mathf.Sin (((Mathf.PI * 2) / trajectory_length) * y - ((Mathf.PI / 2) * car_path.car_sections [h].curve_position))
				+ car_path.car_sections [h].curve_position);

				//new y is traslated of y_start based on the end of the previous curve
				y = y + y_start;

				if (yaw) {
					Instantiate (diamond, new Vector3 (x, y, 0), Quaternion.identity);
				} else {
					Instantiate (diamond, new Vector3 (x, y, 0), Quaternion.Euler (0f, 0f, 90f));
				}

			}

			//y starts from the previous end curve + 5f of offset
			y_start = y + 5f;

		}

		goal.transform.position = new Vector3 (0, y_start + 5f, 0);

		
	}
}
