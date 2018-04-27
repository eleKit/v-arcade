using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour
{

	public GUISkin customSkin;

	bool menu, random, load_path, pause, win;

	//GUI settings
	Vector2 scrollPosition = Vector2.zero;
	int selGridInt = 0;
	int modGridInt = 0;
	Rect windowRect;
	float width = 350;
	float height = 350f;



	int handInt = 0;
	int movInt = 0;
	int obsInt = 0;
	int diffInt = 1;
	int velInt = 1;

	// Use this for initialization
	void Start ()
	{
		windowRect = new Rect ((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);
		menu = true;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown ("escape") && !pause) {
			pause = true;
			//TODO i messaggi devono essere ricevuti da il game manager
			//SendMessage ("PauseCarGame");
		
		}
	}

	void OnGUI ()
	{
		windowRect.x = (Screen.width - width) / 2;
		windowRect.y = (Screen.height - height) / 2;

		GUI.skin = customSkin;

		if (menu)
			windowRect = GUI.Window (0, windowRect, MenuWindow, "Menu Principale");
		if (win)
			windowRect = GUI.Window (1, windowRect, EndWindow, "Finito");
		if (pause)
			windowRect = GUI.Window (2, windowRect, PauseWindow, "Pausa");
		if (random)
			windowRect = GUI.Window (3, windowRect, RandomWindow, "Random Menu");
		

	}


	void MenuWindow (int id)
	{
		GUI.skin = customSkin;
		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 40, 200, 75), "Percorso casuale")) {
			menu = false;
			random = true;
		}
		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 140, 200, 75), "Percorsi Salvati")) {
			// pathNames = get path names List<sting> from saved path 
			load_path = true;
			menu = false;
		}
		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 240, 200, 75), "Menu principale")) {
			SceneManager.LoadSceneAsync ("Main_Menu");
		}
	}


	void EndWindow (int id)
	{
		GUI.skin = customSkin;
		GUI.Label (new Rect (30, 40, 200, 25), "Punteggio:");
		GUI.Label (new Rect (180, 40, 50, 25), "Na"); // TODO CarGameManagerScript.Instance.getScore().ToString());
		GUI.Label (new Rect (30, 80, 250, 25), "Il Tuo Punteggio Migliore:");
		GUI.Label (new Rect (280, 80, 50, 25), "Na"); //TODO caricare da file 

		if (GUI.Button (new Rect (20f, 180, 150, 75), "Ricomincia")) {
			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().buildIndex);
		}
		if (GUI.Button (new Rect (180, 180, 150, 75), "Torna al menu")) {
			SceneManager.LoadSceneAsync ("Main_Menu");

		}
	}

	void PauseWindow (int id)
	{
		GUI.skin = customSkin;
		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 30, 200, 75), "Continua")) {
			pause = false;
			//SendMessage ("UnPause");
		}
		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 100, 200, 75), "Ricomincia")) {
			pause = false;
			//SendMessage ("Reload");
		}
		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 170, 200, 75), "Scegli livello")) {
			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().buildIndex);
		}

		if (GUI.Button (new Rect ((windowRect.width - 200) / 2, 240, 200, 75), "Salva e interrompi")) {
			//TODO salvare
			SceneManager.LoadSceneAsync ("Main_Menu");
		}
	}


	void RandomWindow (int id)
	{
		GUI.skin = customSkin;
		string[] modeStrings = new string[]{ "180°", "90°" };
		string[] handStrings = new string[]{ "Sinistra", "Destra" };
		string[] diffStrings = new string[]{ "Facile", "Medio", "Difficile" };
		string[] velStrings = new string[]{ "Lenta", "Media", "Veloce" };


		GUI.Label (new Rect (30, 30, 250, 20), new GUIContent ("Seleziona difficolta'", "La difficolta' influisce sull'ampiezza del movimento da effettuare."));
		diffInt = GUI.SelectionGrid (new Rect (25, 50, 300, 30), diffInt, diffStrings, 3);

		GUI.Label (new Rect (30, 90, 200, 20), "Seleziona mano");
		handInt = GUI.SelectionGrid (new Rect (25, 110, 300, 30), handInt, handStrings, 2);

	
		GUI.Label (new Rect (30, 150, 200, 20), "Inclinazione mano");
		modGridInt = GUI.SelectionGrid (new Rect (25, 170, 300, 30), modGridInt, modeStrings, 2);

		GUI.Label (new Rect (30, 200, 310, 20), "Accelerazione massima dell'automobile");
		velInt = GUI.SelectionGrid (new Rect (25, 215, 300, 30), velInt, velStrings, 3);

	
		GUI.Label (new Rect (30, 220, 300, 100), GUI.tooltip);


		if (GUI.Button (new Rect (20, 265, 150, 65), "Inizia")) {
			random = false;
			//SendMessage("Start");
		}
		if (GUI.Button (new Rect (180, 265, 150, 65), "Torna al menu")) {
			Application.LoadLevel ("Menu");
		}
		
	}













}
