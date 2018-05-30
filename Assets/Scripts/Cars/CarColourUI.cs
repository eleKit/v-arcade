using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarColourUI : MonoBehaviour
{

	public GameObject player;

	public void SetCarColour (Sprite car)
	{
		player.GetComponent<SpriteRenderer> ().sprite = car;
	}
}
