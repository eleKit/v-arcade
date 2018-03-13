using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class PlayerPlaneMenuGUI : MonoBehaviour
{

	public GUISkin customSkin;
	
	Vector2 scrollPosition = Vector2.zero;
	int selGridInt = 0;
	int velGridInt = 1;
	int handModeInt = 0;
	int handInt = 0;
	int openInt = 0;
	Rect windowRect;
	float width = 350;
	float height = 350f;
	string pathName = "";
	bool menu = true;
	bool pause, load, setup, end;
	bool twoHands = false;
	List<string> pathNames;

	void Start ()
	{
		windowRect = new Rect ((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);
		pathNames = PathSaveData.pathData.GetPathNames ();
	}

	void Update ()
	{
		if (Input.GetKeyDown ("escape") && !pause) {
			pause = true;
			SendMessage ("Pause", true);
		}
	}

	void OnGUI ()
	{
		windowRect.x = (Screen.width - width) / 2;
		windowRect.y = (Screen.height - height) / 2;
		
		GUI.skin = customSkin;
		if (end)
			windowRect = GUI.Window (0, windowRect, EndWindow, "Finito");
		if (menu)
			windowRect = GUI.Window (1, windowRect, MenuWindow, "Ciao, " + PlayerSaveData.playerData.GetUserName () + "!");
		if (pause)
			windowRect = GUI.Window (2, windowRect, PauseWindow, "Pausa");
		if (load)
			windowRect = GUI.Window (3, windowRect, LoadWindow, "Carica Percorso");
		if (setup)
			windowRect = GUI.Window (4, windowRect, SetupWindow, "Impostazioni");
	}

	
	void MenuWindow (int id)
	{
		GUI.skin = customSkin;
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 80, 210, 75), "Seleziona Percorso")) {
			menu = false;
			load = true;
		}
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 180, 210, 75), "Menu principale")) {
			SceneManager.LoadSceneAsync ("Main_Menu");
		}
	}

	void SetupWindow (int id)
	{
		GUI.skin = customSkin;
		string[] velStrings = new string[]{ "bassa", "media", "alta" };
		string[] modeStrings = new string[]{ "Una mano", "Due mani" };
		string[] handStrings = new string[]{ "Sinistra", "Destra" };
		string[] openStrings = new string[]{ "Aperta", "Chiusa" };
		GUI.Label (new Rect (30, 30, 120, 25), "Velocita' aereo");
		velGridInt = GUI.SelectionGrid (new Rect (25, 50, 300, 40), velGridInt, velStrings, 3);
		GUI.Label (new Rect (30, 90, 120, 25), "Numero mani");
		handModeInt = GUI.SelectionGrid (new Rect (25, 110, 300, 40), handModeInt, modeStrings, 2);
		GUI.Label (new Rect (30, 150, 250, 25), "Mano aperta o chiusa");
		openInt = GUI.SelectionGrid (new Rect (25, 170, 300, 40), openInt, openStrings, 2);
		if (!twoHands) {
			GUI.Label (new Rect (30, 205, 250, 25), "Seleziona mano");
			handInt = GUI.SelectionGrid (new Rect (25, 225, 300, 40), handInt, handStrings, 2);
		}

		if (handModeInt == 1)
			twoHands = true;
		else
			twoHands = false;
		
		if (GUI.Button (new Rect (20, 265, 150, 65), "Inizia")) {
			PlayerSaveData.playerData.SetPlaneSpeed (velGridInt + 1);
			PlayerSaveData.playerData.SetOpenHandMode (openInt == 0);
			if (twoHands) {
				PlayerSaveData.playerData.SetOneHandMode (false);
				PlayerSaveData.playerData.SetRightHand (handInt == 1);
			} else {
				PlayerSaveData.playerData.SetOneHandMode (true);
				PlayerSaveData.playerData.SetRightHand (handInt == 1);
			}
			setup = false;
			SendMessage ("Begin");
		}
		if (GUI.Button (new Rect (180, 265, 150, 65), "Torna al menu")) {
			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().buildIndex);
		}
	}

	void PauseWindow (int id)
	{
		GUI.skin = customSkin;
		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 30, 200, 75), "Riprendi")) {
			pause = false;
			SendMessage ("Pause", false);
		}
		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 100, 200, 75), "Ricomincia")) {
			pause = false;
			SendMessage ("ResetPath");
		}
		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 170, 200, 75), "Torna al menu")) {
			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().buildIndex);
		}

		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 240, 200, 75), "Salva (Testing)")) {
			SendMessage ("SaveToXML");
		}
	}

	void EndWindow (int id)
	{
		GUI.skin = customSkin;
		GUI.Label (new Rect (30, 40, 200, 25), "Punteggio");
		GUI.Label (new Rect (180, 40, 50, 25), PlayerSaveData.playerData.GetScore ().ToString ());
		GUI.Label (new Rect (30, 80, 205, 25), "Il Tuo Punteggio Migliore");
		GUI.Label (new Rect (240, 80, 50, 25), PlayerSaveData.playerData.GetHighscore ().ToString ());
		GUI.Label (new Rect (30, 120, 200, 25), "Il Punteggio Migliore");
		GUI.Label (new Rect (30, 150, 200, 25), PathSaveData.pathData.GetBestPlayer (PlayerSaveData.playerData.GetCurrentPathName ()));
		GUI.Label (new Rect (180, 150, 200, 25), PathSaveData.pathData.GetPathHighscore (PlayerSaveData.playerData.GetCurrentPathName ()).ToString ());

		if (GUI.Button (new Rect (20f, 180, 150, 75), "Ricomincia")) {
			SendMessage ("ResetPath");
			end = false;
		}
		if (GUI.Button (new Rect (180, 180, 150, 75), "Torna al menu")) {
			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().buildIndex);
		}
		
		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 260, 200, 75), "Salva (Testing)")) {
			SendMessage ("SaveToXML");
		}
	}

	void LoadWindow (int id)
	{
		GUI.skin = customSkin;
		int count = pathNames.Count;
		string[] selStrings = pathNames.ToArray ();
		GUI.Label (new Rect ((windowRect.width - 120) / 2, 20, 200, 25), "Seleziona il percorso");
		scrollPosition = GUI.BeginScrollView (new Rect (20, 50, 300, 220), scrollPosition, new Rect (0, 0, 280, 50 * count));
		selGridInt = GUI.SelectionGrid (new Rect (0, 0, 250, 50 * count), selGridInt, selStrings, 1);
		GUI.EndScrollView ();
		if (GUI.Button (new Rect (30, 280, 100, 50), "Carica")) {
			PlayerSaveData.playerData.SetCurrentPathName (selStrings [selGridInt]);
			SendMessage ("CreatePath", selStrings [selGridInt]);
			load = false;
			setup = true;
		}
		if (GUI.Button (new Rect (220, 280, 100, 50), "Indietro")) {
			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().buildIndex);
		}
	}

	void EndMenu ()
	{
		end = true;
	}
}
