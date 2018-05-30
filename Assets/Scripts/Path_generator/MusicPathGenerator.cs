using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using POLIMIGameCollective;

public class MusicPathGenerator : Singleton<MusicPathGenerator>
{
	public GameObject left_hand;

	public GameObject right_hand;


	/* TODO load from file for every music the offset array for every hand
	 */
	//this changes for every music

	float[] instantiationTimer_left;
	//= new float[5] { 1f, 10f, 20f, 30f, 10f };

	float[] instantiationTimer_right;
	//= new float[5] { 1f, 10f, 10f, 10f, 10f };

	Vector3 left_position = new Vector3 (-6f, 0f, 0f);
	Vector3 right_position = new Vector3 (-6f, 2f, 0f);

	int left;
	int right;

	float spawn_time_left;

	float spawn_time_right;





	string dataPath = "Assets/MusicTexts";


	// Use this for initialization
	void Start ()
	{
		
	}



	void Update ()
	{
		if (GameManager.Instance.Get_Is_Playing ()) {
			if (Time.time > spawn_time_left && left < instantiationTimer_left.Length) {
				//do stuff here (like instantiate)
				Instantiate (left_hand, left_position, Quaternion.identity);


				//increment next_spawn_time
				left++;
				if (left < instantiationTimer_left.Length)
					spawn_time_left = Time.time + instantiationTimer_left [left];
			}
			
			if (Time.time > spawn_time_right && right < instantiationTimer_right.Length) {
				Instantiate (right_hand, right_position, Quaternion.identity);

				right++;
				if (right < instantiationTimer_right.Length)
					spawn_time_right = Time.time + instantiationTimer_right [right];
			}

			if (left >= instantiationTimer_left.Length && right >= instantiationTimer_right.Length) {
				MusicGameManager.Instance.no_more_hands = true;
			}
		}
	}


	public void SetupMusicPath (string path)
	{
		ReadPath (path);
		
		left = 0;
		right = 0;

		spawn_time_left = Time.time + instantiationTimer_left [left];

		spawn_time_right = Time.time + instantiationTimer_right [right];
	}




	void ReadPath (string filename)
	{
		
		StreamReader reader = new StreamReader (filename); 

		if (!reader.EndOfStream) {
			string inp_ln = reader.ReadLine ();
			instantiationTimer_left = Array.ConvertAll (inp_ln.Split (','), float.Parse);
		}

		if (!reader.EndOfStream) {
			string inp_ln = reader.ReadLine ();
			instantiationTimer_right = Array.ConvertAll (inp_ln.Split (','), float.Parse);
		}

		reader.Close ();  

	}
}
