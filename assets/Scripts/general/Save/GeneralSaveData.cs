using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;

public class GeneralSaveData : MonoBehaviour
{

	public static GeneralSaveData generalData;

	List<LoginInfo> fisioLogIn = new List<LoginInfo> ();
	List<LoginInfo> playerLogIn = new List<LoginInfo> ();
	List<PlayerInfos> players = new List<PlayerInfos> ();

	
	void Awake ()
	{
		if (generalData == null) {
			DontDestroyOnLoad (gameObject);
			generalData = this;

			// loads the login info of all the therapist in the list<LoginInfo> fisioLogIn
			LoadFisioLoginInfos ();

			// loads the login info of all the players in the list<LoginInfo> playerLogIn
			// players credentials contains only usernames
			LoadPlayerLoginInfos ();

			// loads the list of PlayerInfo containing highscores and tuning
			LoadPlayers ();

		} else if (generalData != this) {
			Destroy (gameObject);
		}
	}


	// Therapist Logon: save new  info for therapist, called by LoginMenu

	public void AddFisioInfos (string n, string p)
	{
		LoginInfo lo = new LoginInfo (n, p);
		fisioLogIn.Add (lo);
		SaveFisioLoginInfos ();
	}


	public void SaveFisioLoginInfos ()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/fcred.dat");
		bf.Serialize (file, fisioLogIn);
		file.Close ();
		LoadFisioLoginInfos ();
	}

	// end Therapist Logon

	public void LoadFisioLoginInfos ()
	{
		if (File.Exists (Application.persistentDataPath + "/fcred.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/fcred.dat", FileMode.Open);
			fisioLogIn = (List<LoginInfo>)bf.Deserialize (file);
			file.Close ();
		} else
			fisioLogIn = new List<LoginInfo> ();
	}

	// Player Logon: save new username of player, called by LoginMenu

	public void AddPlayerInfos (string n, string p)
	{
		LoginInfo lo = new LoginInfo (n, p);
		playerLogIn.Add (lo);
		SavePlayerLoginInfos ();
	}


	public void SavePlayerLoginInfos ()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/pcred.dat");
		bf.Serialize (file, playerLogIn);
		file.Close ();
		LoadPlayerLoginInfos ();
	}

	// end Player Logon

	public void LoadPlayerLoginInfos ()
	{
		if (File.Exists (Application.persistentDataPath + "/pcred.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/pcred.dat", FileMode.Open);
			playerLogIn = (List<LoginInfo>)bf.Deserialize (file);
			file.Close ();
		} else
			playerLogIn = new List<LoginInfo> ();
		;
	}


	// save player info as highscore, tuning, ...

	public void SavePlayers ()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/players.dat");
		bf.Serialize (file, players);
		file.Close ();
		LoadPlayerLoginInfos ();
	}

	public void LoadPlayers ()
	{
		if (File.Exists (Application.persistentDataPath + "/players.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/players.dat", FileMode.Open);
			players = (List<PlayerInfos>)bf.Deserialize (file);
			file.Close ();
		} else
			players = new List<PlayerInfos> ();
	}


	/*** all these functions are used by LoginMenu ***/

	public bool CheckFisioExists (string n)
	{
		Debug.Log (fisioLogIn.Count);
		if (fisioLogIn.Count > 0) {
			if (fisioLogIn.Exists (x => x.userName.Equals (n)))
				return true;
			else
				return false;
		} else
			return false;
	}

	public bool CheckFisioCorrectPassword (string n, string p)
	{
		if (CheckFisioExists (n)) {
			if (fisioLogIn.Find (x => x.userName.Equals (n)).password.Equals (p))
				return true;
			else
				return false;
		} else
			return false;
	}


	public bool CheckPlayerExists (string n)
	{
		if (playerLogIn.Count > 0) {
			if (playerLogIn.Exists (x => x.userName.Equals (n)))
				return true;
			else
				return false;
		} else
			return false;
	}

	//TODO player password is not used
	public bool CheckPlayerCorrectPassword (string n, string p)
	{
		if (CheckPlayerExists (n)) {
			if (playerLogIn.Find (x => x.userName.Equals (n)).password.Equals (p))
				return true;
			else
				return false;
		} else
			return false;
	}

	public PlayerInfos GetPlayer (string uName)
	{
		return players.Find (x => x.userName.Equals (uName));
	}

	public void AddPlayer (PlayerInfos pl)
	{
		players.Add (pl);
		SavePlayers ();
	}

	public void UpdatePlayerTunings (string uName, float minLH, float maxLH, float minLV, float maxLV, float minRH, float maxRH, float minRV, float maxRV)
	{
		players.Find (x => x.userName.Equals (uName)).tunings = new PlayerTunings (minLH, maxLH, minLV, maxLV, minRH, maxRH, minRV, maxRV);
		SavePlayers ();
	}

	public void UpdatePlayerPlaneHighscore (string player, string path, int score)
	{
		if (players.Exists (x => x.userName.Equals (player))) {
			if (players.Find (x => x.userName.Equals (player)).planeHighscores.Exists (x => x.pathName.Equals (path))) {
				if (players.Find (x => x.userName.Equals (player)).planeHighscores.Find (x => x.pathName.Equals (path)).score < score)
					players.Find (x => x.userName.Equals (player)).planeHighscores.Find (x => x.pathName.Equals (path)).score = score;
			} else {
				Highscore hs = new Highscore ();
				hs.pathName = path;
				hs.score = score;
				players.Find (x => x.userName.Equals (player)).planeHighscores.Add (hs);
				SavePlayers ();
			}
		}
	}

	public void UpdatePlayerMusicHighscore (string player, string song, int score)
	{
		if (players.Exists (x => x.userName.Equals (player))) {
			if (players.Find (x => x.userName.Equals (player)).musicHighscores.Exists (x => x.pathName.Equals (song))) {
				if (players.Find (x => x.userName.Equals (player)).musicHighscores.Find (x => x.pathName.Equals (song)).score < score)
					players.Find (x => x.userName.Equals (player)).musicHighscores.Find (x => x.pathName.Equals (song)).score = score;
			} else {
				Highscore hs = new Highscore ();
				hs.pathName = song;
				hs.score = score;
				players.Find (x => x.userName.Equals (player)).musicHighscores.Add (hs);
				SavePlayers ();
			}
		}
	}

	public void UpdatePlayerSkiHighscore (string player, string path, int score)
	{
		if (players.Exists (x => x.userName.Equals (player))) {
			if (players.Find (x => x.userName.Equals (player)).skiHighscores.Exists (x => x.pathName.Equals (path))) {
				if (players.Find (x => x.userName.Equals (player)).skiHighscores.Find (x => x.pathName.Equals (path)).score < score)
					players.Find (x => x.userName.Equals (player)).skiHighscores.Find (x => x.pathName.Equals (path)).score = score;
			} else {
				Highscore hs = new Highscore ();
				hs.pathName = path;
				hs.score = score;
				players.Find (x => x.userName.Equals (player)).skiHighscores.Add (hs);
				SavePlayers ();
			}
		}
	}

}

[Serializable]
class LoginInfo
{
	public string userName;
	public string password;

	public LoginInfo (string na, string pw)
	{
		userName = na;
		password = pw;
	}
}

[Serializable]
public class PlayerInfos
{
	
	public string userName;
	public List<Highscore> planeHighscores;
	public List<Highscore> musicHighscores;
	public List<Highscore> skiHighscores;
	public PlayerTunings tunings;

	public PlayerInfos ()
	{
		userName = "";
		planeHighscores = new List<Highscore> ();
		musicHighscores = new List<Highscore> ();
		skiHighscores = new List<Highscore> ();
		tunings = new PlayerTunings ();
	}

	public PlayerInfos (string na)
	{
		userName = na;
		planeHighscores = new List<Highscore> ();
		musicHighscores = new List<Highscore> ();
		skiHighscores = new List<Highscore> ();
		tunings = new PlayerTunings ();
	}
}

[Serializable]
public class Highscore
{
	public string pathName;
	public int score;
}

[Serializable]
public class PlayerTunings
{
	public float minLeftHorizontal;
	public float maxLeftHorizontal;
	public float minLeftVertical;
	public float maxLeftVertical;
	public float minRightHorizontal;
	public float maxRightHorizontal;
	public float minRightVertical;
	public float maxRightVertical;
	public bool hasTunings;
	public DateTime lastUpdate;

	public PlayerTunings ()
	{
	}

	public PlayerTunings (float minLH, float maxLH, float minLV, float maxLV,
	                      float minRH, float maxRH, float minRV, float maxRV)
	{
		minLeftHorizontal = minLH;
		maxLeftHorizontal = maxLH;
		minLeftVertical = minLV;
		maxLeftVertical = maxLV;
		minRightHorizontal = minRH;
		maxRightHorizontal = maxRH;
		minRightVertical = minRV;
		maxRightVertical = maxRV;
		hasTunings = true;
		lastUpdate = DateTime.Now;
	}
}
