using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class SpaceStandard
{

	//y fixed coord targets
	public const float BACK_Y = 20f;

	public const float MIDDLE_Y = 15f;

	public const float FRONT_Y = 10f;



	//These coupled array have always the same size:

	//x starting coord of front targets
	public float[] front_enemies_x;
	//this contains the indexes that corresponds to the array of the GO in the DuchPathGenerator
	public int[] front_generation_indexes;

	//x starting coord of front targets
	public float[] middle_enemies_x;
	//this contains the indexes that corresponds to the array of the GO in the DuchPathGenerator
	public int[] middle_generation_indexes;

	//x starting coord of front targets
	public float[] back_enemies_x;
	//this contains the indexes that corresponds to the array of the GO in the DuchPathGenerator
	public int[] back_generation_indexes;

	/*  Generation indexes values:
	 * 0 standard enemy
	 * 1 big enemy
	 * 2 eyed enemy
	 * 3 mini boss black
	 * 4 mini boss gree
	 * 5 boss
	 */


}
