using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using POLIMIGameCollective;

public class DuckPathGenerator : Singleton<DuckPathGenerator>
{

	public GameObject m_back_duck_left;
	public GameObject m_back_duck_right;

	public GameObject m_middle_duck_left;
	public GameObject m_middle_duck_right;


	public GameObject m_front_duck_left;
	public GameObject m_front_duck_right;


	DuckPath duck_path;



	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	//TODO this is called by UI and used to load the path data
	public void LoadPath (string filePath)
	{
		

		string duckPath = File.ReadAllText (filePath);

		duck_path = JsonUtility.FromJson<DuckPath> (duckPath);

		LoadDucks ();

	}




	void LoadDucks ()
	{

		for (int i = 0; i < duck_path.front.ducks.Length; i++) {
			if (duck_path.front.ducks [i]) {
				if (i < 3) {
					Instantiate (m_front_duck_left, duck_path.front.back_coord [i] - new Vector3 (0, DuckSection.FRONT_y_coord, 0), Quaternion.identity);
				} else if (i >= 3) {
					Instantiate (m_front_duck_right, duck_path.front.back_coord [i] - new Vector3 (0, DuckSection.FRONT_y_coord, 0), Quaternion.identity);
				}
			}
		}


		for (int i = 0; i < duck_path.middle.ducks.Length; i++) {
			if (duck_path.middle.ducks [i]) {
				if (i < 3) {
					Instantiate (m_middle_duck_left, duck_path.middle.back_coord [i] - new Vector3 (0, DuckSection.MIDDLE_y_coord, 0), Quaternion.identity);
				} else if (i >= 3) {
					Instantiate (m_middle_duck_right, duck_path.middle.back_coord [i] - new Vector3 (0, DuckSection.MIDDLE_y_coord, 0), Quaternion.identity);
				}
			}
		}

		for (int i = 0; i < duck_path.back.ducks.Length; i++) {
			if (duck_path.back.ducks [i]) {
				if (i < 3) {
					Instantiate (m_back_duck_left, duck_path.back.back_coord [i], Quaternion.identity);
				} else if (i >= 3) {
					Instantiate (m_back_duck_right, duck_path.back.back_coord [i], Quaternion.identity);
				}
			}
		}
		
	}
}
