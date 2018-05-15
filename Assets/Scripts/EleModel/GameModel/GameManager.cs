using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using POLIMIGameCollective;
using System;
using System.IO;
using Leap;

public class GameManager : Singleton<GameManager>
{

	public GameObject m_background;

	public GameObject m_wait_background;
	public Text m_wait_text;

	[Header ("Loading time for Level")]
	[Range (0f, 4f)]
	public float m_loading_time = 0.5f;

	public GameObject player;

	public Vector3 player_initial_pos;


	public HandController hc;


	public GameMenuScript menu_GUI;


	//name of the path chosen
	public string current_path = "";

	public bool car, music, shooting;


	//gameobject array containing the diamonds whose position indicates the level path
	private GameObject[] m_path;




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
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void BaseStart (string music_title)
	{
		ClearScreens ();

		//music starts
		MusicManager.Instance.PlayMusic (music_title);

		//cleans from all sound effects
		SfxManager.Instance.Stop ();


		m_background.SetActive (true);
		//reset car position and deactivates car gameObj 

		//the scene begins with the game main menu
		menu_GUI.menu = true;


	}


	public void BaseUpdate ()
	{
		if (Input.GetKeyDown ("escape") && is_playing) {
			BasePauseLevel ();
		}

	}


	void ClearScreens ()
	{
		if (m_background != null)
			m_background.SetActive (false);

		if (m_wait_background != null)
			m_wait_background.SetActive (false);
	}




	public void BaseChooseLevel (string name)
	{
		SfxManager.Instance.Unmute ();
		current_path = name;
		StartCoroutine (LoadLevel (name));
	}

	IEnumerator LoadLevel (string name)
	{
		

		yield return new WaitForSeconds (m_loading_time);

		is_playing = true;

		Debug.Log ("inside LoadLevel");

		//deactivate BG 
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

		//deactivate wait screen
		m_wait_background.SetActive (false);


		hc.Record ();


	}



	/* these function are in common between all the games
	 */

	public int BaseGetScore ()
	{
		return score;
	}

	public void BaseAddPoints ()
	{
		score = score + 10;

	}


	public bool Get_Is_Playing ()
	{
		return is_playing;
	}


	/* end common functions */
		





	//called when the player pauses the game
	void BasePauseLevel ()
	{

		is_playing = false;

		SfxManager.Instance.Mute ();

		player.SetActive (false);

		menu_GUI.pause = true;

	}

	//triggered by the button "continue" in the pause screen
	public void BaseResumeLevel ()
	{
		SfxManager.Instance.Unmute ();

		is_playing = true;

		player.SetActive (true);

	}



	//called when the player reaches the end of the level
	public void BaseWinLevel ()
	{
		is_playing = false;


		StartCoroutine (WinCoroutine ());
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

	// TODO this myst be called also by restart level function
	void EndLevel ()
	{
		is_playing = false;

		SfxManager.Instance.Unmute ();
		SfxManager.Instance.Stop ();

		MusicManager.Instance.UnmuteAll ();

		if (m_path != null) {
			for (int i = 0; i < m_path.Length; i++) {
				if (m_path [i] != null) {
					Destroy (m_path [i]);
				}
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


		if (car) {
			m.gameType = GameMatch.GameType.Car;
		} else if (music) {
			m.gameType = GameMatch.GameType.Music;
		} else if (shooting) {
			m.gameType = GameMatch.GameType.Shooting;
		}

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

		//now we stop the leap recorder
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


	//TODO this doesn't go here
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
		BaseChooseLevel ("Na");
		hc.PlayRecording ();
	}






}
