using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinCollectScript : MonoBehaviour
{

	/* Shooting game attributes */

	//the therapist must be able to change this paramether
	[Range (0, 500)]
	public int max_duck_time = 10;
	//the time after the duck must fall down


	//the counter used to count the #frames in which the pointer is inside the duck collider
	private int duck_time = 0;


	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	/* Scripts used by car game */

	void OnTriggerEnter2D (Collider2D other)
	{
		if (SceneManager.GetActiveScene ().name.Equals ("Car_game")) {
			this.gameObject.SetActive (false);
			CarGameManager.Instance.AddPoints ();
			Debug.Log ("Collect diamond");
		}

	}

	/* Scripts used by shooting game */

	//Sent each frame where another object is within a trigger collider attached to this object (2D physics only).
	void OnTriggerStay2D (Collider2D other)
	{

		if (SceneManager.GetActiveScene ().name.Equals ("Shooting_game")) {
			if (other.gameObject.CompareTag ("Player")) {
				duck_time++;

				if (duck_time >= max_duck_time) {
					//ShootingGameManager.Instance.AddPoints ();
					StartCoroutine ("Fall");
					Debug.Log ("duck shooted");
				}
			}
		}
		
	}


	//after the duck is shooted it falls down
	IEnumerator Fall ()
	{
		yield return new WaitForSeconds (0.5f);
		//TODO shoot SFX

		Rigidbody2D rb2d = GetComponent<Rigidbody2D> ();
		Collider2D coll2d = GetComponent<Collider2D> ();

		rb2d.isKinematic = false;
		Destroy (coll2d);

		yield return new WaitForSeconds (2.5f);
		gameObject.SetActive (false);
	}
}
