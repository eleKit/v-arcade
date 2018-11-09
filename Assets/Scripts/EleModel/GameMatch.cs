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

	public enum LevelType
	{
		Training,
		Standard}

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

	public LevelType levelType;

	public long timestamp;

	public string patientName;

	public string id_path;

	public float left_yaw_scale;
	public float right_yaw_scale;

	public float left_pitch_scale;
	public float right_pitch_scale;

}



