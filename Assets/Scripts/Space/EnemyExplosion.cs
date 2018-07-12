using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{

	// Use this for initialization
	void OnEnable ()
	{
		StartCoroutine (WaitForAnimation ());
	}

	IEnumerator WaitForAnimation ()
	{
		yield return new WaitForSeconds (0.7f);
		gameObject.SetActive (false);
	}
}



