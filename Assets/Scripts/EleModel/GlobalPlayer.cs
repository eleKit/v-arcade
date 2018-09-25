using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GlobalPlayer
{

	public string name;

	/* data used to set game angles
	 * Pay attention! yaw scale and pitch scale are now saved in RADIANTS
	 */
	public float yaw_scale;
	public float pitch_scale;

	//most recent tuning data
	public float pitch_left_max;
	public float pitch_left_min;
	public float pitch_right_max;
	public float pitch_right_min;

	public float yaw_left_max;
	public float yaw_left_min;
	public float yaw_right_max;
	public float yaw_right_min;

	private const int PITCH_SCALE = 2;
	private const int YAW_SCALE = 3;

	public void ComputeGesturesDeltas ()
	{
		yaw_scale = Mathf.Min (Mathf.Abs (yaw_left_max), Mathf.Abs (yaw_left_min),
			Mathf.Abs (yaw_right_max), Mathf.Abs (yaw_right_min)) / YAW_SCALE;

		pitch_scale = Mathf.Min (Mathf.Abs (pitch_left_max), Mathf.Abs (pitch_left_min),
			Mathf.Abs (pitch_right_max), Mathf.Abs (pitch_right_min)) / PITCH_SCALE;


		
	}

}

