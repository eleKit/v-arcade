using UnityEngine;
using System.Collections;

public class SkiEditorGUI : MonoBehaviour {

	public GUISkin customSkin;

	Rect windowRect;
	float width = 350;
	float height = 350f;
	bool save, exists;
	string pathName = "";

	// Use this for initialization
	void Start () {
		windowRect = new Rect ((Screen.width-width)/2, (Screen.height-height)/2, width, height);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		GUI.skin = customSkin;
		windowRect.x = (Screen.width-width)/2;
		windowRect.y = (Screen.height-height)/2;
		//windowRect.width = Screen.width*width;
		//windowRect.height = Screen.height*height;

		if (GUI.Button(new Rect(Screen.width/3, Screen.height * 6/7, 100, 50), "Salva")){
			save = true;
		}

		if (GUI.Button(new Rect(Screen.width * 4/7, Screen.height * 6/7, 100, 50), "Indietro")){
			Application.LoadLevel("Fisio_ski");
		}
		
		GUI.skin = customSkin;
		if(save)
			windowRect = GUI.Window (0, windowRect, SaveWindow, "Salvataggio");

	}
	
	void SaveWindow (int id){
		GUI.skin = customSkin;
		GUI.Label (new Rect(30,50,250,25), "Nome del percorso:");
		pathName = GUI.TextField(new Rect(30, 80 , 190f, 25f),pathName,20);
		if (GUI.Button(new Rect(20, 250, 100, 50), "Salva")){
			SkiSaveData.skiData.InitializePath ();
			SkiSaveData.skiData.SetCurrentPathName(pathName);
			if(!SkiSaveData.skiData.CheckPathName(pathName)){
				SendMessage("PathSetup");
				exists = false;
				SkiSaveData.skiData.SaveSkiPaths();
				Debug.Log ("Percorso salvato");
				save = false;
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
		}
	}
}
