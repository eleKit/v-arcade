using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;

public class PlayerSaveData : MonoBehaviour {

	public static PlayerSaveData playerData;
	
	PlayerInfos player;
	
	static float midSpeed = 30f;
	//Distanza in millimetri
	static float handDistance = 200f;
	
	float planeSpeed = 30f;
	int seed;
	int totObstacles;
	int skiDifficulty = 1;
	bool oneHandMode = true;
	bool openHandMode = true;
	bool rightHandSelected = true;
	bool firstTimePlaying, tuningTooOld;
	bool music, flight, ski;
	bool randomPath;
	float minXBonus, maxXBonus, minYBonus, maxYBonus;
	Vector3 startAngles;
	float gameTime;
	Hashtable replayTunings = new Hashtable ();

	string userName = "Rocco";
	string currentPathName = "";
	int score = 0;
	int highscore = 0;
	
	void Awake () {
		if(playerData == null){
			DontDestroyOnLoad(gameObject);
			playerData = this;
		}
		else if(playerData != this){
			Destroy (gameObject);
		}
	}

	void Update(){
		if(currentPathName != "" && flight){
			if(player.planeHighscores.Exists(x => x.pathName.Equals(currentPathName))){
			   highscore = player.planeHighscores.Find (x => x.pathName.Equals(currentPathName)).score;
			}
		}
		else if (currentPathName != "" && music){
			if(player.musicHighscores.Exists(x => x.pathName.Equals(currentPathName)))
				highscore = player.musicHighscores.Find (x => x.pathName.Equals(currentPathName)).score;
		}
		else if(ski){
			if(SkiSaveData.skiData.GetRandomFlagPath())
				highscore = SkiSaveData.skiData.GetPlayerFlagHighscore(userName);
			else if(SkiSaveData.skiData.GetRandomTreePath())
				highscore = SkiSaveData.skiData.GetPlayerTreeHighscore(userName);
			else if(currentPathName != ""){
				if(player.skiHighscores.Exists (x => x.pathName.Equals(currentPathName)))
					highscore = player.skiHighscores.Find (x => x.pathName.Equals(currentPathName)).score;
			}
		}

		if(player != null){
			userName = player.userName;
		}
	}

	public Hashtable GetPlayerTunings(){
		Hashtable tunings = new Hashtable ();
		if(!SaveInfos.replay){
			tunings.Add ("Min Left Vertical", player.tunings.minLeftVertical);
			tunings.Add ("Max Left Vertical", player.tunings.maxLeftVertical);
			tunings.Add ("Min Left Horizontal", player.tunings.minLeftHorizontal);
			tunings.Add ("Max Left Horizontal", player.tunings.maxLeftHorizontal);
			tunings.Add ("Min Right Vertical", player.tunings.minRightVertical);
			tunings.Add ("Max Right Vertical", player.tunings.maxRightVertical);
			tunings.Add ("Min Right Horizontal", player.tunings.minRightHorizontal);
			tunings.Add ("Max Right Horizontal", player.tunings.maxRightHorizontal);
			return tunings;
		}
		else
			return replayTunings;
	}

	public void SetReplayTunings(float minLV, float maxLV, float minLH, float maxLH, float minRV, float maxRV, float minRH, float maxRH){
		replayTunings = new Hashtable ();
		replayTunings.Add ("Min Left Vertical", minLV);
		replayTunings.Add ("Max Left Vertical", maxLV);
		replayTunings.Add ("Min Left Horizontal", minLH);
		replayTunings.Add ("Max Left Horizontal", maxLH);
		replayTunings.Add ("Min Right Vertical", minRV);
		replayTunings.Add ("Max Right Vertical", maxRV);
		replayTunings.Add ("Min Right Horizontal", minRH);
		replayTunings.Add ("Max Right Horizontal", maxRH);
	}
	

	public void MusicGame(){
		music = true;
		flight = false;
		ski = false;
	}

	public void FlightGame(){
		music = false;
		flight = true;
		ski = false;
	}

	public void SkiGame(){
		music = false;
		flight = false;
		ski = true;
	}

	public float GetPlaneSpeed(){
		return planeSpeed;
	}
	public void SetPlaneSpeed(float sp){
		if(sp == 1)
			planeSpeed = midSpeed * 0.5f;
		else if(sp == 3)
			planeSpeed = midSpeed * 1.5f;
		else
		planeSpeed = midSpeed;
	}

	public void SetOneHandMode(bool mod){
		oneHandMode = mod;
	}
	public void SetOpenHandMode(bool mod){
		openHandMode = mod;
	}
	public void SetRightHand(bool mod){
		rightHandSelected = mod;
	}

	public int GetSeed(){
		return seed;
	}

	public void SetSeed(int se){
		seed = se;
	}

	public int GetTotObstacles(){
		return totObstacles;
	}

	public void SetTotObstacles(int tot){
		totObstacles = tot;
	}

	public bool GetOneHandMode(){
		return oneHandMode;
	}
	public bool GetOpenHandMode(){
		return openHandMode;
	}
	public bool GetRightHand(){
		return rightHandSelected;
	}

	public string GetUserName(){
		return userName;
	}

	public int GetScore(){
		return score;
	}

	public void SetScore(int sc){
		score = sc;
	}

	public int GetHighscore(){
		return highscore;
	}

	public void SetCurrentPathName(string path){
		currentPathName = path;
	}
	public string GetCurrentPathName(){
		return currentPathName;
	}

	public void SetGameTime(float tim){
		gameTime = tim;
	}

	public float GetGameTime(){
		return gameTime;
	}

	public void SetRandomPath(bool x){
		randomPath = x;
	}

	public bool GetRandomPath(){
		return randomPath;
	}

	public void SetUserName(string un){
		userName = un;
	}

	public bool GetFirstTimePlaying(){
		return firstTimePlaying;
	}

	public void SetSkiDifficulty(int dif){
		if(dif < 1 || dif > 3)
			Debug.LogError("Difficoltà non nei valori prestabiliti");
		else
			skiDifficulty = dif;
	}

	public int GetSkiDifficulty(){
		return skiDifficulty;
	}

	public bool GetTuningTooOld(){
		return tuningTooOld;
	}

	public void SetPlayer(PlayerInfos pl){
		player = pl;
		firstTimePlaying = !pl.tunings.hasTunings;
		Debug.Log (DateTime.Now + "--" + pl.tunings.lastUpdate);
		tuningTooOld = (DateTime.Now - pl.tunings.lastUpdate).TotalDays > 7;
	}

	public void NewPlayer(string nam){
		player = new PlayerInfos (nam);
		GeneralSaveData.generalData.AddPlayer (player);
		firstTimePlaying = true;
	}

	public PlayerInfos GetPlayer(){
		return player;
	}

	public void SetBonus(float minX, float maxX, float minY, float maxY){
		minXBonus = minX;
		maxXBonus = maxX;
		minYBonus = minY;
		maxYBonus = maxY;
	}

	public float GetHandDistance(){
		return handDistance;
	}

	public Vector3 GetStartAngles(){
		return startAngles;
	}

	public void SetStartAngles(Vector3 ang){
		startAngles = ang;
	}
}

