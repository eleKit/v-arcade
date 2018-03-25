using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginMenu : MonoBehaviour
{

	/* MENU VALID FOR THERAPIST AND FOR PLAYER
	 */


	public GUISkin customSkin;




	// boolean to choose the login or signup menu
	bool login, signup, account;

	//boolean true if an error has to be shown
	bool fisioLogError, playerLogError, signError, passwError;

	int selGridInt = 0;
	Rect windowRect;
	float width = 350;
	float height = 350f;
	string uName = "";
	string password = "";
	string confirm = "";
	string[] selStrings = new string[] { "Giocatore", "Fisioterapista" };
	bool exists = false;

	void Start ()
	{
		windowRect = new Rect ((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);
	}

	void OnGUI ()
	{
		windowRect.x = (Screen.width - width) / 2;
		windowRect.y = (Screen.height - height) / 2;

		GUI.skin = customSkin;

		if (BoolMenu.boolMenu.fisio)
			windowRect = GUI.Window (0, windowRect, FisioWindow, "Seleziona Gioco");
		if (BoolMenu.boolMenu.menu)
			windowRect = GUI.Window (1, windowRect, MenuWindow, "Menu");
		if (BoolMenu.boolMenu.player)
			windowRect = GUI.Window (2, windowRect, PlayerWindow, "Seleziona Gioco");
		if (login)
			windowRect = GUI.Window (3, windowRect, MyLoginWindow, "Accedi");
		if (signup)
			windowRect = GUI.Window (4, windowRect, SignupWindow, "Registrati");
		if (account)
			windowRect = GUI.Window (5, windowRect, AccountWindow, "Visualizza Account");
	}


	void AccountWindow (int id)
	{
		GUI.skin = customSkin;
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 30, 210, 75), "Cancella Account")) {
			//TODO funzione che cancella l'account
		}
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 100, 210, 75), "Menu principale")) {
			uName = "";
			password = "";
			account = false;
			BoolMenu.boolMenu.menu = true;
		}
	}


	void FisioWindow (int id)
	{
		GUI.skin = customSkin;
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 30, 210, 55), "Simulazione di volo")) {
			SaveInfos.plane = true;
			SaveInfos.ski = false;
			SaveInfos.music = false;
			PlayerSaveData.playerData.FlightGame ();
			SceneManager.LoadSceneAsync ("Fisio_Plane");
		}
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 90, 210, 55), "Eroe della chitarra\n(Replay)")) {
			SaveInfos.plane = false;
			SaveInfos.ski = false;
			SaveInfos.music = true;
			PlayerSaveData.playerData.MusicGame ();
			SceneManager.LoadSceneAsync ("Mandolin_Replay");
		}
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 150, 210, 55), "Sci")) {
			SaveInfos.plane = false;
			SaveInfos.ski = true;
			SaveInfos.music = false;
			PlayerSaveData.playerData.SkiGame ();
			SceneManager.LoadSceneAsync ("Fisio_Ski");
		}
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 210, 210, 55), "Visualizza Account")) {
			BoolMenu.boolMenu.fisio = false;
			account = true;
		}
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 270, 210, 55), "Menu principale")) {
			// TODO here there must be the logout
			uName = "";
			password = "";
			BoolMenu.boolMenu.fisio = false;
			BoolMenu.boolMenu.menu = true;
		}
	}

	void MenuWindow (int id)
	{
		GUI.skin = customSkin;
		if (GUI.Button (new Rect ((windowRect.width - 150) / 2, 40, 150, 75), "Accedi")) {
			BoolMenu.boolMenu.menu = false;
			login = true;
		}
		if (GUI.Button (new Rect ((windowRect.width - 150) / 2, 140, 150, 75), "Registrati")) {
			signup = true;
			BoolMenu.boolMenu.menu = false;
		}
		if (GUI.Button (new Rect ((windowRect.width - 150) / 2, 240, 150, 75), "Esci")) {
			Application.Quit ();
		}
	}


	void PlayerWindow (int id)
	{
		GUI.skin = customSkin;
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 30, 210, 75), "Simulazione di volo")) {
			SaveInfos.plane = true;
			SaveInfos.ski = false;
			SaveInfos.music = false;
			PlayerSaveData.playerData.FlightGame ();
			SceneManager.LoadSceneAsync ("Player_Plane");
		}
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 100, 210, 75), "Eroe della chitarra")) {
			SaveInfos.plane = false;
			SaveInfos.ski = false;
			SaveInfos.music = true;
			PlayerSaveData.playerData.MusicGame ();
			SceneManager.LoadSceneAsync ("Mandolin_Hero");
		}
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 170, 210, 75), "Sci")) {
			SaveInfos.plane = false;
			SaveInfos.ski = true;
			SaveInfos.music = false;
			PlayerSaveData.playerData.SkiGame ();
			SceneManager.LoadSceneAsync ("Ski");
		}
		if (GUI.Button (new Rect ((windowRect.width - 210) / 2, 240, 210, 75), "Menu principale")) {
			// TODO here there must be the logout
			uName = "";
			password = "";
			BoolMenu.boolMenu.player = false;
			BoolMenu.boolMenu.menu = true;
		}
	}

	/* TODO ho cambiato il nome rispetto a quello deciso da Rocco che era LoginWindow 
	 * a causa di una funzione di libreria che aveva lo stesso nome!! 
	 */

	void MyLoginWindow (int id)
	{
		GUI.skin = customSkin;
		GUI.Label (new Rect (30, 30, 150f, 30f), "Nome utente:");
		uName = GUI.TextField (new Rect (30, 55, 150f, 20f), uName, 20);
		if (fisioLogError)
			GUI.Label (new Rect (195, 70, 150f, 50f), "Nome o\npassword errati.");
		if (playerLogError)
			GUI.Label (new Rect (195, 70, 150f, 50f), "Nessun giocatore\ncon questo nome");

		GUI.Label (new Rect (30, 130, 150f, 30f), "Seleziona:");


		//in this grid the buttons "giocatore" and "fisioterapista" are shown and each of them correspond to an ID int number 
		selGridInt = GUI.SelectionGrid (new Rect (30, 155, 180, 70), selGridInt, selStrings, 1);


		/* if the button selected has ID int number == 1 it corresponds to the "fisioterapista", 
		 * otherwise the ID int is == 0 and it corresponds to "giocatore 
		*/
		if (selGridInt == 1) {
			GUI.Label (new Rect (30, 80, 150f, 30f), "Password:");
			password = GUI.PasswordField (new Rect (30, 105, 150f, 20f), password, char.Parse ("*"));
		}

		if (GUI.Button (new Rect (30, 270, 150f, 55f), "Accedi")) {
			if (selGridInt == 0) {
				fisioLogError = false;
				if (GeneralSaveData.generalData.CheckPlayerExists (uName)) {
					playerLogError = false;
					PlayerSaveData.playerData.SetPlayer (GeneralSaveData.generalData.GetPlayer (uName));
					if (PlayerSaveData.playerData.GetFirstTimePlaying ()) {
						login = false;
						BoolMenu.boolMenu.player = true;
						SceneManager.LoadSceneAsync ("Tuning");
					} else if (PlayerSaveData.playerData.GetTuningTooOld ()) {
						login = false;
						BoolMenu.boolMenu.player = true;
						SceneManager.LoadSceneAsync ("Tuning");
					} else {

						/* if the player has done the tuning it is shown to him the PlayerWindow*/

						login = false;
						BoolMenu.boolMenu.player = true;
					}
				} else
					playerLogError = true;
			}
			if (selGridInt == 1) {
				playerLogError = false;
				if (GeneralSaveData.generalData.CheckFisioExists (uName) && GeneralSaveData.generalData.CheckFisioCorrectPassword (uName, password)) {
					fisioLogError = false;
					PlayerSaveData.playerData.SetUserName (uName);
					login = false;
					BoolMenu.boolMenu.fisio = true;
				} else
					fisioLogError = true;
			}

		}
		if (GUI.Button (new Rect (170, 270, 150f, 55f), "Annulla")) {
			login = false;
			uName = "";
			password = "";
			BoolMenu.boolMenu.menu = true;
		}
	}


	void SignupWindow (int id)
	{
		GUI.skin = customSkin;
		GUI.Label (new Rect (30, 30, 150f, 30f), "Nome utente:");
		uName = GUI.TextField (new Rect (30, 55, 150f, 20f), uName, 20);

		if (signError)
			GUI.Label (new Rect (195, 70, 150f, 50f), "Nome gia'\nesistente");
		if (passwError)
			GUI.Label (new Rect (195, 70, 150f, 50f), "Le password\nnon coincidono");

		GUI.Label (new Rect (30, 175, 150f, 30f), "Seleziona:");
		selGridInt = GUI.SelectionGrid (new Rect (30, 195, 180, 70), selGridInt, selStrings, 1);
		if (selGridInt == 1) {
			GUI.Label (new Rect (30, 80, 150f, 30f), "Password:");
			password = GUI.PasswordField (new Rect (30, 105, 150f, 20f), password, char.Parse ("*"));
			GUI.Label (new Rect (30, 130, 200f, 30f), "Conferma password:");
			confirm = GUI.PasswordField (new Rect (30, 155, 150f, 20f), confirm, char.Parse ("*"));
		}

		if (GUI.Button (new Rect (30, 270, 150f, 55f), "Registrati")) {
			if (selGridInt == 0) {
				passwError = false;
				if (GeneralSaveData.generalData.CheckPlayerExists (uName))
					signError = true;
				else {
					signError = false;
					GeneralSaveData.generalData.AddPlayerInfos (uName, password);
					PlayerSaveData.playerData.SetUserName (uName);
					PlayerSaveData.playerData.NewPlayer (uName);

					signup = false;
					BoolMenu.boolMenu.player = true;
					SceneManager.LoadSceneAsync ("Tuning");
				}
			}
			if (selGridInt == 1) {
				if (!password.Equals (confirm))
					passwError = true;
				else {
					passwError = false;
					if (GeneralSaveData.generalData.CheckFisioExists (uName))
						signError = true;
					else {
						signError = false;
						GeneralSaveData.generalData.AddFisioInfos (uName, password);
						PlayerSaveData.playerData.SetUserName (uName);
						signup = false;
						BoolMenu.boolMenu.fisio = true;
					}
				}
			}
		}
		if (GUI.Button (new Rect (170, 270, 150f, 55f), "Annulla")) {
			BoolMenu.boolMenu.menu = true;
			signup = false; 
			uName = "";
			password = "";
			confirm = "";
		}
	}

	void resetBool ()
	{
		BoolMenu.boolMenu.player = false;
		BoolMenu.boolMenu.fisio = false;
		BoolMenu.boolMenu.menu = false;
	}
		



}
