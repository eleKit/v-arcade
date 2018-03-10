using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;

public class MusicSaveData : MonoBehaviour {

	public static MusicSaveData musicData;

	List<MusicHighscore> highscores = new List<MusicHighscore>();

	List<string> songs = new List<string>();
	float buttonSpeed = 17;
	float leftGuideX = -10f;
	float rightGuideX = 10f;
	float centerY = -13.75f;
	float buttonZ = -2f;
	bool leftGesture, rightGesture, start;
	int buttonScore = 20;
	string locationPath;

	void Awake () {
		if(musicData == null){
			DontDestroyOnLoad(gameObject);
			musicData = this;
			LoadHighscores();
			locationPath = Application.dataPath + "/Resources/Audio/Mandolin txts/";
			LoadFileNames();
		}
		else if(musicData != this){
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SaveHighscores(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/music.dat");
		bf.Serialize(file, highscores);
		file.Close();
		LoadHighscores ();
	}

	void LoadFileNames(){
		string[] files = System.IO.Directory.GetFiles(locationPath, "*.txt");
		for(int i = 0; i < files.Length; i++){
			string[] pieces = files[i].Split(new Char [] {'/'});
			string[] name = pieces[pieces.Length-1].Split(new Char [] {'.'});
			songs.Add(name[0]);
		}
	}
	
	public void LoadHighscores(){
		if(File.Exists(Application.persistentDataPath + "/music.dat")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/music.dat", FileMode.Open);
			highscores = (List<MusicHighscore>) bf.Deserialize(file);
			file.Close();
		}
		else
			highscores = new List<MusicHighscore>();;
	}

	public void AddHighscore(string song, string player, int score){
		if(highscores.Exists(x => x.songName.Equals(song)))
			UpdateHighscore(song, player, score);
		else{
			MusicHighscore mh = new MusicHighscore(song, player, score);
			highscores.Add(mh);
			SaveHighscores();
		}
	}

	public void UpdateHighscore(string song, string player, int score){
		if(highscores.Find (x => x.songName.Equals(song)).score < score){
			highscores.Find (x => x.songName.Equals(song)).score = score;
			highscores.Find (x => x.songName.Equals(song)).userName = player;
		}
	}

	public int GetHighscore(string song){
		if(highscores.Exists(x => x.songName.Equals(song)))
			return highscores.Find (x => x.songName.Equals(song)).score;
		else
			return 0;
	}

	public string GetBestPlayer(string song){
		if(highscores.Exists(x => x.songName.Equals(song)))
			return highscores.Find (x => x.songName.Equals(song)).userName;
		else
			return "";
	}

	public string GetTxtLocationPath(){
		return locationPath;
	}

	public int GetButtonScore(){
		return buttonScore;
	}

	public bool GetStart(){
		return start;
	}

	public void SetStart(bool x){
		start = x;
	}

	public float GetButtonSpeed(){
		return buttonSpeed;
	}

	public bool GetRightGesture(){
		return rightGesture;
	}

	public bool GetLeftGesture(){
		return leftGesture;
	}

	public void SetLeftGesture(bool x){
		leftGesture = x;
	}

	public void SetRightGesture(bool x){
		rightGesture = x;
	}

	public float GetLeftGuideX(){
		return leftGuideX;
	}

	public float GetRightGuideX(){
		return rightGuideX;
	}

	public List<string> GetSongNames(){
		return songs;
	}

	public float GetCenterY(){
		return centerY;
	}

	public float GetButtonZ(){
		return buttonZ;
	}
}

[Serializable]
public class MusicHighscore{
	public string songName;
	public string userName;
	public int score;

	public MusicHighscore(){}

	public MusicHighscore(string songN, string pl, int sco){
		songName = songN;
		userName = pl;
		score = sco;
	}
}
