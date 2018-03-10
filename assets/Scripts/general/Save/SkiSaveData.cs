using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System;

public class SkiSaveData : MonoBehaviour {

	public static SkiSaveData skiData;

	float trackSpeed = 3.5f;
	int points = 20;
	bool start, random;
	int numberOfObstacles = 40;
	float obstacleInterval = 3;
	bool slideMode = true;
	bool slapMode;
	bool stepMode;
	bool smoothMode = true;
	bool randomFlagPath;
	bool randomTreePath;
	int maxFlagScore = 0;
	string bestFlagPlayer = "";
	int maxTreeScore = 0;
	string bestTreePlayer = "";
	SkiPathInfos currentPath;

	List<Highscore> skiFlagHighscores = new List<Highscore>();
	List<Highscore> skiTreeHighscores = new List<Highscore>();
	List<SkiPathInfos> skiPaths = new List<SkiPathInfos>();

	void Awake () {
		if(skiData == null){
			DontDestroyOnLoad(gameObject);
			skiData = this;
			LoadSkiFlagHighscores();
			LoadSkiTreeHighscores();
			LoadSkiPaths();
			GetBestSkiFlagHighscore();
			GetBestSkiTreeHighscore();
		}
		else if(skiData != this){
			Destroy (gameObject);
		}
	}

	public bool CheckPathName(string pName){
		return skiPaths.Exists (x => x.pathName.Equals (pName));
	}
	
	public void InitializePath(){
		currentPath = new SkiPathInfos ();
		Debug.Log ("Percorso Inizializzato");
	}
	
	public void SetCurrentPathName(string pName){
		currentPath.pathName = pName;
	}

	public void SetMovementType(bool step){
		currentPath.smoothMov = !step;
		currentPath.stepMov = step;
	}

	public void AddObstacle(string oName, Vector3 localPos){
		SkiObstacleInfos obstacle = new SkiObstacleInfos ();
		obstacle.obstacleName = oName;
		obstacle.xLocalPos = localPos.x;
		obstacle.yLocalPos = localPos.y;
		obstacle.zLocalPos = localPos.z;
		currentPath.obstacles.Add (obstacle);
	}

	public float GetObstacleInterval(){
		return obstacleInterval;
	}
	
	public void SetObstacleInterval(float ob){
		obstacleInterval = ob;
	}

	public void SaveSkiFlagHighscores(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/skif.dat");
		bf.Serialize(file, skiFlagHighscores);
		file.Close();
		LoadSkiFlagHighscores();
	}
	
	public void LoadSkiFlagHighscores(){
		if(File.Exists(Application.persistentDataPath + "/skif.dat")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/skif.dat", FileMode.Open);
			skiFlagHighscores = (List<Highscore>) bf.Deserialize(file);
			file.Close();
		}
		else
			skiFlagHighscores = new List<Highscore>();
	}

	public void SaveSkiTreeHighscores(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/skit.dat");
		bf.Serialize(file, skiTreeHighscores);
		file.Close();
		LoadSkiTreeHighscores();
	}
	
	public void LoadSkiTreeHighscores(){
		if(File.Exists(Application.persistentDataPath + "/skit.dat")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/skit.dat", FileMode.Open);
			skiTreeHighscores = (List<Highscore>) bf.Deserialize(file);
			file.Close();
		}
		else
			skiTreeHighscores = new List<Highscore>();
	}
	

	public void SaveSkiPaths(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/skipaths.dat");
		if(currentPath != null){
			skiPaths.Add(currentPath);
			bf.Serialize(file, skiPaths);
			file.Close();
			LoadSkiPaths ();
		}
		else{
			bf.Serialize(file, skiPaths);
			file.Close();
			LoadSkiPaths ();
		}
	}
	
	public void LoadSkiPaths(){
		if(File.Exists(Application.persistentDataPath + "/skipaths.dat")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/skipaths.dat", FileMode.Open);
			skiPaths = (List<SkiPathInfos>) bf.Deserialize(file);
			file.Close();
		}
		else
			skiPaths = new List<SkiPathInfos>();
	}

	public List<string> GetPathNames(){
		List<string> names = new List<string> ();
		for(int i = 0; i < skiPaths.Count; i++){
			names.Add(skiPaths[i].pathName);
		}
		return names;
	}

	public List<Vector3> GetPathLocalPositions(string pName){
		List<Vector3> positions = new List<Vector3> ();
		if(skiPaths.Exists(x => x.pathName.Equals(pName))){
			SkiPathInfos path = skiPaths.Find(x => x.pathName.Equals(pName));
			for(int i = 0; i < path.obstacles.Count; i++){
				Vector3 position = new Vector3 (path.obstacles[i].xLocalPos, path.obstacles[i].yLocalPos, path.obstacles[i].zLocalPos);
				positions.Add (position);
			}
		}
		return positions;
	}

	public List<float> GetPathPoss(string pName){
		List<float> poss = new List<float> ();
		if(skiPaths.Exists(x => x.pathName.Equals(pName))){
			SkiPathInfos path = skiPaths.Find(x => x.pathName.Equals(pName));
			poss = path.xPoss;
		}
		return poss;
	}

	public bool GetPathStepMode(string pName){
		if(skiPaths.Exists(x => x.pathName.Equals(pName))){
			SkiPathInfos path = skiPaths.Find(x => x.pathName.Equals(pName));
			return path.stepMov;
		}
		return false;
	}

	public bool GetRandomPath(string pName){
		if(skiPaths.Exists(x => x.pathName.Equals(pName))){
			SkiPathInfos path = skiPaths.Find(x => x.pathName.Equals(pName));
			return path.random;
		}
		return false;
	}

	public void DeletePath(string pathName){
		if(skiPaths.Exists(x => x.pathName.Equals(pathName)))
			skiPaths.Remove(skiPaths.Find(x => x.pathName.Equals(pathName)));
		SaveSkiPaths ();
	}

	public void UpdateSkiFlagHighscore(string player, int sco){
		if(skiFlagHighscores.Exists(x => x.pathName.Equals (player))){
			if(skiFlagHighscores.Find(x => x.pathName.Equals (player)).score < sco){
				skiFlagHighscores.Find(x => x.pathName.Equals (player)).score = sco;
				SaveSkiFlagHighscores();
				GetBestSkiFlagHighscore();
			}
		}
		else{
			Highscore hs = new Highscore();
			hs.pathName = player;
			hs.score = sco;
			skiFlagHighscores.Add(hs);
			SaveSkiFlagHighscores();
			GetBestSkiFlagHighscore();
		}
	}

	public void UpdateSkiTreeHighscore(string player, int sco){
		if(skiTreeHighscores.Exists(x => x.pathName.Equals (player))){
			if(skiTreeHighscores.Find(x => x.pathName.Equals (player)).score < sco){
				skiTreeHighscores.Find(x => x.pathName.Equals (player)).score = sco;
				SaveSkiTreeHighscores();
				GetBestSkiTreeHighscore();
			}
		}
		else{
			Highscore hs = new Highscore();
			hs.pathName = player;
			hs.score = sco;
			skiTreeHighscores.Add(hs);
			SaveSkiTreeHighscores();
			GetBestSkiTreeHighscore();
		}
	}

	public int GetPlayerFlagHighscore(string player){
		if(skiFlagHighscores.Exists(x => x.pathName.Equals (player))){
			return skiFlagHighscores.Find(x => x.pathName.Equals (player)).score;
		}
		else
			return 0;
	}

	public int GetPlayerTreeHighscore(string player){
		if(skiTreeHighscores.Exists(x => x.pathName.Equals (player))){
			return skiTreeHighscores.Find(x => x.pathName.Equals (player)).score;
		}
		else
			return 0;
	}


	public void GetBestSkiFlagHighscore(){
		foreach(Highscore sc in skiFlagHighscores){
			if(sc.score > maxFlagScore){
				maxFlagScore = sc.score;
				bestFlagPlayer = sc.pathName;
			}
		}
	}

	public void GetBestSkiTreeHighscore(){
		foreach(Highscore sc in skiTreeHighscores){
			if(sc.score > maxTreeScore){
				maxTreeScore = sc.score;
				bestTreePlayer = sc.pathName;
			}
		}
	}


	public void UpdateHighscore(string path, string player, int score){
		int high = 0;
		if(skiPaths.Exists(x => x.pathName.Equals(path))){
			if(score > skiPaths.Find (x => x.pathName.Equals(path)).highscore){
				skiPaths.Find (x => x.pathName.Equals(path)).highscore = score;
				skiPaths.Find (x => x.pathName.Equals(path)).bestPlayer = player;
			}
		}
	}
	
	public string GetBestPlayer(string pathName){
		string pName = "";
		if(skiPaths.Exists(x => x.pathName.Equals(pathName))){
			pName = skiPaths.Find (x => x.pathName.Equals(pathName)).bestPlayer;
		}
		return pName;
	}
	
	public int GetPathHighscore(string pathName){
		int high = 0;
		if(skiPaths.Exists(x => x.pathName.Equals(pathName))){
			high = skiPaths.Find (x => x.pathName.Equals(pathName)).highscore;
		}
		return high;
	}

	public int GetMaxFlagScore(){
		return maxFlagScore;
	}

	public string GetBestFlagPlayer(){
		return bestFlagPlayer;
	}

	public int GetMaxTreeScore(){
		return maxTreeScore;
	}

	public string GetBestTreePlayer(){
		return bestTreePlayer;
	}

	public float GetTrackSpeed(){
		return trackSpeed;
	}

	public bool GetStart(){
		return start;
	}

	public int GetNumberOfObstacles(){
		return numberOfObstacles;
	}

	public void SetStart(bool x){
		start = x;
	}

	public bool GetSlapMode(){
		return slapMode;
	}

	public bool GetSlideMode(){
		return slideMode;
	}

	public void SetSlapMode(bool x){
		slapMode = x;
		slideMode = !x;
	}
	
	public void SetStepMode(bool x){
		stepMode = x;
		smoothMode = !x;
	}

	public bool GetStepMode(){
		return stepMode;
	}

	public bool GetSmoothMode(){
		return smoothMode;
	}

	public int GetPoints(){
		return points;
	}

	public bool GetRandomTreePath(){
		return randomTreePath;
	}

	public bool GetRandomFlagPath(){
		return randomFlagPath;
	}
	public void SetRandomTreePath(bool x){
		if(x)
			PlayerSaveData.playerData.SetCurrentPathName("Random Alberi");
		randomTreePath = x;
	}
	
	public void SetRandomFlagPath(bool x){
		if(x)
			PlayerSaveData.playerData.SetCurrentPathName("Random Bandiere");
		randomFlagPath = x;
	}

	public void SetRandomPath(){
		currentPath.random = true;
	}

	public void SetXPoss(List<float> poss){
		currentPath.xPoss = poss;
	}
}

[Serializable]
class SkiPathInfos{
	public string pathName;
	public List<SkiObstacleInfos> obstacles;
	public List<float> xPoss;
	public int highscore;
	public string bestPlayer;
	public bool smoothMov;
	public bool stepMov;
	public bool random;
	
	public SkiPathInfos(){
		obstacles = new List<SkiObstacleInfos>();
		xPoss = new List<float> ();
		highscore = 0;
		bestPlayer = "";
		smoothMov = true;
		random = false;
	}
}

[Serializable]
class SkiObstacleInfos{
	public string obstacleName;
	public float xLocalPos;
	public float yLocalPos;
	public float zLocalPos;
}
