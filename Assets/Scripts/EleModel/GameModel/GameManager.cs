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
	[Header ("Check this bool if this is a replay scene")]
	public bool replay;

	/* in case of a REPLAY SCENE: 
	 * 
	 * all the calls to the GameMenuScript must be substitued with calls to the ReplayManagerUI
	 * 
	 * all the HandController recording instructions must be substitued with PlayRecording instructions
	 * 
	 * the SaveData () method MUST NOT be called (system crashes trying to save a recoding thas is played) 
	 * 
	 * 
	 * when a level is loaded (reloaded) the hc.ResetRecording must be called
	 */



	[Header ("Use for debug, if checked the match is not saved")]
	public bool no_save;
	[Header ("Use for debug, if checked the match is saved into test server")]
	public bool debugging_save;


	//UI GameObjects
	public GameObject m_background;

	public GameObject m_wait_background;
	public Text m_wait_text;


	public GameObject m_resume_screen;
	public Text m_resume_countdown;

	public GameObject m_score_canvas;
	public Text m_score_text;


	//UI loading time attribute
	[Header ("Loading time for a Level")]
	[Range (0f, 4f)]
	public float m_loading_time = 0.5f;


	//Game managing attributes

	public GameObject player;

	public Vector3 player_initial_pos;

	public HandController hc;



	//name of the path chosen
	public string current_path = "";

	//bool used to check what type of game the kid is playing
	GameMatch.GameType current_game_type;

	GameMatch.HandAngle current_hand_angle = GameMatch.HandAngle.None;


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
		//This bool is very important, without setting this the recording is lost
		hc.enableRecordPlayback = true;
		hc.recorderLoop = false;
		hc.recorderSpeed = 1f;
		hc.handMovementScale = new Vector3 (1f, 1f, 1f);
		if (replay)
			hc.GetLeapRecorder ().speed = 1.0f;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
		

	//call this by UI Play Mode buttons to set the angle of the game
	public void SetGameMathcHandAngle (GameMatch.HandAngle hand_angle)
	{
		current_hand_angle = hand_angle;
	}


	public void BaseStart (string music_title, GameMatch.GameType game_type)
	{
		//set the gametype, method called by the NameGameManager class
		current_game_type = game_type;


		//music starts
		MusicManager.Instance.PlayMusic (music_title);


		if (replay) {
			ReplayManagerUI.Instance.LoadReplayUI ();
		} else {
			GameMenuScript.Instance.LoadUIOfGame (game_type);
		}

		//the scene begins with the game main menu
		BaseToMenu ();


	}

	//called when the game menu is loaded
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



	//method used in case of a match scene
	public void BaseChooseLevel (string name)
	{
		SfxManager.Instance.Unmute ();
		SfxManager.Instance.Stop ();

		//the path the kid is playing is saved here
		current_path = name;

		StartCoroutine (LoadLevel ());
	}


	//@Overload of BaseChooseLevel, method used in case of a replay scene
	public void BaseChooseLevel (ReplayNamesOfPaths replay_path)
	{
		

		SfxManager.Instance.Unmute ();
		SfxManager.Instance.Stop ();

		//save the recording path used to load the replay inside the hand controller
		hand_data_file_path = replay_path.hand_data_path;

		StartCoroutine (LoadLevel ());
	}


	IEnumerator LoadLevel ()
	{
		if (current_game_type.Equals (GameMatch.GameType.Music)) {
			//now stops the music menuu
			MusicManager.Instance.StopAll ();
		}

		yield return new WaitForSeconds (m_loading_time);



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


		if (current_game_type.Equals (GameMatch.GameType.Music)) {
			//now plays the music correspondent to the level chosen
			MusicManager.Instance.PlayMusic (current_path);
		}

		is_playing = true;

		//this must be before setting the position
		player.SetActive (true);

		//deactivate wait screen
		m_wait_background.SetActive (false);

		//activate score screen
		m_score_canvas.SetActive (true);

		score = 0;

		m_score_text.text = "Punti: " + score.ToString ();

		if (replay) {

			/* the recording is loaded from the chosen file and then the playback starts
			 */
			ReplayFromFile ();
			hc.PlayRecording ();

		} else {

			/* reset the recorder before start playing:
			 * BaseChooseLevel() method is also called by the RestartLevel function 
			* in the NameGameManager
			 */
			hc.ResetRecording ();

			// the recorder starts to save all the Frames
			hc.Record ();
		}


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
		m_score_text.text = "Punti: " + score.ToString ();

	}


	public bool Get_Is_Playing ()
	{
		return is_playing;
	}

	public GameMatch.GameType GetCurrentGameType ()
	{
		return current_game_type;
	}


	/* end common functions */
		





	//called when the player pauses the game
	void BasePauseLevel ()
	{
		/* here i'm using the SAME METHOD FOR BOTH REPLAY AND RECORD
		 * this happens because:
		 * 
		 * PauseRecording()
		 * Summary
		 * Stops playback or recording without resetting the frame counter
		 */
	
		//pause the recording(playback) when the game is in pause
		hc.PauseRecording ();

		is_playing = false;

		SfxManager.Instance.Mute ();

		if (current_game_type.Equals (GameMatch.GameType.Music)) {
			//pause the current path level music
			MusicManager.Instance.PauseAll ();
		}

		player.SetActive (false);

		if (replay) {
			ReplayManagerUI.Instance.LoadPauseScreen ();
		} else {
			GameMenuScript.Instance.LoadPauseScreen ();
		}

	}

	//triggered by the button "continue" in the pause screen
	public void BaseResumeLevel ()
	{
		//the player controller works only when is_playing is true
		player.SetActive (true);

		StartCoroutine (Resume ());

		SfxManager.Instance.Unmute ();

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


		//resume the recording|playback when the game is resumed by the player
		if (replay) {
			//resume the playback
			hc.PlayRecording ();
		} else {
			//resume the recording
			hc.Record ();
		}



		if (current_game_type.Equals (GameMatch.GameType.Music)) {
			//unpause the current path level music
			MusicManager.Instance.UnPauseAll ();
		}
		is_playing = true;
	
	}



	//called when the player reaches the end of the level
	public void BaseWinLevel ()
	{
		is_playing = false;

		SfxManager.Instance.Play ("win_jingle");

		if (replay) {
			ReplayManagerUI.Instance.LoadEndScreen ();
		} else {
			GameMenuScript.Instance.LoadWinScreen (score);
		}

		StartCoroutine (WinCoroutine ());
	}


	IEnumerator WinCoroutine ()
	{

		//menu_GUI.win = true;

		ClearScreens ();

		yield return new WaitForSeconds (0.5f);


		m_background.SetActive (true);

		player.SetActive (false);

		if (!replay) {
			//save the current Match and the Hand Data of that match only if this is not a recording (or i'm debugging)
			if (!no_save) {
				SaveData ();
			}
		}
			

		//every time the game s
		if (replay) {
			hc.ResetRecording ();
		}
			
		is_playing = false;

		yield return new WaitForSeconds (3.5f);
		SfxManager.Instance.Stop ();


	}



	void SaveData ()
	{ 

		GameMatch m = new GameMatch ();

		DateTime gameDate = DateTime.UtcNow;

		m.timestamp = gameDate.ToFileTimeUtc ();
		m.patientName = GlobalPlayerData.globalPlayerData.player;
		m.id_path = current_path;



		m.gameType = current_game_type;
		m.handAngle = current_hand_angle;


		//the game data are saved in the  Patients folder > PatientName foldet > GameType folder
		string directoryPath = Path.Combine (Application.persistentDataPath,
			                       Path.Combine ("Patients", Path.Combine (m.patientName, m.gameType.ToString ())));

		Directory.CreateDirectory (directoryPath);
		string filePath = Path.Combine (
			                  directoryPath,
			                  m.gameType.ToString () + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json"
		                  );

		string jsonString = JsonUtility.ToJson (m);
		File.WriteAllText (filePath, jsonString);


		//save match data on web
		StartCoroutine (SaveMatchDataCoroutine (filePath, m, gameDate, jsonString));


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

		//save hand data on web
		StartCoroutine (SaveHandDataCoroutine (framesPath, m, gameDate, frameString));


	}

	/* string filePath : the path of the original file
	 * GameMatch m : the m GameMatch object saved on file
	 * DateTime gameDate : the TS when the match has been played
	 * string matchString : the json string saved on the original file
	 */
	IEnumerator SaveMatchDataCoroutine (string filePath, GameMatch m, DateTime gameDate, string matchString)
	{
		string address; 
		string webfilename = "patient_" + m.patientName + "_" + m.gameType.ToString () + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + ".json";


		if (debugging_save) {
			address = "http://127.0.0.1/ES2.php?webfilename=";
			Debug.Log ("Debugging save");
		} else {
			address = "http://data.polimigamecollective.org/demarchi/ES2.php?webfilename=";
		}
		string myURL = address + webfilename;
		// Upload the entire local file to the server.
		ES2Web web = new ES2Web (myURL);

		yield return StartCoroutine (web.UploadFile (filePath));

		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
			string directoryPath = Path.Combine (Application.persistentDataPath, "TMP_web_saving");
			Directory.CreateDirectory (directoryPath);
			string path = Path.Combine (directoryPath, webfilename);
			File.WriteAllText (path, matchString);
		}
	}


	IEnumerator SaveHandDataCoroutine (string framesPath, GameMatch m, DateTime gameDate, string frameString)
	{
		string address; 
		string webfilename = "patient_" + m.patientName + "_" + m.gameType.ToString () + "_" + gameDate.ToString ("yyyyMMddTHHmmss") + "_hand_data.json";

		if (debugging_save) {
			address = "http://127.0.0.1/ES2.php?webfilename=";
			Debug.Log ("Debugging save");
		} else {
			address = "http://data.polimigamecollective.org/demarchi/ES2.php?webfilename=";
		}
		string myURL = address + webfilename;
		// Upload the entire local file to the server.
		ES2Web web = new ES2Web (myURL);


		yield return StartCoroutine (web.UploadFile (framesPath));

		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
			string directoryPath = Path.Combine (Application.persistentDataPath, "TMP_web_saving");
			Directory.CreateDirectory (directoryPath);
			string path = Path.Combine (directoryPath, webfilename);
			File.WriteAllText (path, frameString);
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


		firstTime = false;
	}






}
