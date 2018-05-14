using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckPathGenerator : MonoBehaviour
{

	public GameObject m_back_duck_left;
	public GameObject m_back_duck_right;

	public GameObject m_middle_duck_left;
	public GameObject m_middle_duck_right;


	public GameObject m_front_duck_left;
	public GameObject m_front_duck_right;


	const int N = 6;


	Vector3[] back_coord = new Vector3[N] {new Vector3 (-6f, 3f, 0f), 
		new Vector3 (-3.8f, 3.5f, 0f), new Vector3 (-1.65f, 3f, 0f), new Vector3 (1.45f, 3.5f, 0f),
		new Vector3 (3.85f, 3f, 0f), new Vector3 (6.1f, 3.5f, 0f)
	};


	float middle_y_coord = 2.5f;

	float front_y_coord = 4.5f;

	bool[] front = new bool[N] { true, false, false, false, true, true };
	bool[] middle = new bool[N]{ false, false, false, false, true, true };
	bool[] back = new bool[N]{ true, false, false, false, false, true };


	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < front.Length; i++) {
			if (front [i]) {
				if (i < 3) {
					Instantiate (m_front_duck_left, back_coord [i] - new Vector3 (0, front_y_coord, 0), Quaternion.identity);
				} else if (i > 4) {
					Instantiate (m_front_duck_right, back_coord [i] - new Vector3 (0, front_y_coord, 0), Quaternion.identity);
				}
			}
		}


		for (int i = 0; i < middle.Length; i++) {
			if (middle [i]) {
				if (i < 3) {
					Instantiate (m_middle_duck_left, back_coord [i] - new Vector3 (0, middle_y_coord, 0), Quaternion.identity);
				} else if (i > 4) {
					Instantiate (m_middle_duck_right, back_coord [i] - new Vector3 (0, middle_y_coord, 0), Quaternion.identity);
				}
			}
		}

		for (int i = 0; i < back.Length; i++) {
			if (back [i]) {
				if (i < 3) {
					Instantiate (m_back_duck_left, back_coord [i], Quaternion.identity);
				} else if (i > 4) {
					Instantiate (m_back_duck_right, back_coord [i], Quaternion.identity);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
