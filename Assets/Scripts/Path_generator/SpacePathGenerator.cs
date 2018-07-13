using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using POLIMIGameCollective;

public class SpacePathGenerator : Singleton<SpacePathGenerator>
{

	//prefabs to instantiate
	public GameObject[] m_enemies_prefabs;

	SpacePath space_path;

	// Use this for initialization
	void Start ()
	{
		
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
		for (int i = 0; i < space_path.back.enemies.Length; i++) {
			if (space_path.back.enemies [i]) {
				Instantiate (m_enemies_prefabs [Random.Range (0, m_enemies_prefabs.Length - 1)], space_path.back.front_enemies_coord [i] + new Vector3 (0, SpaceSection.Y_OFFSET_COORD * 2, 0), Quaternion.identity);
			}
		}

		for (int i = 0; i < space_path.middle.enemies.Length; i++) {
			if (space_path.middle.enemies [i]) {
				Instantiate (m_enemies_prefabs [Random.Range (0, m_enemies_prefabs.Length - 1)], space_path.back.front_enemies_coord [i] + new Vector3 (0, SpaceSection.Y_OFFSET_COORD, 0), Quaternion.identity);
			}
		}

		for (int i = 0; i < space_path.front.enemies.Length; i++) {
			if (space_path.front.enemies [i]) {
				Instantiate (m_enemies_prefabs [Random.Range (0, m_enemies_prefabs.Length - 1)], space_path.back.front_enemies_coord [i], Quaternion.identity);
			}
		}
	}
}
