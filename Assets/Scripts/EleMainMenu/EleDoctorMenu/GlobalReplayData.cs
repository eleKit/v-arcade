using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalReplayData : MonoBehaviour
{

	public static GlobalReplayData globalReplayData;

	public string patient_folder_name;

	void Awake ()
	{
		if (globalReplayData == null) {
			DontDestroyOnLoad (gameObject);
			globalReplayData = this;
		} else if (globalReplayData != this) {
			Destroy (gameObject);
		}
	}



}
