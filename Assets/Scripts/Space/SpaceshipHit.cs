using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipHit : MonoBehaviour
{
	//script that subtracts points if the spaceship is hit

	public GameObject explosion;

	[Header ("points to add every time, if negative points are subtracted")]
	public int points = -10;


	// Use this for initialization
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag ("Enemy_Shot")) {
			if (!(this.GetComponent<SpriteRenderer> ().color.Equals (this.GetComponent<SpaceGesturesManager> ().transparent_white)
			    || this.GetComponent <SpriteRenderer> ().color.Equals (this.GetComponent<SpaceGesturesManager> ().medium_white))) {
				SpaceGameManager.Instance.AddPoints (points);
				Instantiate (explosion, transform.position, Quaternion.identity);
				other.gameObject.SetActive (false);
			}

		}
	}

}
