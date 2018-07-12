using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;
using System;
using System.IO;


public class FisioSpacePathGenerator :Singleton<FisioSpacePathGenerator>
{
	SpacePath space_path;

	public Toggle[] front_buttons = new Toggle[SpaceSection.M];
	public Toggle[] middle_buttons = new Toggle[SpaceSection.M];
	public Toggle[] back_buttons = new Toggle[SpaceSection.M];

	string name_path = "";

	// Use this for initialization
	void Start ()
	{
		space_path = new SpacePath ();

		space_path.back = new SpaceSection ();
		space_path.back.section = SpaceSection.SpaceGameSection.Back;

		space_path.middle = new SpaceSection ();
		space_path.middle.section = SpaceSection.SpaceGameSection.Middle;

		space_path.front = new SpaceSection ();
		space_path.front.section = SpaceSection.SpaceGameSection.Front;



		Reset ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Reset ()
	{
		for (int i = 0; i < SpaceSection.M; i++) {

			space_path.back.enemies [i] = false;
			front_buttons [i].isOn = false;


			space_path.middle.enemies [i] = false;
			middle_buttons [i].isOn = false;


			space_path.front.enemies [i] = false;
			back_buttons [i].isOn = false;
		}
	}


	public void SaveEnemiesBool ()
	{

		for (int i = 0; i < SpaceSection.M; i++) {

			space_path.back.enemies [i] = back_buttons [i].isOn;

			space_path.middle.enemies [i] = middle_buttons [i].isOn;

			space_path.front.enemies [i] = front_buttons [i].isOn;

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
			space_path.timestamp = gameDate.ToFileTimeUtc ();
			space_path.doctorName = GlobalDoctorData.globalDoctorData.doctor;

			space_path.id_path = name_path;

			string directoryPath = Path.Combine (Application.persistentDataPath,
				                       Path.Combine ("Paths", GameMatch.GameType.Space.ToString ()));

			Directory.CreateDirectory (directoryPath);
			string filePath = Path.Combine (
				                  directoryPath,
				                  GameMatch.GameType.Space.ToString () + "_"
				                  + FromNameToFilename (name_path) + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json"
			                  );

			string jsonString = JsonUtility.ToJson (space_path);
			File.WriteAllText (filePath, jsonString);


			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().name);
		}
	}


	string FromNameToFilename (string name)
	{
		return name.Replace (" ", "-");

	}


}
