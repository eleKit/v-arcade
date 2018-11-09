using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class DuckStandard
{
	//y fixed coord targets
	public const float BACK_Y = 3f;

	public const float MIDDLE_Y = 0.5f;

	public const float FRONT_Y = -1.5f;



	//These coupled array have always the same size:

	//x starting coord of front targets
	public float[] front_obstacles_x;
	//this contains the indexes that corresponds to the array of the GO in the DuchPathGenerator
	public int[] front_generation_indexes;

	//x starting coord of front targets
	public float[] middle_obstacles_x;
	//this contains the indexes that corresponds to the array of the GO in the DuchPathGenerator
	public int[] middle_generation_indexes;

	//x starting coord of front targets
	public float[] back_obstacles_x;
	//this contains the indexes that corresponds to the array of the GO in the DuchPathGenerator
	public int[] back_generation_indexes;

	/*  Generation indexes values:
	 * 0 fixed target
	 * 1 red moving target
	 * 2 moving duck
	 */


}
