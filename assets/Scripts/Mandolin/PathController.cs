using UnityEngine;
using System.Collections;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

public class PathController : MonoBehaviour {
	
	string filePath;
	float[] seconds;
	List<string> fileNames = new List<string>();
	string buttonPrefab = "Prefabs/Mandolin/Tasto";
	string audioResourcesFolder = "Audio/Mandolin tracks/";
	float leftTime, rightTime;
	int time = 3;
	bool pause, track, checkLength, gotTunings, replaySelected;
	bool firstMenu = true;
	float clipTime, startTime, pauseTime;

	//Player tunings
	float minLeftVertical, maxLeftVertical;
	float minRightVertical, maxRightVertical;

	public GameObject score, timer, starBar, leftCircle, rightCircle, saveManager;

	// Update is called once per frame
	void Update () {
		if(PlayerSaveData.playerData.GetPlayer() != null && !gotTunings  & !SaveInfos.replay){
			GetTunings (PlayerSaveData.playerData.GetUserName());
			GetComponent<LeftJumpGesture> ().SetTunings (minLeftVertical, maxLeftVertical);
			GetComponent<RightJumpGesture> ().SetTunings (minRightVertical, maxRightVertical);
			gotTunings = true;
		}
		else if(SaveInfos.replay && PlayerSaveData.playerData.GetPlayerTunings().Count > 0 && !gotTunings){
			GetTunings (PlayerSaveData.playerData.GetUserName());
			GetComponent<LeftJumpGesture> ().SetTunings (minLeftVertical, maxLeftVertical);
			GetComponent<RightJumpGesture> ().SetTunings (minRightVertical, maxRightVertical);
			gotTunings = true;
		}

		if(timer != null)
			timer.GetComponent<TextMesh>().text = time.ToString();
		if(MusicSaveData.musicData.GetLeftGesture()){
			if(Time.time > leftTime+0.1f)
				MusicSaveData.musicData.SetLeftGesture(false);
		}
		if(MusicSaveData.musicData.GetRightGesture()){
			if(Time.time > rightTime+0.1f)
				MusicSaveData.musicData.SetRightGesture(false);
		}

		if((Time.time - startTime) > clipTime && !pause && checkLength){
			CancelInvoke();
			checkLength = false;
			MusicSaveData.musicData.AddHighscore(PlayerSaveData.playerData.GetCurrentPathName(),PlayerSaveData.playerData.GetUserName(), PlayerSaveData.playerData.GetScore());
			GeneralSaveData.generalData.UpdatePlayerMusicHighscore(PlayerSaveData.playerData.GetCurrentPathName(),PlayerSaveData.playerData.GetUserName(), PlayerSaveData.playerData.GetScore());
			SendMessage ("EndMenu");
		}

		score.GetComponent<TextMesh>().text = "PUNTEGGIO\n" + PlayerSaveData.playerData.GetScore();
		if(time == 0){
			CancelInvoke();
			timer.SetActive(false);
			MusicSaveData.musicData.SetStart(true);
			//Camera.main.audio.Stop();
			startTime = Time.time;
			Camera.main.audio.Play();
			checkLength = true;
			time = 3;
			if(!track)
				track = true;
		}

		if(SaveInfos.replay &&  (Time.time - startTime) >= PlayerSaveData.playerData.GetGameTime() && replaySelected){
			SendMessage("EndMenu");
			replaySelected = false;
		}
	}
	
	void FixedUpdate(){
		if(track){
			InvokeRepeating("SaveData", 0f, 0.1f);
			track = false;
		}
	}

	void StartReplay (){
		timer.SetActive(false);
		MusicSaveData.musicData.SetStart(true);
		//Camera.main.audio.Stop();
		startTime = Time.time;
		Camera.main.audio.Play();
		replaySelected = true;
		checkLength = true;
		track = false;
	}

	void CreatePath(string song){
		filePath = MusicSaveData.musicData.GetTxtLocationPath() + song + ".txt";
		string[] lines = System.IO.File.ReadAllLines(@filePath);
		seconds = new float[lines.Length];
		for(int i = 0; i < lines.Length; i++){
			seconds[i] = float.Parse(lines[i], CultureInfo.InvariantCulture.NumberFormat);
		}
		Camera.main.audio.clip = (AudioClip)Resources.Load (audioResourcesFolder + song);
		clipTime = Camera.main.audio.clip.length;
		CreateButtons (seconds);
		timer.GetComponent<TextMesh>().text = time.ToString();
		PlayerSaveData.playerData.SetScore(0);
		PlayerSaveData.playerData.SetTotObstacles (lines.Length);

		timer.SetActive (true);
		firstMenu = false;
		if(!SaveInfos.replay){
			SaveInfos.ResetData ();
			InvokeRepeating ("UpdateTimer",1,1);
		}
		else{
			StartReplay();
		}
	}

	void CreateButtons(float[] sec){
		GameObject[] buttons = GameObject.FindGameObjectsWithTag ("Button");
		if(buttons.Length > 0){
			foreach(GameObject b in buttons){
				Destroy(b);
			}
		}
		for(int i = 0; i < sec.Length; i += 2){
			float yv = MusicSaveData.musicData.GetCenterY() + MusicSaveData.musicData.GetButtonSpeed()*sec[i]; 
			Vector3 temp = new Vector3(MusicSaveData.musicData.GetRightGuideX(), yv, MusicSaveData.musicData.GetButtonZ());
			GameObject go = (GameObject)Instantiate(Resources.Load(buttonPrefab), temp, Quaternion.identity);
			if(i < 10)
				go.name = "Button0" + i;
			else
				go.name = "Button" + i;
		}
		for(int i = 1; i < sec.Length; i += 2){
			float yv = MusicSaveData.musicData.GetCenterY() + MusicSaveData.musicData.GetButtonSpeed()*sec[i]; 
			Vector3 temp = new Vector3(MusicSaveData.musicData.GetLeftGuideX(), yv, MusicSaveData.musicData.GetButtonZ());
			GameObject go = (GameObject)Instantiate(Resources.Load(buttonPrefab), temp, Quaternion.identity);
			if(i < 10)
				go.name = "Button0" + i;
			else
				go.name = "Button" + i;
		}
	}

	//hand è false se la mano è sinistra, true se è destra
	public void GoDown(bool hand){
		if(!hand){
			MusicSaveData.musicData.SetLeftGesture(true);
			leftTime = Time.time;
		}
		if(hand){
			MusicSaveData.musicData.SetRightGesture(true);
			rightTime = Time.time;
		}
	}

	public void JumpUp(bool hand){
	}

	public void UpdateTimer(){
		time--;
	}

	public void Pause(){
		pauseTime = Time.time;
		pause = true;
		Camera.main.audio.Pause ();
		MusicSaveData.musicData.SetStart (false);

	}

	public void UnPause(){
		clipTime += Time.time - pauseTime;
		pause = false;
		timer.SetActive (true);
		InvokeRepeating ("UpdateTimer",1,1);
	}

	public void SaveToXML(){
		PlayerSaveData.playerData.SetGameTime (Time.time - startTime);
		saveManager.SendMessage ("SaveHandsToFile");
	}
	
	void SaveData(){
		saveManager.SendMessage ("SaveData");
	}

	public void GreenLeftCircle(){
		leftCircle.renderer.material.color = Color.green;
	}

	public void GreenRightCircle(){
		rightCircle.renderer.material.color = Color.green;
	}

	public void LeftBackToRed(){
		leftCircle.renderer.material.color = Color.red;
	}

	public void RightBackToRed(){
		rightCircle.renderer.material.color = Color.red;
	}

	void GetTunings(string pl){
		Hashtable tunings = PlayerSaveData.playerData.GetPlayerTunings();
		minLeftVertical = (float)tunings ["Min Left Vertical"];
		maxLeftVertical = (float)tunings ["Max Left Vertical"];
		minRightVertical = (float)tunings ["Min Right Vertical"];
		maxRightVertical = (float)tunings ["Max Right Vertical"];
	}
}
