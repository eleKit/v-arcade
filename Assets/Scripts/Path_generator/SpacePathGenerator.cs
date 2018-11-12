using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using POLIMIGameCollective;

public class SpacePathGenerator : Singleton<SpacePathGenerator>
{

	//prefabs to instantiate
	[Header ("GameObject of training levels")]
	public GameObject[] m_enemies_prefabs;


	[Header ("GameObject of standard levels")]
	public GameObject[] targets;


	//these two attributes are the whole length of the game must not be changed

	[Range (1, 50)]
	public int coeff_trajectory_lenght = 30;
	float trajectory_length;

	//set to false when a pitch match is going on
	bool yaw = true;

	SpacePath space_path;

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


	//this is called by UI and used to load the path data
	public void LoadPath (string filePath)
	{


		string spacePath = File.ReadAllText (filePath);

		space_path = JsonUtility.FromJson<SpacePath> (spacePath);

		LoadEnemies ();

	}


	void LoadEnemies ()
	{
		if (!space_path.standard) {
			//diamond coordinates
			float y = SpacePath.Y_OFFSET_COORD;
			float x = 0f;

			float y_start = y;

			for (int h = 0; h < space_path.space_sections.Length; h++) {

				for (int i = 0; i < space_path.space_sections [h].num_enemies; i++) {


					y = i * (trajectory_length / space_path.space_sections [h].num_enemies);
					x = space_path.curve_amplitude *
					(Mathf.Sin (((Mathf.PI * 2) / trajectory_length) * y - ((Mathf.PI / 2) * space_path.space_sections [h].curve_position))
					+ space_path.space_sections [h].curve_position);

					//new y is traslated of y_start based on the end of the previous curve
					y = y + y_start;


					//if (i < Mathf.RoundToInt (3 / 4 * space_path.space_sections [h].num_enemies)) {
					if (yaw) {
						Instantiate (m_enemies_prefabs [Random.Range (0, m_enemies_prefabs.Length)], new Vector3 (x, y, 0), Quaternion.identity);
					} else {
						Instantiate (m_enemies_prefabs [Random.Range (0, m_enemies_prefabs.Length)], new Vector3 (x, y, 0), Quaternion.Euler (0f, 0f, 90f));
					}

				}

				//y starts from the previous end curve + 5f of offset
				y_start = y + 5f;
		
			}
		} else {

			//extreme front
			for (int i = 0; i < space_path.standard_model.extreme_front_generation_indexes.Length; i++) {
				Instantiate (targets [space_path.standard_model.extreme_front_generation_indexes [i]],
					new Vector3 (space_path.standard_model.extreme_front_enemies_x [i], SpaceStandard.EXTREME_FRONT_Y, 0f), Quaternion.identity);
			} 

			//front
			for (int i = 0; i < space_path.standard_model.front_generation_indexes.Length; i++) {
				Instantiate (targets [space_path.standard_model.front_generation_indexes [i]],
					new Vector3 (space_path.standard_model.front_enemies_x [i], SpaceStandard.FRONT_Y, 0f), Quaternion.identity);
			} 

			//middle
			for (int i = 0; i < space_path.standard_model.middle_generation_indexes.Length; i++) {
				Instantiate (targets [space_path.standard_model.middle_generation_indexes [i]],
					new Vector3 (space_path.standard_model.middle_enemies_x [i], SpaceStandard.MIDDLE_Y, 0f), Quaternion.identity);
			} 

			//back
			for (int i = 0; i < space_path.standard_model.back_generation_indexes.Length; i++) {
				Instantiate (targets [space_path.standard_model.back_generation_indexes [i]],
					new Vector3 (space_path.standard_model.back_enemies_x [i], SpaceStandard.BACK_Y, 0f), Quaternion.identity);
				
			
			}

	
		}
	}
}
