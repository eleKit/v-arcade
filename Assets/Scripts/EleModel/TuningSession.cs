using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TuningSession
{

	public string patientName;

	public long timestamp;

	public float pitch_left_max;
	public float pitch_left_min;
	public float pitch_right_max;
	public float pitch_right_min;
	public float yaw_left_max;
	public float yaw_left_min;
	public float yaw_right_max;
	public float yaw_right_min;
}
