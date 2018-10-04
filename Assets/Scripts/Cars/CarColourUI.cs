using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class CarColourUI : MonoBehaviour
{

	public GameObject player;

	public void SetCarColour (Sprite car)
	{
		player.GetComponent<SpriteRenderer> ().sprite = car;
	}


	public void SetCarAnimator (AnimatorController animator)
	{
		player.GetComponent<Animator> ().runtimeAnimatorController = animator;
	}
}
