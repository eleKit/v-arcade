﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DuckSection
{

	public const int N = 6;

	public enum DuckGameSection
	{
		Front,
		Middle,
		Back,
	}

	;



	public DuckGameSection section;

	public bool[] ducks = new bool[N];

	public Vector3[] back_coord = new Vector3[N] {
		new Vector3 (-6f, 3f, 0f), new Vector3 (-3.8f, 3.5f, 0f), 
		new Vector3 (-1.65f, 3f, 0f), new Vector3 (1.65f, 3.5f, 0f),
		new Vector3 (3.8f, 3f, 0f), new Vector3 (6f, 3.5f, 0f)
	};

	public const float MIDDLE_y_coord = 2.5f;

	public const float FRONT_y_coord = 4.5f;
}