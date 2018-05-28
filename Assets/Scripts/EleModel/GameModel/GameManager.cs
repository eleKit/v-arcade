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


	public GameObject m_resume_screen;
	public Text m_resume_countdown;

	public GameObject m_score_canvas;
	public Text m_score_text;


	[Header ("Loading time for Level")]
	[Range (0f, 4f)]
	public float m_loading_time = 0.5f;

	public GameObject player;

	public Vector3 player_initial_pos;


	public HandController hc;



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




	void Awake ()
	{
		car = false;
		shooting = false;
		music = false;

	}

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void BaseStart (string music_title, GameMatch.GameType game_type)
	{
		switch (game_type) {

		case GameMatch.GameType.Car:
			car = true;
			break;
		case GameMatch.GameType.Shooting:
			shooting = true;
			break;
		case GameMatch.GameType.Music:
			music = true;
			break;
		}


		//music starts
		MusicManager.Instance.PlayMusic (music_title);

		GameMenuScript.Instance.LoadUIOfGame (game_type);

		//the scene begins with the game main menu
		BaseToMenu ();


	}

	public void BaseToMenu ()
	{
		ClearScreens ();

		SfxManager.Instance.Stop ();

		m_background.SetActive (true);


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

		if (m_score_canvas != null)
			m_score_canvas.SetActive (false);

		if (m_resume_screen != null)
			m_resume_screen.SetActive (false);
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

		//activate score screen
		m_score_canvas.SetActive (true);

		score = 0;

		m_score_text.text = "Punteggio: " + score.ToString ();


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
		m_score_text.text = "Punteggio: " + score.ToString ();

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

		GameMenuScript.Instance.LoadPauseScreen ();

	}

	//triggered by the button "continue" in the pause screen
	public void BaseResumeLevel ()
	{
		//the player controller works only when is_playing is true
		player.SetActive (true);

		StartCoroutine (Resume ());

		SfxManager.Instance.Unmute ();

		is_playing = true;



	}

	IEnumerator Resume ()
	{
		ClearScreens ();
		m_resume_screen.SetActive (true);

		for (int i = 3; i > 0; i--) {

			m_resume_countdown.text = i.ToString ();
			yield return new WaitForSeconds (0.5f);
		}

		ClearScreens ();
		m_score_canvas.SetActive (true);
	}



	//called when the player reaches the end of the level
	public void BaseWinLevel ()
	{
		is_playing = false;


		StartCoroutine (WinCoroutine ());
	}


	IEnumerator WinCoroutine ()
	{

		//menu_GUI.win = true;

		ClearScreens ();

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
