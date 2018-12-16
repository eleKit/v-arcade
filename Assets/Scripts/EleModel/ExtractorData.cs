using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ExtractorData
{

	public string patientName;

	public long timestamp;

	public List<float> flexion = new List<float> ();
	public List<float> extension = new List<float> ();
	public List<float> radial = new List<float> ();
	public List<float> ulnar = new List<float> ();
}
