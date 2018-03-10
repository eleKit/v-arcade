using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;

public class FisioSkiMenuGUI : MonoBehaviour {

	public GUISkin customSkin;

	Vector2 scrollPosition = Vector2.zero;
	int modGridInt = 0;
	int timGridInt = 2;
	int handInt = 0;
	int selGridInt = 0;
	int movInt = 0;
	int obsInt = 0;
	Rect windowRect;
	float width = 350;
	float height = 350f;
	string pathName = "";
	bool menu = true;
	bool pause, save, load, setup, loadSetup, create;
	bool exists = false;
	List<string> pathNames;

	void Start(){
		windowRect = new Rect ((Screen.width-width)/2, (Screen.height-height)/2, width, height);
		pathNames = SkiSaveData.skiData.GetPathNames ();
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
		if(create)
			windowRect = GUI.Window (6, windowRect, CreateWindow, "Modalita' creazione");
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
		if(loadSetup)
			windowRect = GUI.Window (5, windowRect, LoadSetupWindow, "Modalità");
	}

	void SaveWindow (int id){
		GUI.skin = customSkin;
		GUI.Label (new Rect(30,50,250,25), "Nome del percorso:");
		pathName = GUI.TextField(new Rect(30, 80 , 190f, 25f),pathName,20);
		if (GUI.Button(new Rect(20, 250, 100, 50), "Salva")){
			SkiSaveData.skiData.SetCurrentPathName(pathName);
			if(!SkiSaveData.skiData.CheckPathName(pathName)){
				exists = false;
				SkiSaveData.skiData.SaveSkiPaths();
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

	void CreateWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 30, 200, 75), "Crea Manualmente")){
			create = false;
			setup = true;
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 100, 200, 75), "Crea Random")){
			Application.LoadLevel("Fisio_Random_Editor");
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 170, 200, 75), "Indietro")){
			create = false;
			menu = true;
		}
	}

	void MenuWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 30, 200, 75), "Crea Percorso")){
			menu = false;
			create = true;
			//setup = true;
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 100, 200, 75), "Percorsi Salvati")){
			pathNames = SkiSaveData.skiData.GetPathNames ();
			load = true;
			menu = false;
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 170, 200, 75), "Guarda replay")){
			SaveInfos.replay = true;
			Application.LoadLevel("Ski_Replay");
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 240, 200, 75), "Menu principale")){
			Application.LoadLevel("Fisio_Main");
		}
	}

	void SetupWindow(int id){
		GUI.skin = customSkin;
		string[] modeStrings = new string[]{"180°", "90°"};
		//string[] movStrings = new string[]{"Continuo", "Scatto"};
		string[] timInts = new string[] {Convert.ToString(1.5), Convert.ToString(2), Convert.ToString(2.5)};
		

		//GUI.Label (new Rect(30,30,250,25), "Seleziona movimento");
		//movInt = GUI.SelectionGrid(new Rect(25, 50, 300, 40), movInt, movStrings, 2);
		GUI.Label (new Rect(30,150,200,25), "Inclinazione mano");
		modGridInt = GUI.SelectionGrid(new Rect(25, 170, 300, 40), modGridInt, modeStrings, 2);
		GUI.Label (new Rect(30,90,250,25), "Intervallo di tempo tra ostacoli");
		timGridInt = GUI.SelectionGrid(new Rect(25, 110, 300, 50), timGridInt, timInts, 3);

		if (GUI.Button(new Rect(20, 260, 150, 75), "Inizia")){
			SkiSaveData.skiData.SetObstacleInterval(float.Parse(timInts[timGridInt], CultureInfo.InvariantCulture.NumberFormat));
			setup = false;
			SkiSaveData.skiData.SetSlapMode(modGridInt == 1);
			SkiSaveData.skiData.SetStepMode(false);
			//SkiSaveData.skiData.SetStepMode(movInt == 1);
			SendMessage("Begin");
		}
		if (GUI.Button(new Rect(180, 260, 150, 75), "Indietro")){
			setup = false;
			menu = true;
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
			loadSetup = true;
		}
		if (GUI.Button(new Rect(125, 280, 100, 50), "Elimina")){
			SkiSaveData.skiData.DeletePath(selStrings[selGridInt]);
			pathNames = SkiSaveData.skiData.GetPathNames ();
		}
		if (GUI.Button(new Rect(230, 280, 100, 50), "Indietro")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	void LoadSetupWindow(int id){
		GUI.skin = customSkin;
		string[] modeStrings = new string[]{"180°", "90°"};

		GUI.Label (new Rect(30,30,200,25), "Inclinazione mano");
		modGridInt = GUI.SelectionGrid(new Rect(25, 50, 300, 40), modGridInt, modeStrings, 2);
		
		if (GUI.Button(new Rect(20, 260, 150, 75), "Inizia")){
			SkiSaveData.skiData.SetSlapMode(modGridInt == 1);
			SendMessage ("BeginTest");
			loadSetup = false;
		}
		if (GUI.Button(new Rect(180, 260, 150, 75), "Indietro")){
			load = true;
			loadSetup = false;
		}
	}
	
}
