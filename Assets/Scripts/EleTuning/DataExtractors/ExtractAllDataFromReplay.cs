using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Leap;
using System;

public class ExtractAllDataFromReplay : MonoBehaviour
{

	public HandController hc;

	public bool roll;

	[Header ("Patient Nickname")]
	public string patientName;
	[Header ("Game")]
	public GameMatch.GameType gameType;
	[Header ("Filename.json")]
	public string filename;

	string directoryPath;
	string hand_data_file_path;

	bool saved_data = false;
	bool loaded_data = false;

	//save the past yaw down angles of left and right hand in the previous K frames
	private List<float> left_flexion = new List<float> ();
	private List<float> right_flexion = new List<float> ();

	//save the past yaw up angles of left and right hand in the previous K frames
	private List<float> left_estension = new List<float> ();
	private List<float> right_estension = new List<float> ();

	//save the past pitch radial deviation
	private List<float> left_radial = new List<float> ();
	private List<float> right_radial = new List<float> ();

	//save the past pitch ulnar deviation
	private List<float> left_ulnar = new List<float> ();
	private List<float> right_ulnar = new List<float> ();

	private string csv = "";
	//"estension,flexion,radial,ulnar,timer\n";

	private float levelTimer;

	// Use this for initialization
	void Start ()
	{
		directoryPath = Path.Combine (Application.persistentDataPath, 
			Path.Combine ("Patients", Path.Combine (patientName, gameType.ToString ())));

		hand_data_file_path = Path.Combine (directoryPath, filename);

		ReplayFromFile ();

		levelTimer = 0;
	}



	// Update is called once per frame
	void Update ()
	{
		
		if (!saved_data && loaded_data) {
			if (hc.GetLeapRecorder ().GetProgress () >= 1) {
				Debug.Log ("Registrazione finita");
				//SaveData ();
				SaveTXT ();
			} else if (hc.GetFrame ().Hands.Count >= 1) {

				if (roll) {

					csv = csv + hc.GetFrame ().Hands.Leftmost.PalmNormal.Roll.ToString ();
					levelTimer += Time.deltaTime;
					csv = csv + "," + levelTimer.ToString ();
					csv = csv + "\n";
					
				} else {


					if (hc.GetFrame ().Hands.Leftmost.Direction.Pitch > 0) {

						//left_estension.Add (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);


						//right_estension.Add (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);


						csv = csv +
						hc.GetFrame ().Hands.Leftmost.Direction.Pitch.ToString () + "," +
						"" + ",";

					} else {

						//left_flexion.Add (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);


						//right_flexion.Add (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
						csv = csv +
						"" + "," +
						hc.GetFrame ().Hands.Leftmost.Direction.Pitch.ToString () + ",";

					}

					if (hc.GetFrame ().Hands.Leftmost.IsLeft) {

						if (hc.GetFrame ().Hands.Leftmost.Direction.Yaw > 0) {
							//left_radial.Add (hc.GetFrame ().Hands.Leftmost.Direction.Yaw);

							csv = csv +
							hc.GetFrame ().Hands.Leftmost.Direction.Yaw.ToString () + "," +
							"" + "";

						} else {

							//left_ulnar.Add (hc.GetFrame ().Hands.Leftmost.Direction.Yaw);
							csv = csv +
							"" + "," +
							hc.GetFrame ().Hands.Leftmost.Direction.Yaw.ToString () + "";



						} 
						/*if (hc.GetFrame ().Hands.Rightmost.PalmNormal.Roll > 0) {
					left_estension.Add (hc.GetFrame ().Hands.Rightmost.PalmNormal.Roll);
				} else {
					left_flexion.Add (hc.GetFrame ().Hands.Rightmost.PalmNormal.Roll);
				} */
					} else {

						if (hc.GetFrame ().Hands.Leftmost.Direction.Yaw > 0) {
							//left_ulnar.Add (hc.GetFrame ().Hands.Leftmost.Direction.Yaw);
							csv = csv +
							"" + "," +
							hc.GetFrame ().Hands.Leftmost.Direction.Yaw.ToString () + "";
						} else {
							//left_radial.Add (hc.GetFrame ().Hands.Leftmost.Direction.Yaw);

							csv = csv +
							hc.GetFrame ().Hands.Leftmost.Direction.Yaw.ToString () + "," +
							"" + "";
						}

					}

					levelTimer += Time.deltaTime;
					csv = csv + "," + levelTimer.ToString ();
					csv = csv + "\n";

				}
			}
		}





	}




	//function used to load a replay inside the Hand Controller
	public void ReplayFromFile ()
	{

		Debug.Log (hand_data_file_path.ToString ());

		string frameString = File.ReadAllText (hand_data_file_path);

		FrameSequence frame_sequence = JsonUtility.FromJson<FrameSequence> (frameString);


		//before save new frames the recording must be clear
		hc.ResetRecording ();

		foreach (Frame frame in frame_sequence.GetFrames ()) {

			hc.GetLeapRecorder ().AddFrame (frame);
		}
		loaded_data = true;

		hc.PlayRecording ();
	}



	void SaveData ()
	{
		saved_data = true;
		ExtractorData s = new ExtractorData ();

		DateTime gameDate = DateTime.UtcNow;
		s.patientName = "Lina";
		s.timestamp = gameDate.ToFileTimeUtc ();

		s.extension = left_estension;
		s.flexion = left_flexion;
		s.radial = left_radial;
		s.ulnar = left_ulnar;

		//s.pitch_left_max = left_estension.Average ();
		//s.pitch_left_min = left_flexion.Average ();
		//s.pitch_right_max = right_estension.Average ();
		//s.pitch_right_min = right_flexion.Average ();

		//		s.yaw_left_max = left_radial.Average ();
		//s.yaw_left_min = left_ulnar.Average ();
		//s.yaw_right_max = right_ulnar.Average ();
		//s.yaw_right_min = right_radial.Average ();

		string directoryPath = Path.Combine (Application.persistentDataPath,
			                       Path.Combine ("Dati Tesi", s.patientName));

		Directory.CreateDirectory (directoryPath);

		string filePath = Path.Combine (
			                  directoryPath,
			                  s.patientName + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + "_all_data" + ".json");

		string jsonString = JsonUtility.ToJson (s);
		File.WriteAllText (filePath, jsonString);
		Debug.Log ("saved data");


	}



	void SaveTXT ()
	{
		saved_data = true;
		DateTime gameDate = DateTime.UtcNow;


		string directoryPath = Path.Combine (Application.persistentDataPath,
			                       Path.Combine ("Dati Paper", patientName));

		Directory.CreateDirectory (directoryPath);

		string filePath = Path.Combine (
			                  directoryPath,
			                  filename.Split ('.') [0] + ".txt");

		File.WriteAllText (filePath, csv);
		Debug.Log ("saved txt");

		
	}

}
