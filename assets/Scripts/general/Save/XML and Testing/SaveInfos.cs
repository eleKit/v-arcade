using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Classe utilizzata per salvare le informazioni da utilizzare nella sessione di gioco
using System;

public static class SaveInfos{

	public static bool plane = true;
	public static bool music;
	public static bool ski;
	public static bool replay;

	public static List<List<GameObjectInfos>> gameObjects = new List<List<GameObjectInfos>> ();
	public static List<List<GameObjectInfos>> leftHandObjects = new List<List<GameObjectInfos>> ();
	public static List<List<GameObjectInfos>> rightHandObjects = new List<List<GameObjectInfos>> ();

	public static void ResetData(){
		gameObjects = new List<List<GameObjectInfos>> ();
		leftHandObjects = new List<List<GameObjectInfos>> ();
		rightHandObjects = new List<List<GameObjectInfos>> ();
	}

	[Serializable]
	public class GameObjectInfos{
		public string gName;
		public string gID;
		public float posX;
		public float posY;
		public float posZ;
		public float angX;
		public float angY;
		public float angZ;
//		public Vector3 pos;
//		public Vector3 ang;
		
		public GameObjectInfos(GameObject go){
			gName = go.name;
			gID = go.name + "_" + go.GetInstanceID();
			posX = go.transform.position.x;
			posY = go.transform.position.y;
			posZ = go.transform.position.z;
			angX = go.transform.eulerAngles.x;
			angY = go.transform.eulerAngles.y;
			angZ = go.transform.eulerAngles.z;
//			pos = go.transform.position;
//			ang = go.transform.eulerAngles;
		}
		public GameObjectInfos(){
			gName = "";
			gID = "";
		}
	}
}
