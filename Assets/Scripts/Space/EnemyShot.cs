using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class EnemyShot : MonoBehaviour
{
	public GameObject explosion;

	// Use this for initialization
	void OnTriggerEnter2D (Collider2D other)
	{
		Instantiate (explosion, transform.position, Quaternion.identity);

		SfxManager.Instance.Play ("rumble");
		other.gameObject.SetActive (false);
		Collider2D coll = GetComponent <Collider2D> ();
		coll.enabled = false;
		SpaceGameManager.Instance.AddPoints ();

		StartCoroutine (WaitBeforeDeactivate ());

	}

	IEnumerator WaitBeforeDeactivate ()
	{
		yield return new WaitForSeconds (0.5f);
		this.gameObject.SetActive (false);
	}

}
