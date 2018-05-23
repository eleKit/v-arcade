using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CarPath
{
	public enum CarSection
	{
		Start,
		Middle,
		End,
		Stop}

	;

	public CarSection section;

	public int num_obstacles;

	public int curve_position;

}