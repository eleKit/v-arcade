using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour {

	public GUISkin customSkin;

	int selGridInt = 0;
	Rect windowRect;
	float width = 350;
	float height = 350f;
	string uName = "";
	string password = "";
	string confirm = "";
	string[] selStrings = new string[] {"Giocatore", "Fisioterapista"};
	bool menu = true;
	bool player, fisio, login, signup, fisioLogError, playerLogError, signError, passwError;
	bool exists = false;
	
	void Start(){
		windowRect = new Rect ((Screen.width-width)/2, (Screen.height-height)/2, width, height);
	}

	
	void OnGUI(){
		windowRect.x = (Screen.width-width)/2;
		windowRect.y = (Screen.height-height)/2;

		GUI.skin = customSkin;

		if(fisio)
			windowRect = GUI.Window (0, windowRect, FisioWindow, "Seleziona Gioco");
		if(menu)
			windowRect = GUI.Window (1, windowRect, MenuWindow, "Menu");
		if(player)
			windowRect = GUI.Window (2, windowRect, PlayerWindow, "Seleziona Gioco");
		if(login)
			windowRect = GUI.Window (3, windowRect, LoginWindow, "Accedi");
		if(signup)
			windowRect = GUI.Window (4, windowRect, SignupWindow, "Registrati");
	}
	
	void FisioWindow (int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 30, 210, 75), "Simulazione di volo")){
			SaveInfos.plane = true;
			SaveInfos.ski = false;
			SaveInfos.music = false;
			PlayerSaveData.playerData.FlightGame();
			Application.LoadLevel("Fisio_Plane");
		}
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 100, 210, 75), "Eroe della chitarra\n(Replay)")){
			SaveInfos.plane = false;
			SaveInfos.ski = false;
			SaveInfos.music = true;
			PlayerSaveData.playerData.MusicGame();
			Application.LoadLevel("Mandolin_Replay");
		}
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 170, 210, 75), "Sci")){
			SaveInfos.plane = false;
			SaveInfos.ski = true;
			SaveInfos.music = false;
			PlayerSaveData.playerData.SkiGame();
			Application.LoadLevel("Fisio_Ski");
		}
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 240, 210, 75), "Menu principale")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}
	
	void MenuWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 150)/2, 40, 150, 75), "Accedi")){
			menu = false;
			login = true;
		}
		if (GUI.Button(new Rect((windowRect.width - 150)/2, 140, 150, 75), "Registrati")){
			signup = true;
			menu = false;
		}
		if (GUI.Button(new Rect((windowRect.width - 150)/2, 240, 150, 75), "Esci")){
			Application.Quit();
		}
	}
	
	void PlayerWindow(int id){
		GUI.skin = customSkin;
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 30, 210, 75), "Simulazione di volo")){
			SaveInfos.plane = true;
			SaveInfos.ski = false;
			SaveInfos.music = false;
			PlayerSaveData.playerData.FlightGame();
			Application.LoadLevel("Player_Plane");
		}
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 100, 210, 75), "Eroe della chitarra")){
			SaveInfos.plane = false;
			SaveInfos.ski = false;
			SaveInfos.music = true;
			PlayerSaveData.playerData.MusicGame();
			Application.LoadLevel("Mandolin_Hero");
		}
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 170, 210, 75), "Sci")){
			SaveInfos.plane = false;
			SaveInfos.ski = true;
			SaveInfos.music = false;
			PlayerSaveData.playerData.SkiGame();
			Application.LoadLevel("Ski");
		}
		if (GUI.Button(new Rect((windowRect.width - 210)/2, 240, 210, 75), "Menu principale")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	void LoginWindow(int id){
		GUI.skin = customSkin;
		GUI.Label(new Rect(30,30,150f,30f), "Nome utente:");
		uName = GUI.TextField (new Rect(30,55,150f,20f),uName,20);
		if(fisioLogError)
			GUI.Label(new Rect(195,70,150f,50f), "Nome o\npassword errati.");
		if(playerLogError)
			GUI.Label(new Rect(195,70,150f,50f), "Nessun giocatore\ncon questo nome");

		GUI.Label(new Rect(30,130,150f,30f), "Seleziona:");
		selGridInt = GUI.SelectionGrid (new Rect (30, 155, 180, 70), selGridInt, selStrings, 1);
		if(selGridInt == 1){
			GUI.Label(new Rect(30,80,150f,30f), "Password:");
			password = GUI.PasswordField (new Rect(30,105,150f,20f),password,char.Parse ("*"));
		}
		
		if(GUI.Button (new Rect(30,270,150f,55f), "Accedi")){
			if(selGridInt == 0){
				fisioLogError = false;
				if(GeneralSaveData.generalData.CheckPlayerExists(uName)){
					playerLogError = false;
					PlayerSaveData.playerData.SetPlayer(GeneralSaveData.generalData.GetPlayer(uName));
					if(PlayerSaveData.playerData.GetFirstTimePlaying()){
						SendMessage("ChangeFocusToPlayerTuning");
						login = false;
					}
					else if(PlayerSaveData.playerData.GetTuningTooOld()){
						SendMessage("ChangeFocusToPlayerTuning");
						login = false;
					}
					else{
						login = false;
						player = true;
					}
				}
				else
					playerLogError = true;
			}
			if(selGridInt == 1){
				playerLogError = false;
				if(GeneralSaveData.generalData.CheckFisioExists(uName) && GeneralSaveData.generalData.CheckFisioCorrectPassword(uName, password)){
					fisioLogError = false;
					PlayerSaveData.playerData.SetUserName(uName);
					login = false;
					fisio = true;
				}
				else
					fisioLogError = true;
			}
			
		}
		if(GUI.Button (new Rect(170,270,150f,55f), "Annulla")){
			login = false;
			uName = "";
			password = "";
			menu = true;
		}
	}

	void SignupWindow(int id){
		GUI.skin = customSkin;
		GUI.Label(new Rect(30,30,150f,30f), "Nome utente:");
		uName = GUI.TextField (new Rect(30,55,150f,20f),uName,20);

		if(signError)
			GUI.Label(new Rect(195,70,150f,50f), "Nome gia'\nesistente");
		if(passwError)
			GUI.Label(new Rect(195,70,150f,50f), "Le password\nnon coincidono");

		GUI.Label(new Rect(30,175,150f,30f), "Seleziona:");
		selGridInt = GUI.SelectionGrid (new Rect (30, 195, 180, 70), selGridInt, selStrings, 1);
		if(selGridInt == 1){
			GUI.Label(new Rect(30,80,150f,30f), "Password:");
			password = GUI.PasswordField (new Rect(30,105,150f,20f),password,char.Parse ("*"));
			GUI.Label(new Rect(30,130,200f,30f), "Conferma password:");
			confirm = GUI.PasswordField (new Rect(30,155,150f,20f),confirm,char.Parse ("*"));
		}
		
		if(GUI.Button (new Rect(30,270,150f,55f), "Registrati")){
			if(selGridInt == 0){
				passwError = false;
				if(GeneralSaveData.generalData.CheckPlayerExists(uName))
					signError = true;
				else{
					signError = false;
					GeneralSaveData.generalData.AddPlayerInfos(uName, password);
					PlayerSaveData.playerData.SetUserName(uName);
					PlayerSaveData.playerData.NewPlayer(uName);

					signup = false;
					SendMessage("ChangeFocusToPlayerTuning");
				}
			}
			if(selGridInt == 1){
				if(!password.Equals(confirm))
					passwError = true;
				else{
					passwError = false;
					if(GeneralSaveData.generalData.CheckFisioExists(uName))
						signError = true;
					else{
						signError = false;
						GeneralSaveData.generalData.AddFisioInfos(uName,password);
						PlayerSaveData.playerData.SetUserName(uName);
						signup = false;
						fisio = true;
					}
				}
			}
		}
		if(GUI.Button (new Rect(170,270,150f,55f), "Annulla")){
			menu = true;
			signup = false; 
			uName = "";
			password = "";
			confirm = "";
		}
	}

	public void ShowPlayer(){
		player = true;
	}
}
