using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using POLIMIGameCollective;

public class DuckPathGenerator : Singleton<DuckPathGenerator>
{
	//prefabs to instantiate
	[Header ("GameObject of training levels")]
	public GameObject m_back_duck_left;
	public GameObject m_back_duck_right;

	public GameObject m_middle_duck_left;
	public GameObject m_middle_duck_right;


	public GameObject m_front_duck_left;
	public GameObject m_front_duck_right;

	[Header ("GameObject of standard levels")]
	public GameObject[] front_targets;
	public GameObject[] middle_targets;
	public GameObject[] back_targets;


	DuckPath duck_path;

	//string where i save the persistent data path to check if it is a training level
	string training_path;
	//if the level is a standard one it is saved inside the Resources folder and loaded with Resource.Load

	// Use this for initialization
	void Start ()
	{
		training_path = Application.persistentDataPath;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	//TODO this is called by UI and used to load the path data
	public void LoadPath (string filePath)
	{
		//check if the filePath contains the persistentDataPath, in this case it is a training level
		if (filePath.Contains (training_path)) {
			string duckPath = File.ReadAllText (filePath);

			duck_path = JsonUtility.FromJson<DuckPath> (duckPath);

			LoadDucks ();
		} else {
			TextAsset txt = Resources.Load<TextAsset> (filePath);
			Debug.Log (filePath);
			string textFile = txt.text;

			duck_path = JsonUtility.FromJson<DuckPath> (textFile);

			Debug.Log (textFile);

			LoadStandardDucks ();
		}

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



	void LoadStandardDucks ()
	{

		//front
		for (int i = 0; i < duck_path.standard_model.front_generation_indexes.Length; i++) {
			Instantiate (front_targets [duck_path.standard_model.front_generation_indexes [i]],
				new Vector3 (duck_path.standard_model.front_obstacles_x [i], DuckStandard.FRONT_Y, 0f), Quaternion.identity);
		} 

		//middle
		for (int i = 0; i < duck_path.standard_model.middle_generation_indexes.Length; i++) {
			Instantiate (middle_targets [duck_path.standard_model.middle_generation_indexes [i]],
				new Vector3 (duck_path.standard_model.middle_obstacles_x [i], DuckStandard.MIDDLE_Y, 0f), Quaternion.identity);
		} 

		//back
		for (int i = 0; i < duck_path.standard_model.back_generation_indexes.Length; i++) {
			Instantiate (back_targets [duck_path.standard_model.back_generation_indexes [i]],
				new Vector3 (duck_path.standard_model.back_obstacles_x [i], DuckStandard.BACK_Y, 0f), Quaternion.identity);
		} 
	}
	
}
