using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class ReplaySave : MonoBehaviour {

	string filePath;
	string game = "";
	string open = "";
	GameInfos infos;

	// Use this for initialization
	void Awake () {
		if(SaveInfos.plane)
			game = "Aereo";
		else if(SaveInfos.music)
			game = "Musica";
		else if(SaveInfos.ski)
			game = "Sci";
	}
	
	// Update is called once per frame
	void Update () {
		if(SaveInfos.plane)
			game = "Aereo";
		else if(SaveInfos.music)
			game = "Musica";
		else if(SaveInfos.ski)
			game = "Sci";

		if(PlayerSaveData.playerData.GetOpenHandMode())
			open = "Mano aperta";
		else
			open = "Mano chiusa";

		filePath = Application.persistentDataPath + "/" + game + "/" + PlayerSaveData.playerData.GetUserName() + "/" + PlayerSaveData.playerData.GetCurrentPathName() + "/" + open;
	}

	public void SaveToXML(string fileName){
		string path = filePath + "/" + fileName + ".xml";
		GetComponent<XMLGameSaveLoad>().SetFileLocation(path);
		GetComponent<XMLGameSaveLoad>().Save();
		Debug.Log ("Dati Salvati");
	}
	
	public void SaveData(){
		GameObject[] leftHandGos = GameObject.FindGameObjectsWithTag ("Left_Hand");
		GameObject[] rightHandGos = GameObject.FindGameObjectsWithTag ("Right_Hand");
		List<GameObject> leftBodies = leftHandGos.ToList<GameObject>();
		List<GameObject> rightBodies = rightHandGos.ToList<GameObject>();
		List<GameObject> toRemove = new List<GameObject>();

		List<SaveInfos.GameObjectInfos> lObjs = new List<SaveInfos.GameObjectInfos>();
		List<SaveInfos.GameObjectInfos> rObjs = new List<SaveInfos.GameObjectInfos>();
		for(int i = 0; i < leftBodies.Count; i++){
			SaveInfos.GameObjectInfos goi = new SaveInfos.GameObjectInfos(leftBodies[i]);
			lObjs.Add(goi);
		}
		for(int i = 0; i < rightBodies.Count; i++){
			SaveInfos.GameObjectInfos goi = new SaveInfos.GameObjectInfos(rightBodies[i]);
			rObjs.Add(goi);
		}
		SaveInfos.leftHandObjects.Add(lObjs);
		SaveInfos.rightHandObjects.Add(rObjs);
		SaveInfos.gameObjects.Add (lObjs);
		SaveInfos.gameObjects.Add (rObjs);
	}

	public void SaveHandsToFile(){
		string fileP = System.DateTime.Now.ToString ("yyyy-MM-dd HH_mm_ss");
		filePath += "/" + fileP;
		if(!System.IO.Directory.Exists(filePath))
			System.IO.Directory.CreateDirectory(filePath);
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(filePath + "/Left.dat");
		bf.Serialize(file, SaveInfos.leftHandObjects);
		file.Close();
		FileStream filer = File.Create(filePath + "/Right.dat");
		bf.Serialize(filer, SaveInfos.rightHandObjects);
		filer.Close();
		SaveGameInfos (filePath);
		SaveToXML (fileP);
	}

	public void LoadHands(string fileL, string fileR){
		if(File.Exists(fileL)){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(fileL, FileMode.Open);
			SaveInfos.leftHandObjects = (List<List<SaveInfos.GameObjectInfos>>) bf.Deserialize(file);
			file.Close();

		}
		if(File.Exists(fileR)){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(fileR, FileMode.Open);
			SaveInfos.rightHandObjects = (List<List<SaveInfos.GameObjectInfos>>) bf.Deserialize(file);
			file.Close();
		}
	}

	public string GetGame(){
		return game;
	}

	void SaveGameInfos(string fileP){
		infos = new GameInfos ();
		infos.planeSpeed = PlayerSaveData.playerData.GetPlaneSpeed ();
		infos.oneHandMode = PlayerSaveData.playerData.GetOneHandMode ();
		infos.openHandMode = PlayerSaveData.playerData.GetOpenHandMode ();
		infos.rightHandSelected = PlayerSaveData.playerData.GetRightHand ();
		infos.startAngx = PlayerSaveData.playerData.GetStartAngles ().x;
		infos.startAngy = PlayerSaveData.playerData.GetStartAngles ().y;
		infos.startAngz = PlayerSaveData.playerData.GetStartAngles ().z;
		infos.seed = PlayerSaveData.playerData.GetSeed ();
		infos.gameDifficulty = PlayerSaveData.playerData.GetSkiDifficulty ();
		infos.randomPath = PlayerSaveData.playerData.GetRandomPath ();
		infos.gameTime = PlayerSaveData.playerData.GetGameTime ();
		infos.score = PlayerSaveData.playerData.GetScore ();
		infos.totObstacles = PlayerSaveData.playerData.GetTotObstacles ();
		Hashtable tunings = PlayerSaveData.playerData.GetPlayerTunings();
		infos.minLeftVertical = (float)tunings ["Min Left Vertical"];
		infos.maxLeftVertical = (float)tunings ["Max Left Vertical"];
		infos.minLeftHorizontal = (float)tunings ["Min Left Horizontal"];
		infos.maxLeftHorizontal = (float)tunings ["Max Left Horizontal"];
		infos.minRightVertical = (float)tunings ["Min Right Vertical"];
		infos.maxRightVertical = (float)tunings ["Max Right Vertical"];
		infos.minRightHorizontal = (float)tunings ["Min Right Horizontal"];
		infos.maxRightHorizontal = (float)tunings ["Max Right Horizontal"];
		//infos.slideMode = SkiSaveData.skiData.GetSlideMode ();

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(fileP + "/Infos.dat");
		bf.Serialize(file, infos);
		file.Close();
	}

	public void LoadGameInfos(string fileI){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(fileI, FileMode.Open);
		infos = (GameInfos) bf.Deserialize(file);
		file.Close();
		Vector3 angs = new Vector3 (infos.startAngx, infos.startAngy, infos.startAngz);
		PlayerSaveData.playerData.SetStartAngles (angs);
		PlayerSaveData.playerData.SetPlaneSpeed (infos.planeSpeed);
		PlayerSaveData.playerData.SetOneHandMode (infos.oneHandMode);
		PlayerSaveData.playerData.SetOpenHandMode (infos.openHandMode);
		PlayerSaveData.playerData.SetRightHand (infos.rightHandSelected);
		PlayerSaveData.playerData.SetSeed (infos.seed);
		PlayerSaveData.playerData.SetSkiDifficulty (infos.gameDifficulty);
		PlayerSaveData.playerData.SetRandomPath (infos.randomPath);
		PlayerSaveData.playerData.SetGameTime (infos.gameTime);
		PlayerSaveData.playerData.SetReplayTunings (infos.minLeftVertical,infos.maxLeftVertical,infos.minLeftHorizontal, infos.maxLeftHorizontal,
		                                            infos.minRightVertical,infos.maxRightVertical, infos.minRightHorizontal, infos.maxRightHorizontal);
		//SkiSaveData.skiData.SetSlapMode (!infos.slideMode);
	}
}

[Serializable]
public class GameInfos{
	public float planeSpeed;
	public bool oneHandMode;
	public bool openHandMode;
	public bool rightHandSelected;
	public float startAngx;
	public float startAngy;
	public float startAngz;
	public int seed;
	public int gameDifficulty;
	public bool randomPath;
	public float gameTime;
	public int score;
	public int totObstacles;
	public float minLeftVertical;
	public float maxLeftVertical;
	public float minLeftHorizontal;
	public float maxLeftHorizontal;
	public float minRightVertical;
	public float maxRightVertical;
	public float minRightHorizontal;
	public float maxRightHorizontal;
	//public bool slideMode;
}
