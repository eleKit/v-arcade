using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameMatch
{
	public enum GameType
	{
		Shooting,
		Car,
		Music,
	};

	public GameType gameType;

	public long timestamp;

	public string patientName;

	public string id_path;

	public int score;

}



