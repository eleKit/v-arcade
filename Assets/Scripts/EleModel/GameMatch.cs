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
		Space}

	;


	public enum HandAngle
	{
		One_hundred,
		Ninety,
		Roll,
		None}

	;

	public GameType gameType;

	public HandAngle handAngle;

	public long timestamp;

	public string patientName;

	public string id_path;

	public int score;

}



