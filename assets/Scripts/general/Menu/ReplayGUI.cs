using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayGUI : MonoBehaviour {

	public GUISkin customSkin;
	
	Vector2 scrollPosition = Vector2.zero;
	int selUsInt = 0;
	int selPtInt = 0;
	int selMdInt = 0;
	int selRnInt = 0;
	Rect windowRect;
	float width = 350;
	float height = 350f;
	bool menu = true;
	bool pause, end, load;
	bool selectName, selectPath, selectMode, selectRun;
	List<string> users;
	string mode, run;
	
	void Start(){
		windowRect = new Rect ((Screen.width-width)/2, (Screen.height-height)/2, width, height);
		users = GetComponent<ReplayController> ().GetAvailableUsers ();
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
		
		if(pause)
			windowRect = GUI.Window (0, windowRect, PauseWindow, "Pausa");
		if(menu)
			windowRect = GUI.Window (1, windowRect, MenuWindow, "Menu");
		if(end)
			windowRect = GUI.Window (2, windowRect, EndWindow, "Fine");
		if(load)
			windowRect = GUI.Window (3, windowRect, LoadWindow, "Carica Replay");
		
	}
	
	void PauseWindow (int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 40, 210, 75), "Riprendi")){
			pause = false;
		}
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 140, 210, 75), "Ricomincia")){
			pause = false;
		}
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 240, 210, 75), "Menu")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}
	
	void EndWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 150)/2, 80, 150, 75), "Ricomincia")){
			menu = true;
			end = false;
		}
		
		if (GUI.Button(new Rect((windowRect.width - 150)/2, 200, 150, 75), "Menu")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}
	
	void MenuWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 80, 210, 75), "Seleziona replay")){
			load = true;
			selectName = true;
			selectPath = false;
			selectMode = false;
			selectRun = false;
			menu = false;
		}
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 200, 210, 75), "Menu principale")){
			SaveInfos.replay = false;
			Application.LoadLevel("Fisio_Main");
		}
	}
	
	void LoadWindow(int id){
		GUI.skin = customSkin;
		string userN;
		
		if(selectRun){
			List<string> runs = GetComponent<ReplayController>().GetAvailableRuns(PlayerSaveData.playerData.GetUserName(), PlayerSaveData.playerData.GetCurrentPathName(), mode);
			int count = runs.Count;
			string[] selStrings = runs.ToArray ();
			GUI.Label (new Rect((windowRect.width - 120)/2,20,200,25), "Seleziona partita");
			scrollPosition = GUI.BeginScrollView(new Rect(20, 50, 300, 220), scrollPosition, new Rect(0, 0, 280, 50*count));
			selRnInt = GUI.SelectionGrid(new Rect(0, 0, 250, 50*count), selRnInt, selStrings, 1);
			GUI.EndScrollView();
			if (GUI.Button(new Rect(30, 280, 100, 50), "Inizia")){
				run = selStrings[selRnInt];
				load = false;
				GetComponent<ReplayController>().LoadHands(PlayerSaveData.playerData.GetUserName(), PlayerSaveData.playerData.GetCurrentPathName(), mode, run);
				SendMessage ("CreatePath", PlayerSaveData.playerData.GetCurrentPathName());
				selectRun = false;
				if(SaveInfos.plane)
					SendMessage("Go");
			}
			if (GUI.Button(new Rect(220, 280, 100, 50), "Indietro")){
				selectMode = true;
				selectRun = false;
			}
		}
		
		if(selectMode){
			if(SaveInfos.plane){
				string[] openStrings = new string[]{"Mano aperta", "Mano chiusa"};
				GUI.Label (new Rect((windowRect.width - 120)/2,20,200,25), "Seleziona modalita'");
				//scrollPosition = GUI.BeginScrollView(new Rect(20, 50, 300, 220), scrollPosition, new Rect(0, 0, 280, 50*count));
				selMdInt = GUI.SelectionGrid(new Rect(30, 50, 230, 100), selMdInt, openStrings, 1);
				//GUI.EndScrollView();
				if (GUI.Button(new Rect(30, 280, 100, 50), "Partita")){
					mode = openStrings[selMdInt];
					selectRun = true;
					selectMode = false;
				}
				if (GUI.Button(new Rect(220, 280, 100, 50), "Indietro")){
					selectMode = false;
					selectPath = true;
				}
			}
			else{
				mode = "Mano aperta";
				selectRun = true;
				selectMode = false;
			}
		}
		
		if(selectPath){
			List<string> paths = GetComponent<ReplayController>().GetAvailablePaths(PlayerSaveData.playerData.GetUserName());
			int count = paths.Count;
			string[] selStrings = paths.ToArray ();
			GUI.Label (new Rect((windowRect.width - 120)/2,20,200,25), "Seleziona percorso");
			scrollPosition = GUI.BeginScrollView(new Rect(20, 50, 300, 220), scrollPosition, new Rect(0, 0, 280, 50*count));
			selPtInt = GUI.SelectionGrid(new Rect(0, 0, 250, 50*count), selPtInt, selStrings, 1);
			GUI.EndScrollView();
			if (GUI.Button(new Rect(30, 280, 100, 50), "Modalita'")){
				PlayerSaveData.playerData.SetCurrentPathName(selStrings[selPtInt]);
				//				SendMessage ("CreatePath", selStrings[selGridInt]);
				selectMode = true;
				selectPath = false;
			}
			if (GUI.Button(new Rect(220, 280, 100, 50), "Indietro")){
				selectName = true;
				selectPath = false;
			}
		}
		
		if(selectName){
			int count = users.Count;
			string[] selStrings = users.ToArray ();
			GUI.Label (new Rect((windowRect.width - 120)/2,20,200,25), "Seleziona giocatore");
			scrollPosition = GUI.BeginScrollView(new Rect(20, 50, 300, 220), scrollPosition, new Rect(0, 0, 280, 50*count));
			selUsInt = GUI.SelectionGrid(new Rect(0, 0, 250, 50*count), selUsInt, selStrings, 1);
			GUI.EndScrollView();
			if (GUI.Button(new Rect(30, 280, 100, 50), "Percorso")){
				//				PlayerSaveData.playerData.SetCurrentPathName(selStrings[selGridInt]);
				//				SendMessage ("CreatePath", selStrings[selGridInt]);
				PlayerSaveData.playerData.SetPlayer(GeneralSaveData.generalData.GetPlayer(selStrings[selUsInt]));
				PlayerSaveData.playerData.SetUserName(selStrings[selUsInt]);
				//SaveInfos.replay = false;
				selectName = false;
				selectPath = true;
			}
			if (GUI.Button(new Rect(220, 280, 100, 50), "Indietro")){
				load = false;
				menu = true;
			}
		}
		
		
	}
	
	public void EndMenu(){
		end = true;
	}
}
