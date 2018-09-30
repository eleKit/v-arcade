using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PatientsList
{
	public List<string> patients;

	public PatientsList ()
	{
		patients = new List<string> ();
	}

}
