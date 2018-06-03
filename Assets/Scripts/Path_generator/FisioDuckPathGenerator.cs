using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;
using System;
using System.IO;

public class FisioDuckPathGenerator : Singleton<FisioDuckPathGenerator>
{

	DuckPath duck_path;

	public Toggle[] front_buttons = new Toggle[DuckSection.N];
	public Toggle[] middle_buttons = new Toggle[DuckSection.N];
	public Toggle[] back_buttons = new Toggle[DuckSection.N];


	string name_path = "";



	// Use this for initialization
	void Start ()
	{
		duck_path = new DuckPath ();

		duck_path.back = new DuckSection ();
		duck_path.back.section = DuckSection.DuckGameSection.Back;

		duck_path.middle = new DuckSection ();
		duck_path.middle.section = DuckSection.DuckGameSection.Middle;

		duck_path.front = new DuckSection ();
		duck_path.front.section = DuckSection.DuckGameSection.Front;



		Reset ();
			
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Reset ()
	{
		
		for (int i = 0; i < DuckSection.N; i++) {

			duck_path.back.ducks [i] = false;
			front_buttons [i].isOn = false;
	

			duck_path.middle.ducks [i] = false;
			middle_buttons [i].isOn = false;
	

			duck_path.front.ducks [i] = false;
			back_buttons [i].isOn = false;
		}


	}


	public void SaveDuckBool ()
	{
		
		for (int i = 0; i < DuckSection.N; i++) {
			
			duck_path.back.ducks [i] = back_buttons [i].isOn;

			duck_path.middle.ducks [i] = middle_buttons [i].isOn;

			duck_path.front.ducks [i] = front_buttons [i].isOn;

		}
		


	}



	public void SetPathName (string name)
	{
		name_path = name;
		Debug.Log (name_path);
		
	}


	public void SavePath ()
	{
		if (name_path.Equals ("")) {
			//do nothing
		} else {
			DateTime gameDate = DateTime.UtcNow;
			duck_path.timestamp = gameDate.ToFileTimeUtc ();
			duck_path.doctorName = GlobalDoctorData.globalDoctorData.doctor;

			duck_path.id_path = name_path;

			//TODO if the paths are divided into doctors substitute GameMatch.GameType.Shooting.ToString () with duck_path.doctorName
			string directoryPath = Path.Combine (Application.persistentDataPath,
				                       Path.Combine ("Paths", GameMatch.GameType.Shooting.ToString ()));

			Directory.CreateDirectory (directoryPath);
			string filePath = Path.Combine (
				                  directoryPath,
				                  GameMatch.GameType.Shooting.ToString () + "_"
				                  + FromNameToFilename (name_path) + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json"
			                  );

			string jsonString = JsonUtility.ToJson (duck_path);
			File.WriteAllText (filePath, jsonString);


			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().name);
		}
	}


	string FromNameToFilename (string name)
	{
		return name.Replace (" ", "-");

	}
}

