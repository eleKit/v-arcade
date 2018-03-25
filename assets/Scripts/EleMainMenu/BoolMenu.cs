using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolMenu : MonoBehaviour
{

	//boolean to chose the tipe of menu to show
	public bool player, fisio, menu;

	public static BoolMenu boolMenu;

	void Awake ()
	{
		if (boolMenu == null) {
			DontDestroyOnLoad (gameObject);
			boolMenu = this;
		} else if (boolMenu != this) {
			Destroy (gameObject);
		}
	}
}
