using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DuckPath
{

	public DuckSection back;
	public DuckSection middle;
	public DuckSection front;

	public string id_path;

	public long timestamp;

	public string doctorName;

	//false if this is a training generated level
	public bool standard;

	//standard_level_attribute
	public DuckStandard standard_model;


}
