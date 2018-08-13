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


	/* To be playable in the txt file the distance between two spauned buttons must be >= 5f
	 */



	//these two arrays change for every music

	float[] instantiationTimer_left;
	//= new float[5] { 1f, 10f, 20f, 30f, 10f };

	float[] instantiationTimer_right;
	//= new float[5] { 1f, 10f, 10f, 10f, 10f };

	Vector3 left_position = new Vector3 (-6f, 1f, 0f);
	Vector3 right_position = new Vector3 (-6f, 1f, 0f);

	//index of the two arrays of intantiationTimer
	int left = 0;
	int right = 0;

	//delta time of frames passed between an hand instantiation
	float delta_spawn_time_left = 0f;
	float delta_spawn_time_right = 0f;





	string dataPath = "Assets/MusicTexts";


	// Use this for initialization
	void Start ()
	{
		
	}



	void FixedUpdate ()
	{
		if (GameManager.Instance.Get_Is_Playing ()) {

			//at every update the time goes down to
			delta_spawn_time_left += Time.deltaTime;
			delta_spawn_time_right += Time.deltaTime;

			if (left < instantiationTimer_left.Length) {
				
				if (delta_spawn_time_left > instantiationTimer_left [left]) {
					//do stuff here (like instantiate)

					Debug.Log ("delta spawn time left " + Mathf.Round (delta_spawn_time_left).ToString ());
					Debug.Log ("delta arrat time left " + Mathf.Round (instantiationTimer_left [left]).ToString ());


					Instantiate (left_hand, left_position, Quaternion.identity);

					//reset delta_spawn_time
					//delta_spawn_time_left = 0;

					//increment next_spawn_time
					left++;

				}
			}

			if (right < instantiationTimer_right.Length) {

				if (delta_spawn_time_right > instantiationTimer_right [right]) {
					//do stuff here (like instantiate)

					Debug.Log ("delta spawn time right " + Mathf.Round (delta_spawn_time_right).ToString ());
					Debug.Log ("delta arrat time right " + Mathf.Round (instantiationTimer_right [right]).ToString ());


					Instantiate (right_hand, right_position, Quaternion.identity);

					//reset delta_spawn_time
					//delta_spawn_time_right = 0;

					//increment next_spawn_time
					right++;

				}
			}


			if (left >= instantiationTimer_left.Length && right >= instantiationTimer_right.Length) {
				MusicGameManager.Instance.no_more_hands = true;

			}
		}

	}


	//this function is called before the game starts (i.e. when GameManager is_playing = false)
	public void SetupMusicPath (string path)
	{
		ReadPath (path);
		
		left = 0;
		right = 0;

		/* after a correct delta time is passed a new hand button is spawned,
		 * at the beginning the delta time of every hand is set to 0f
		 */


		delta_spawn_time_left = 0f;

		delta_spawn_time_right = 0f;

	}



	void ReadPath (string filename)
	{
		
		StreamReader reader = new StreamReader (filename); 

		/* the first line contains the spauning seconds of the left hand buttons
		 * the second line the ones of the right hand buttons
		 */

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
