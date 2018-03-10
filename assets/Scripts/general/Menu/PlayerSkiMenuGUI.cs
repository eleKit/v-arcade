using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSkiMenuGUI : MonoBehaviour {

	public GUISkin customSkin;

	Vector2 scrollPosition = Vector2.zero;
	int selGridInt = 0;
	int modGridInt = 0;
	int handInt = 0;
	int movInt = 0;
	int obsInt = 0;
	int diffInt = 1;
	Rect windowRect;
	float width = 350;
	float height = 350f;
	bool setup;
	bool menu = true;
	bool pause, end, load, loadSetup;
	List<string> pathNames;
	
	void Start(){
		windowRect = new Rect ((Screen.width-width)/2, (Screen.height-height)/2, width, height);
	}
	
	void Update(){
		if(Input.GetKeyDown("escape") && !pause){
			pause = true;
			SendMessage ("Pause");
		}
	}
	
	void OnGUI(){
		windowRect.x = (Screen.width-width)/2;
		windowRect.y = (Screen.height-height)/2;
		
		GUI.skin = customSkin;
		if(end)
			windowRect = GUI.Window (0, windowRect, EndWindow, "Finito");
		if(pause)
			windowRect = GUI.Window (2, windowRect, PauseWindow, "Pausa");
		if(setup)
			windowRect = GUI.Window (1, windowRect, SetupWindow, "Impostazioni");
		if(menu)
			windowRect = GUI.Window (3, windowRect, MenuWindow, "Menu Principale");
		if(load)
			windowRect = GUI.Window (4, windowRect, LoadWindow, "Carica Percorso");
		if(loadSetup)
			windowRect = GUI.Window (5, windowRect, LoadSetupWindow, "Impostazioni");
	}
	
	void MenuWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 40, 200, 75), "Percorso casuale")){
			menu = false;
			setup = true;
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 140, 200, 75), "Percorsi Salvati")){
			pathNames = SkiSaveData.skiData.GetPathNames ();
			load = true;
			menu = false;
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 240, 200, 75), "Menu principale")){
			Application.LoadLevel("Ski_Main");
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
		if (GUI.Button(new Rect(230, 280, 100, 50), "Indietro")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}
	
	void LoadSetupWindow(int id){
		GUI.skin = customSkin;
		string[] modeStrings = new string[]{"180°", "90°"};
		string[] handStrings = new string[]{"Sinistra", "Destra"};
		
		GUI.Label (new Rect(30,30,200,25), "Inclinazione mano");
		modGridInt = GUI.SelectionGrid(new Rect(25, 50, 300, 40), modGridInt, modeStrings, 2);
		GUI.Label (new Rect(30,90,200,25), "Seleziona mano");
		handInt = GUI.SelectionGrid(new Rect(25, 110, 300, 40), handInt, handStrings, 2);
		
		if (GUI.Button(new Rect(20, 260, 150, 75), "Inizia")){
			SkiSaveData.skiData.SetRandomFlagPath(false);
			SkiSaveData.skiData.SetRandomTreePath(false);
			SkiSaveData.skiData.SetSlapMode(modGridInt == 1);
			PlayerSaveData.playerData.SetRightHand(handInt == 1);
			SendMessage ("BeginSaved");
			loadSetup = false;
		}
		if (GUI.Button(new Rect(180, 260, 150, 75), "Indietro")){
			load = true;
			loadSetup = false;
		}
	}

	void PauseWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 30, 200, 75), "Riprendi")){
			pause = false;
			SendMessage ("UnPause");
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 100, 200, 75), "Ricomincia")){
			pause = false;
			SendMessage("Reload");
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 170, 200, 75), "Torna al menu")){
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 240, 200, 75), "Salva (Testing)")){
			SendMessage("SaveToXML");
		}
	}

	void SetupWindow(int id){
		GUI.skin = customSkin;
		string[] modeStrings = new string[]{"180°", "90°"};
		string[] handStrings = new string[]{"Sinistra", "Destra"};
		string[] diffStrings = new string[]{"Facile","Medio","Difficile"};
		//string[] movStrings = new string[]{"Continuo", "Scatto"};

		GUI.Label (new Rect(30,90,200,25), "Seleziona mano");
		handInt = GUI.SelectionGrid(new Rect(25, 110, 300, 40), handInt, handStrings, 2);
		GUI.Label (new Rect(30,30,250,25), new GUIContent("Seleziona difficolta'", "La difficolta' influisce sull'ampiezza del movimento da effettuare."));
		diffInt = GUI.SelectionGrid(new Rect(25, 50, 300, 40), diffInt, diffStrings, 3);
		//GUI.Label (new Rect(30,30,250,25), "Seleziona movimento");
		//movInt = GUI.SelectionGrid(new Rect(25, 50, 300, 40), movInt, movStrings, 2);
		//if(movInt == 1){
			GUI.Label (new Rect(30,150,200,25), "Inclinazione mano");
			modGridInt = GUI.SelectionGrid(new Rect(25, 170, 300, 40), modGridInt, modeStrings, 2);
		//}
		GUI.Label (new Rect(30,200,300,100), GUI.tooltip);

		
		if (GUI.Button(new Rect(20, 265, 150, 65), "Inizia")){
			SkiSaveData.skiData.SetSlapMode(modGridInt == 1);
			//SkiSaveData.skiData.SetStepMode(movInt == 1);
			SkiSaveData.skiData.SetStepMode(false);
			PlayerSaveData.playerData.SetRightHand(handInt == 1);
			PlayerSaveData.playerData.SetSkiDifficulty(diffInt+1);
			setup = false;
//			if(movInt == 0){
			SkiSaveData.skiData.SetRandomFlagPath(true);
//				SkiSaveData.skiData.SetRandomTreePath(false);
//			}
//			else{
//				SkiSaveData.skiData.SetRandomFlagPath(false);
//				SkiSaveData.skiData.SetRandomTreePath(true);
//			}
			SendMessage("Begin");
		}
		if (GUI.Button(new Rect(180, 265, 150, 65), "Torna al menu")){
			Application.LoadLevel("Menu");
		}
	}

	void EndWindow(int id){
		GUI.skin = customSkin;
		GUI.Label (new Rect(30,40,200,25), "Punteggio:");
		GUI.Label (new Rect(180,40,50,25), PlayerSaveData.playerData.GetScore().ToString());
		GUI.Label (new Rect(30,80,250,25), "Il Tuo Punteggio Migliore:");
		GUI.Label (new Rect(280,80,50,25), PlayerSaveData.playerData.GetHighscore().ToString());
		GUI.Label (new Rect(30,120,200,25), "Il Punteggio Migliore");

		string bestPlayer = "";
		if(SkiSaveData.skiData.GetRandomFlagPath())
			bestPlayer = SkiSaveData.skiData.GetBestFlagPlayer().ToString();
		else if(SkiSaveData.skiData.GetRandomTreePath())
			bestPlayer = SkiSaveData.skiData.GetBestTreePlayer().ToString();
		else
			bestPlayer = SkiSaveData.skiData.GetBestPlayer(PlayerSaveData.playerData.GetCurrentPathName()).ToString();

		GUI.Label (new Rect(30,150,200,25), bestPlayer);

		string maxScore = "";
		if(SkiSaveData.skiData.GetRandomFlagPath())
			maxScore = SkiSaveData.skiData.GetMaxFlagScore().ToString();
		else if(SkiSaveData.skiData.GetRandomTreePath())
			maxScore = SkiSaveData.skiData.GetMaxTreeScore().ToString();
		else
			maxScore = SkiSaveData.skiData.GetPathHighscore(PlayerSaveData.playerData.GetCurrentPathName()).ToString();
		GUI.Label (new Rect(180,150,200,25), maxScore);
		
		if (GUI.Button(new Rect(20f, 180, 150, 75), "Ricomincia")){
			end = false;
			SendMessage("Reload");
		}
		if (GUI.Button(new Rect(180, 180, 150, 75), "Torna al menu")){
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 260, 200, 75), "Salva (Testing)")){
			SendMessage("SaveToXML");
		}
	}

	
	void EndMenu(){
		end = true;
	}

	void PauseMenu(){
		pause = true;
	}
}
