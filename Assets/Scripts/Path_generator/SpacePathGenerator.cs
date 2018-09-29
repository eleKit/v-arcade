using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using POLIMIGameCollective;

public class SpacePathGenerator : Singleton<SpacePathGenerator>
{

	//prefabs to instantiate
	public GameObject[] m_enemies_prefabs;

	public GameObject[] m_big_enemies_prefabs;

	//these two attributes are the whole length of the game must not be changed

	[Range (1, 50)]
	public int coeff_trajectory_lenght = 30;
	float trajectory_length;

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


	//this is called by UI and used to load the path data
	public void LoadPath (string filePath)
	{


		string spacePath = File.ReadAllText (filePath);

		space_path = JsonUtility.FromJson<SpacePath> (spacePath);

		LoadEnemies ();

	}


	void LoadEnemies ()
	{
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
				Instantiate (m_enemies_prefabs [Random.Range (0, m_enemies_prefabs.Length)], new Vector3 (x, y, 0), Quaternion.identity);
				/*} else {
					Instantiate (m_big_enemies_prefabs [Random.Range (0, m_big_enemies_prefabs.Length)], new Vector3 (x, y, 0), Quaternion.identity);
				}*/
			}

			//y starts from the previous end curve + 5f of offset
			y_start = y + 5f;

		
		}

	
	}
}
