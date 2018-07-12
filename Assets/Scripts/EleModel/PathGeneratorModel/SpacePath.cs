using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SpacePath
{
	public SpaceSection back;
	public SpaceSection middle;
	public SpaceSection front;

	public string id_path;

	public long timestamp;

	public string doctorName;
}
