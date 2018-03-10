using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FisioPlaneMenuGUI : MonoBehaviour {

	public GUISkin customSkin;

	Vector2 scrollPosition = Vector2.zero;
	int selGridInt = 0;
	int velGridInt = 1;
	int timGridInt = 2;
	Rect windowRect;
	float width = 350;
	float height = 350f;
	string pathName = "";
	bool menu = true;
	bool pause, save, load, setup;
	bool exists = false;
	List<string> pathNames;

	void Start(){
		windowRect = new Rect ((Screen.width-width)/2, (Screen.height-height)/2, width, height);
		pathNames = PathSaveData.pathData.GetPathNames ();
	}

	void Update(){
		if(Input.GetKeyDown("escape") && !pause){
			pause = true;
			SendMessage ("Pause", true);
		}
	}

	void OnGUI(){
		windowRect.x = (Screen.width-width)/2;
		windowRect.y = (Screen.height-height)/2;
		//windowRect.width = Screen.width*width;
		//windowRect.height = Screen.height*height;

		GUI.skin = customSkin;
		if(save)
			windowRect = GUI.Window (0, windowRect, SaveWindow, "Salvataggio");
		if(menu)
			windowRect = GUI.Window (1, windowRect, MenuWindow, "Menu");
		if(pause)
			windowRect = GUI.Window (2, windowRect, PauseWindow, "Pausa");
		if(load)
			windowRect = GUI.Window (3, windowRect, LoadWindow, "Carica Percorso");
		if(setup)
			windowRect = GUI.Window (4, windowRect, SetupWindow, "Impostazioni");
	}

	void SaveWindow (int id){
		GUI.skin = customSkin;
		GUI.Label (new Rect(30,50,250,25), "Nome del percorso:");
		pathName = GUI.TextField(new Rect(30, 80 , 190f, 25f),pathName,20);
		if (GUI.Button(new Rect(20, 250, 100, 50), "Salva")){
			PathSaveData.pathData.SetCurrentPathName(pathName);
			if(!PathSaveData.pathData.CheckPathName(pathName)){
				exists = false;
				PathSaveData.pathData.SavePaths();
				Debug.Log ("Percorso salvato");
				Application.LoadLevel(Application.loadedLevel);
			}
			else{
				exists = true;
				Debug.Log ("Esiste già un percorso salvato con questo nome.");	
			}
		}

		if(exists)
			GUI.Label (new Rect(20,100,330,50), "Esiste già un percorso salvato con questo nome.");

		if (GUI.Button(new Rect(230, 250, 100, 50), "Annulla")){
			save = false;
			pause = true;
		}
	}

	void MenuWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 30, 200, 75), "Crea Percorso")){
			menu = false;
			setup = true;
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 100, 200, 75), "Percorsi Salvati")){
			pathNames = PathSaveData.pathData.GetPathNames ();
			load = true;
			menu = false;
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 170, 200, 75), "Guarda replay")){
			Application.LoadLevel("Plane_Replay");
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 240, 200, 75), "Menu principale")){
			Application.LoadLevel("Fisio_ Main");
		}
	}

	void SetupWindow(int id){
		GUI.skin = customSkin;
		string[] timInts = new string[] {Convert.ToString(1), Convert.ToString(2), Convert.ToString(3), Convert.ToString(4), Convert.ToString(5)};
		string[] velStrings = new string[]{"bassa", "media", "alta"};
		GUI.Label (new Rect(30,40,250,25), new GUIContent("Intervallo di tempo tra ostacoli", "Definisce l'intervallo di tempo (in secondi) tra la creazione di un ostacolo e la creazione di quello successivo"));
		timGridInt = GUI.SelectionGrid(new Rect(25, 65, 300, 50), timGridInt, timInts, 5);
		GUI.Label (new Rect(30,130,150,25), new GUIContent("Velocita' aereo", "La velocita' a cui si muove l'aereo. Piu' alta e' la velocità, piu' saranno distanti gli ostacoli."));
		velGridInt = GUI.SelectionGrid(new Rect(25, 155, 300, 50), velGridInt, velStrings, 3);
		GUI.Label (new Rect(30,185,300,100), GUI.tooltip);

		if (GUI.Button(new Rect(20, 260, 150, 75), "Inizia")){
			PathSaveData.pathData.SetObstacleInterval(timGridInt + 1);
			PlayerSaveData.playerData.SetPlaneSpeed(velGridInt+1);
			setup = false;
			SendMessage("Begin");
		}
		if (GUI.Button(new Rect(180, 260, 150, 75), "Indietro")){
			Application.LoadLevel("Menu");
		}
	}

	void PauseWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 40, 200, 75), "Riprendi")){
			pause = false;
			SendMessage ("Pause", false);
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 140, 200, 75), "Salva il percorso")){
			save = true;
			pause = false;
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 240, 200, 75), "Torna al menu")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	void LoadWindow(int id){
		GUI.skin = customSkin;
		int count = pathNames.Count;
		string[] selStrings = pathNames.ToArray ();
		GUI.Label (new Rect((windowRect.width - 120)/2,20,200,25), "Seleziona il percorso");
		scrollPosition = GUI.BeginScrollView(new Rect(20, 50, 300, 220), scrollPosition, new Rect(0, 0, 280, 50*count));
		selGridInt = GUI.SelectionGrid(new Rect(0, 0, 250, 50*count), selGridInt, selStrings, 1);
		GUI.EndScrollView();
		if (GUI.Button(new Rect(20, 280, 100, 50), "Carica")){
			PlayerSaveData.playerData.SetCurrentPathName(selStrings[selGridInt]);
			SendMessage ("CreatePath", selStrings[selGridInt]);
			load = false;
			SendMessage ("BeginTest");
		}
		if (GUI.Button(new Rect(125, 280, 100, 50), "Elimina")){
			PathSaveData.pathData.DeletePath(selStrings[selGridInt]);
			pathNames = PathSaveData.pathData.GetPathNames ();
		}
		if (GUI.Button(new Rect(230, 280, 100, 50), "Indietro")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}
	
}
