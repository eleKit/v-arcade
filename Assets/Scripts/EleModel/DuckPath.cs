using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DuckPath
{
	public enum DuckSection
	{
		Front,
		Middle,
		Back,
	}

	;

	public const int N = 6;

	public DuckSection section;

	public bool[] ducks = new bool[N];

}
