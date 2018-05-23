using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDoctorData : MonoBehaviour
{

	public string doctor;

	public static GlobalDoctorData globalDoctorData;

	void Awake ()
	{
		if (globalDoctorData == null) {
			DontDestroyOnLoad (gameObject);
			globalDoctorData = this;
		} else if (globalDoctorData != this) {
			Destroy (gameObject);
		}
	}
}
