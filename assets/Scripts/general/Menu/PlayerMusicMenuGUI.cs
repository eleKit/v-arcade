using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMusicMenuGUI : MonoBehaviour {

	public GUISkin customSkin;
	
	Vector2 scrollPosition = Vector2.zero;
	int selGridInt = 0;
	Rect windowRect;
	float width = 350;
	float height = 350f;
	string selectedSong = "";
	bool menu = true;
	bool pause, load, end;
	List<string> songNames;
	
	void Start(){
		windowRect = new Rect ((Screen.width-width)/2, (Screen.height-height)/2, width, height);
		songNames = MusicSaveData.musicData.GetSongNames();
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
		if(menu)
			windowRect = GUI.Window (1, windowRect, MenuWindow, "Benvenuto!");
		if(pause)
			windowRect = GUI.Window (2, windowRect, PauseWindow, "Pausa");
		if(load)
			windowRect = GUI.Window (3, windowRect, LoadWindow, "Carica Percorso");
	}
	
	
	void MenuWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 80, 210, 75), "Seleziona Canzone")){
			menu = false;
			load = true;
		}
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 180, 210, 75), "Menu principale")){
			Application.LoadLevel("Mandolin_Main");
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
			SendMessage("CreatePath", selectedSong);
		}
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 170, 200, 75), "Torna al menu")){
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 240, 200, 75), "Salva (Testing)")){
			SendMessage("SaveToXML");
		}
	}
	
	void EndWindow(int id){
		GUI.skin = customSkin;
		GUI.Label (new Rect(30,40,200,25), "Punteggio:");
		GUI.Label (new Rect(180,40,50,25), PlayerSaveData.playerData.GetScore().ToString());
		GUI.Label (new Rect(30,80,250,25), "Il Tuo Punteggio Migliore:");
		GUI.Label (new Rect(180,80,50,25), PlayerSaveData.playerData.GetHighscore().ToString());
		GUI.Label (new Rect(30,120,200,25), "Il Punteggio Migliore");
		GUI.Label (new Rect(30,150,200,25), MusicSaveData.musicData.GetBestPlayer(PlayerSaveData.playerData.GetCurrentPathName()));
		GUI.Label (new Rect(180,150,200,25), MusicSaveData.musicData.GetHighscore(PlayerSaveData.playerData.GetCurrentPathName()).ToString());
		
		if (GUI.Button(new Rect(20f, 180, 150, 75), "Ricomincia")){
			SendMessage("CreatePath", selectedSong);
			end = false;
		}
		if (GUI.Button(new Rect(180, 180, 150, 75), "Torna al menu")){
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (GUI.Button(new Rect((windowRect.width - 200)/2, 260, 200, 75), "Salva (Testing)")){
			SendMessage("SaveToXML");
		}
	}
	
	void LoadWindow(int id){
		GUI.skin = customSkin;
		int count = songNames.Count;
		string[] selStrings = songNames.ToArray ();
		GUI.Label (new Rect((windowRect.width - 120)/2,20,200,25), "Seleziona il percorso");
		scrollPosition = GUI.BeginScrollView(new Rect(20, 50, 300, 220), scrollPosition, new Rect(0, 0, 280, 50*count));
		selGridInt = GUI.SelectionGrid(new Rect(0, 0, 250, 50*count), selGridInt, selStrings, 1);
		GUI.EndScrollView();
		if (GUI.Button(new Rect(30, 280, 100, 50), "Seleziona")){
			selectedSong = selStrings[selGridInt];
			PlayerSaveData.playerData.SetCurrentPathName(selectedSong);
			SendMessage ("CreatePath", selStrings[selGridInt]);
			load = false;
		}
		if (GUI.Button(new Rect(220, 280, 100, 50), "Indietro")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}
	
	void EndMenu(){
		Camera.main.audio.Stop ();
		end = true;
	}
}
