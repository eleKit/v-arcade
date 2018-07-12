using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SpaceSection
{
	public const int M = 7;

	public enum SpaceGameSection
	{
		Front,
		Middle,
		Back,
	}

	;


	public SpaceGameSection section;

	public bool[] enemies = new bool[M];

	public Vector3[] front_enemies_coord = new Vector3[M] { 
		new Vector3 (-15f, 0f, 0f), 
		new Vector3 (-10f, 0f, 0f), 
		new Vector3 (-5f, 0f, 0f), 
		new Vector3 (0f, 0f, 0f), 
		new Vector3 (5f, 0f, 0f), 
		new Vector3 (10f, 0f, 0f), 
		new Vector3 (15f, 0f, 0f), 
	};
		
	/*In the space game the aliens must go down from the upper corner
	 * front -> y=10 = Y_OFFSET_COORD * 2
	 * middle -> y=15 = Y_OFFSET_COORD * 3
	 * back -> y=20 = Y_OFFSET_COORD * 4
	 */

	public const float Y_OFFSET_COORD = 5f;
}
