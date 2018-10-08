using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class CarColourUI : MonoBehaviour
{

	public GameObject player;

	public void SetCarColour (Sprite car)
	{
		player.GetComponent<SpriteRenderer> ().sprite = car;
	}

	#if UNITY_EDITOR
	public void SetCarAnimator (AnimatorController animator)
	{
		player.GetComponent<Animator> ().runtimeAnimatorController = animator;
	}
	#endif
}
