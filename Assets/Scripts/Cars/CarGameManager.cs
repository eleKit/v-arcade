using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using POLIMIGameCollective;
using System;
using System.IO;
using Leap;

public class CarGameManager : Singleton<CarGameManager>
{

	[Header ("Cheat Flag")]
	public bool cheat;

	public GameObject m_background;

	public GameObject m_wait_background;
	public Text m_wait_text;

	[Header ("Loading time for Level")]
	[Range (0f, 4f)]
	public float m_loading_time = 0.5f;

	public GameObject player;

	public HandController hc;


	public GameMenuScript menu_GUI;

	//attribute used to set the winning point coordinates
	public GameObject winning_Point;


	private Vector3 player_initial_pos = new Vector3 (0f, -5.7f, 0f);

	//gameobject array containing the diamonds whose position indicates the level path
	private GameObject[] m_path;

	//name of the path chosen
	private string current_path = "";


	//bool to deactivate player if the game is paused
	private bool is_playing = false;


	//score of the game
	private int score;




	//TODO cancellare fa crashare Unity
	private string hand_data_file_path;

	private bool firstTime = true;




	// Use this for initialization
	void Start ()
	{
		ClearScreens ();
		m_background.SetActive (true);
		//reset car position and deactivates car gameObj 
		ResetPlayer ();

		//the scene begins with the game main menu
		menu_GUI.menu = true;

		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown ("escape") && is_playing) {
			PauseLevel ();
		}
		
	}

	//This method is used to set the point where the game ends
	public void SetWinningPosition (Vector3 coord)
	{
		winning_Point.transform.position = coord;
	}


	public void ResetPlayer ()
	{
		player.GetComponent<CarControllerScript> ().SetGravityToZero ();
		player.transform.position = player_initial_pos;
		player.SetActive (false);
	}


	//called when the user choses one level
	public void ChooseLevel (string n)
	{
		current_path = n;
		StartCoroutine (LoadLevel (name));

	}

	IEnumerator LoadLevel (string name)
	{

		yield return new WaitForSeconds (m_loading_time);
		//TODO generate the path

		is_playing = true;

		Debug.Log ("inside LoadLevel");

		//deactivate BG screen
		m_background.SetActive (false);

		m_wait_background.SetActive (true);
		m_wait_text.text = "caricamento.";

		yield return new WaitForSeconds (0.5f);


		m_wait_text.text = "caricamento..";

		yield return new WaitForSeconds (0.5f);

		m_wait_text.text = "caricamento...";

		yield return new WaitForSeconds (0.5f);


		//this must be before setting the position
		player.SetActive (true);


		m_wait_background.SetActive (false);


		//reset car position
		player.transform.position = player_initial_pos;


		Debug.Log ("end LoadLevel");

		if (firstTime) {
			hc.Record ();
		}




	}

	//called when the player reaches the end of the level
	public void WinLevel ()
	{
		is_playing = false;


		StartCoroutine ("WinCoroutine");
	}


	IEnumerator WinCoroutine ()
	{
		
		menu_GUI.win = true;

		yield return new WaitForSeconds (0.5f);

		m_background.SetActive (true);

		player.SetActive (false);

		SaveData ();

		EndLevel ();




	}

	// called to destroy the current level path
	// never called directly by the UI
	void EndLevel ()
	{
		is_playing = false;
	
		if (m_path != null) {
			for (int i = 0; i < m_path.Length; i++) {
				Destroy (m_path [i]);
			}
		}

	}


	void SaveData ()
	{ 

		GameMatch m = new GameMatch ();

		DateTime gameDate = DateTime.UtcNow;

		m.timestamp = gameDate.ToFileTimeUtc ();
		m.patientName = GlobalPlayerData.globalPlayerData.player;
		m.id_path = current_path;
		m.gameType = GameMatch.GameType.Car;

		string directoryPath = 
			Path.Combine (Application.persistentDataPath, m.patientName);

		Directory.CreateDirectory (directoryPath);
		string filePath = Path.Combine (
			                  directoryPath,
			                  m.gameType.ToString () + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json"
		                  );

		string jsonString = JsonUtility.ToJson (m);
		File.WriteAllText (filePath, jsonString);


		// Now we save motion data
		FrameSequence frame_sequence = new FrameSequence ();
		frame_sequence.timestamp = gameDate.ToFileTimeUtc ();
		frame_sequence.patientName = m.patientName;
		hc.StopRecording ();

		LeapRecorder recorder = hc.GetLeapRecorder ();

	
		foreach (Frame f in recorder.GetFrames ()) {
			frame_sequence.addFrame (f);
		}

		string framesPath = Path.Combine (
			                    directoryPath,
			                    m.gameType.ToString () + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + "_hand_data.json"
		                    );


		string frameString = JsonUtility.ToJson (frame_sequence);
		File.WriteAllText (framesPath, frameString);

		hand_data_file_path = framesPath;
		
	}


	public void ReplayWithouthPath ()
	{
		Debug.Log ("ReplayWithouthPath");
		ReplayFromFile (hand_data_file_path);
	}


	public void ReplayFromFile (string filePath)
	{

		Debug.Log (filePath.ToString ());

		string frameString = File.ReadAllText (filePath);

		FrameSequence frame_sequence = JsonUtility.FromJson<FrameSequence> (frameString);

		hc.ResetRecording ();

		foreach (Frame frame in frame_sequence.GetFrames ()) {

			hc.GetLeapRecorder ().AddFrame (frame);
		}


		firstTime = false;
		ChooseLevel ("Na");
		hc.PlayRecording ();




		
		
	}





	//called when the player pauses the game
	void PauseLevel ()
	{
		
		is_playing = false;

		player.SetActive (false);

		menu_GUI.pause = true;

	}


	//triggered by the button "continue" in the pause screen
	public void ResumeLevel ()
	{
		is_playing = true;
	
		player.SetActive (true);

	}

	public void RestartLevel ()
	{
		Debug.Log ("Load Level call");
		StartCoroutine (LoadLevel (current_path));



	}


	public int GetScore ()
	{
		return score;
	}

	public void AddPoints ()
	{
		score = score + 10;
		
	}


	void ClearScreens ()
	{
		if (m_background != null)
			m_background.SetActive (false);

		if (m_wait_background != null)
			m_wait_background.SetActive (false);
	}
}
