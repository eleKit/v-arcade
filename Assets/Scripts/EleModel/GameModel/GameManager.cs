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


	[Header ("Win buttons activated only when the file has been saved")]
	public Button win_UI_button_restart;
	public Button win_UI_button_menu;


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

	//used to check what type of game the kid is playing
	GameMatch.GameType current_game_type;
	//used to check what kind of gesture is recognized (only in case of games with multiple gestures)
	GameMatch.HandAngle current_hand_angle = GameMatch.HandAngle.None;
	//used to check if the level is standard or training
	GameMatch.LevelType current_level_type = GameMatch.LevelType.Training;


	//bool to deactivate player if the game is paused
	private bool is_playing = false;


	//score of the game
	private int score;




	/* Replay:
	 * instead of using the LeapRecorder Play() it is used the NextFrame() inside the  GetCurrentFrame () function [see below]
	 *
	 * leap_start_time and game_start_time
	 * are used in the GetCurrentFrame () function
	 * to calculate the correct instant t when the next frame of the LeapRecorder must be loaded
	 * in order to obtain a replay perfectly synchronized with the original match
	 *
	 * playback_index
	 * is the index of the replay_frames list, it is used to load the next frame
	 *
	 * GetCurrentFrame () function is called by the Gesture Recognizer scripts instead of hc.GetFrame()
	 * and the behaviuor inside that function is different in case of Replay or Match
	 */

	private string hand_data_file_path;

	private bool loaded_file = false;

	private float leap_start_time;
	private float game_start_time;
	private List<Frame> replay_frames;
	private int playback_index;


	private float pause_time;





	// Use this for initialization
	void Start ()
	{
		if (!replay) {
			//set the win buttons not interactable
			SetButtonsInteractable (false);
		}

		//This bool is very important, without setting this the recording is lost
		hc.enableRecordPlayback = true;
		hc.recorderLoop = false;
		hc.recorderSpeed = 1f;
		//TODO check what of these is fundamental
		hc.handMovementScale = new Vector3 (1f, 1f, 1f);



	}

	// Update is called once per frame
	void Update ()
	{

	}

	/* This function is called in the Update of each Gesture Recognizer script
	 *
	 * https://github.com/richjoyce/LeapRecorder/blob/6c6d988f5dc99463360c4d9c660ac439194b7356/LeapRecorder.cpp#L116
	 *
	 */

	public Frame GetCurrentFrame ()
	{
		/* this is the behaviour in case this is a Replay scene
			 * (i.e. replay == true)
			 * and in case the Hand Data have been already loaded inside the LeapRecorder
			 * (i.e loaded_file == true)
			 *
			 *
			 * leap_start_time, game_start_time, playback_index
			 * are setup in the LoadLevel() Coroutine
			 *
			 */
		if (loaded_file && replay) {

			//last and next frames setup
			Frame last = replay_frames [playback_index];
			Frame next = last;

			float leap_time = next.Timestamp / 1e6f - leap_start_time;

			/*
			 * equalize the leap_start_time and the leap_time
			 */
			if (leap_start_time == 0 && leap_time != 0) {
				leap_start_time = leap_time;
			} else if (leap_start_time != 0 && leap_time == 0) {
				leap_start_time = 0;
			}

			/*
			 * setup the game_time as the current time - the time when the UI has been loaded
			 * in order to not consider the time of navigating the UI before loading the level
			 *
			 */
			float game_time = Time.time - game_start_time;

			/*
			 * If there is a "jump in time" from the current frame TS and the next frame TS (the replay_frames [playback_index + 1])
			 * it means that in the Match the player has paused and resumed the game
			 *
			 * In the Replay scene this gap is filled adding the time difference in the leap_start_time
			 * updating the leap_time, and setting the next and last frames as the replay_frames [playback_index + 1]
			 * like in the last and next setup
			 *
			 */
			if (playback_index + 1 < replay_frames.Count && replay_frames [playback_index + 1].Timestamp - replay_frames [playback_index].Timestamp > 1e6f) {
				Debug.Log ("Skipping :" + ((replay_frames [playback_index + 1].Timestamp - replay_frames [playback_index].Timestamp) / 1e6f).ToString ("F2"));
				leap_start_time += (replay_frames [playback_index + 1].Timestamp - replay_frames [playback_index].Timestamp) / 1e6f;
				next = last = replay_frames [playback_index + 1];
				leap_time = next.Timestamp / 1e6f - leap_start_time;
			}


			while (game_time > leap_time) {
				//if there are other frames in the list
				if (playback_index + 1 >= replay_frames.Count) {
					return hc.GetFrame ();
				}

				//update play_back index
				playback_index++;

				//load the next frame in the LeapRecorder
				hc.GetLeapRecorder ().NextFrame ();

				//update last and next frames
				last = next;
				next = replay_frames [playback_index];

				//update leap_time as next frame TS - leap_start_time
				leap_time = next.Timestamp / 1e6f - leap_start_time;
			}

			/*Debug.Log (
				"Game time: " + game_time.ToString ("F2")
				+ " - Leap time: " + leap_time.ToString ("F2")
				+ " - Delta: " + (leap_time - game_time).ToString ("F2")
			);*/

			//return the last frame to the Gesture Recognizer script
			return last;
		} else {
			/*
			 * If this is a Match (i.e. replay == false)
			 * or the hand data have not been already loaded (i.e.loaded_file == false)
			 *
			 */
			leap_start_time = 0;
			// return the current frame of LeapController
			return hc.GetFrame ();
		}
	}


	//call this by UI Play Mode buttons to set the angle of the game
	public void SetGameMathcHandAngle (GameMatch.HandAngle hand_angle)
	{
		current_hand_angle = hand_angle;
	}

	//call this by UI Level Type buttons to set the level type of the game
	public void SetGameMathcHandAngle (GameMatch.LevelType level_type)
	{
		//set the level type
		current_level_type = level_type;
	}


	public void BaseStart (string music_title, GameMatch.GameType game_type)
	{
		//set the gametype
		current_game_type = game_type;
		//N.B the angle type and the level type are set apart only by games which have more than one gesture recognizer script


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

	void SetButtonsInteractable (bool b)
	{
		win_UI_button_menu.interactable = b;
		win_UI_button_restart.interactable = b;
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
	public void BaseChooseLevel (ReplayNamesOfPaths replay_path, string name)
	{


		SfxManager.Instance.Unmute ();
		SfxManager.Instance.Stop ();

		//save the recording path used to load the replay inside the hand controller
		hand_data_file_path = replay_path.hand_data_path;

		current_path = name;

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
			//hc.PlayRecording ();
			//hc.PauseRecording ();
			hc.GetLeapRecorder ().Pause ();
			hc.GetLeapRecorder ().SetIndex (0);

			leap_start_time = 0;
			game_start_time = Time.time;
			playback_index = 0;

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

		if (!replay) {
			//pause the recording(playback) when the game is in pause
			hc.PauseRecording ();
		}

		is_playing = false;

		SfxManager.Instance.Mute ();

		if (current_game_type.Equals (GameMatch.GameType.Music)) {
			//pause the current path level music
			MusicManager.Instance.PauseAll ();
		}

		player.SetActive (false);

		if (replay) {
			ReplayManagerUI.Instance.LoadPauseScreen ();
			pause_time = Time.time;
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
		if (!replay) {
			//resume the recording
			hc.Record ();
		} else {
			//hc.PlayRecording ();
			float pause_duration = Time.time - pause_time;
			game_start_time += pause_duration;
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

		ClearScreens ();


		m_background.SetActive (true);

		player.SetActive (false);

		if (!replay) {
			//save the current Match and the Hand Data of that match only if this is not a recording (or i'm debugging)

			yield return StartCoroutine (SaveData ());
			SetButtonsInteractable (true);

		} else if (replay) {
			hc.ResetRecording ();
		}

		is_playing = false;

		yield return new WaitForSeconds (3.5f);
		SfxManager.Instance.Stop ();


	}



	IEnumerator SaveData ()
	{

		GameMatch m = new GameMatch ();

		DateTime gameDate = DateTime.UtcNow;

		m.timestamp = gameDate.ToFileTimeUtc ();
		m.patientName = GlobalPlayerData.globalPlayerData.player;
		m.id_path = current_path;


		//save the game type
		m.gameType = current_game_type;

		//save the hand orientation
		m.handAngle = current_hand_angle;

		//save the level type
		m.levelType = current_level_type;

		//set the thresholds used in the match
		m.left_pitch_scale = GlobalPlayerData.globalPlayerData.player_data.left_pitch_scale;
		m.right_pitch_scale = GlobalPlayerData.globalPlayerData.player_data.right_pitch_scale;
		m.left_yaw_scale = GlobalPlayerData.globalPlayerData.player_data.left_yaw_scale;
		m.right_yaw_scale = GlobalPlayerData.globalPlayerData.player_data.right_yaw_scale;


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
		yield return StartCoroutine (SaveMatchDataCoroutine (filePath, m, gameDate, jsonString));


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
		yield return StartCoroutine (SaveHandDataCoroutine (framesPath, m, gameDate, frameString));


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

		replay_frames = hc.GetLeapRecorder ().GetFrames ();

		loaded_file = true;
	}






}
