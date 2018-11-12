using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SpacePath
{
	public SpaceSection[] space_sections;

	public float curve_amplitude;

	public string id_path;

	public long timestamp;

	public string doctorName;

	//false if this is a training generated level
	public bool standard;

	public const float Y_OFFSET_COORD = 10f;
	//the enemy must start from upper than the top of the scene

	//standard levels attribute
	public SpaceStandard standard_model;
}
