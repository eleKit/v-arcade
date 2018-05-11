using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

public class CoinCollectScript : MonoBehaviour
{

	/* Shooting game attributes */

	//the therapist must be able to change this paramether
	[Range (50, 500)]
	public int max_duck_time = 50;
	//the time after the duck must fall down


	//the counter used to count the #frames in which the pointer is inside the duck collider
	private int duck_time = 0;

	private bool duck_already_shooted = false;


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
			CarManager.Instance.AddPoints ();
			Debug.Log ("Collect diamond");
		} 

		if (SceneManager.GetActiveScene ().name.Equals ("Music_game")) {

			Debug.Log ("inside trigger");

			MusicGameManager.Instance.hand_to_delete = other.gameObject;

			if (other.gameObject.name.Equals ("Pink_left_hand"))
				MusicGameManager.Instance.left_trigger = true;
			if (other.gameObject.name.Equals ("Blue_right_hand"))
				MusicGameManager.Instance.right_trigger = true;
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		
		if (SceneManager.GetActiveScene ().name.Equals ("Music_game")) {

			//deactivate the pink hand | blue hand
			other.gameObject.SetActive (false);


			if (other.gameObject.name.Equals ("Pink_left_hand")) {
				MusicGameManager.Instance.left_trigger = false;
			}
			if (other.gameObject.name.Equals ("Blue_right_hand")) {
				MusicGameManager.Instance.right_trigger = false;
			}

			
		}
		
	}
		

	/* Scripts used by shooting game */

	//Sent each frame where another object is within a trigger collider attached to this object (2D physics only).
	void OnTriggerStay2D (Collider2D other)
	{

		if (SceneManager.GetActiveScene ().name.Equals ("Shooting_game")) {
			if (other.gameObject.CompareTag ("Player")) {
				if (other.gameObject.GetComponent<SpriteRenderer> ().color.Equals (Color.white)) {
					duck_time++;
					Debug.Log ("duck time " + duck_time.ToString ());

					if (duck_time >= max_duck_time && !duck_already_shooted) {
						duck_already_shooted = true;
						ShootingManager.Instance.AddPoints ();
						StartCoroutine ("Fall");
						Debug.Log ("duck shooted");
					}
				}
			}
		}
		
	}


	//after the duck is shooted it falls down
	IEnumerator Fall ()
	{
		
		SfxManager.Instance.Play ("rumble");
		Debug.Log ("music");

		Rigidbody2D rb2d = GetComponent<Rigidbody2D> ();
		Collider2D coll2d = GetComponent<Collider2D> ();
		duck_already_shooted = false;
		duck_time = 0;

		rb2d.isKinematic = false;
		Destroy (coll2d);

		yield return new WaitForSeconds (2.5f);
		gameObject.SetActive (false);

	}
}
