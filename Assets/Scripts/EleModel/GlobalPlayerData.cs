using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerData : MonoBehaviour
{
	public string player;

	public static GlobalPlayerData globalPlayerData;

	void Awake ()
	{
		if (globalPlayerData == null) {
			DontDestroyOnLoad (gameObject);
			globalPlayerData = this;
		} else if (globalPlayerData != this) {
			Destroy (gameObject);
		}
	}
}
