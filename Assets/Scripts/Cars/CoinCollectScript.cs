using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

public class CoinCollectScript : MonoBehaviour
{
	/* This class is used by all the games (and the replays), 
	 * it add points when the coin is collected.
	 * When instantiated in the scene the bool indicating what kind of game is playing is set as True
	 * 
	 * N.B since aliens are enemies the Space Game uses a different script (EnemyShot.cs)
	 */

	//bool used to check what type of game is playing
	bool car, music, shooting;

	/* Shooting game attributes */

	//the time after the duck must fall down
	[Range (0, 10)]
	public float max_duck_time = 0.5f;




	// Use this for initialization
	void Start ()
	{
		SetBoolToFalse ();
		string current_scene = SceneManager.GetActiveScene ().name;

		switch (current_scene) {
		case ("Car_game"):
			car = true;
			break;
		case ("Car_replay"):
			car = true;
			break;
		case ("Shooting_game"):
			shooting = true;
			break;
		case ("Shooting_replay"):
			shooting = true;
			break;
		case ("Music_game"):
			music = true;
			break;
		case ("Music_replay"):
			music = true;
			break;
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	//used inside the start function
	void SetBoolToFalse ()
	{
		car = false;
		music = false;
		shooting = false;
	}

	/* Scripts used by car game */

	void OnTriggerEnter2D (Collider2D other)
	{
		
		if (car) {
			this.gameObject.SetActive (false);
			CarManager.Instance.AddPoints ();
			Debug.Log ("Collect diamond");
			SfxManager.Instance.Play ("pickup");
			Debug.Log ("music");

			/*
			//set the arrow (and arrow children element) color when the trigger is activated
			//GetComponentsInchildren returns also the parent component in the array
			SpriteRenderer[] renderers = this.gameObject.GetComponentsInChildren <SpriteRenderer> ();

			foreach (SpriteRenderer rd in renderers) {
				rd.color = new Color (1, 1, 1, 1);
			}

			
			if (this.CompareTag ("DecelerateCar")) {
				DrivePitchGesture.Instance.decelerate_trigger = true;
			} else {
				DrivePitchGesture.Instance.accelerate_trigger = true;
			}

			Debug.Log ("enter arrow trigger");
			*/
		} 

		if (music) {

			if (other.gameObject.tag.Equals ("LeftButton")) {
				MusicGameManager.Instance.left_trigger = true;
				MusicGameManager.Instance.left_hand_to_delete = other.gameObject;
			}
			
			if (other.gameObject.tag.Equals ("RightButton")) {
				MusicGameManager.Instance.right_trigger = true;
				MusicGameManager.Instance.right_hand_to_delete = other.gameObject;
			}
		}


		if (shooting) {
			if (other.gameObject.CompareTag ("Player")) {
				if (other.gameObject.GetComponent<SpriteRenderer> ().color.Equals (Color.white)) {

					ShootingManager.Instance.AddPoints ();
					StartCoroutine (Fall ());
					Debug.Log ("duck shooted");
				}
			}
		}

	}

	void OnTriggerExit2D (Collider2D other)
	{
		
		if (music) {

			//deactivate the pink hand | blue hand
			other.gameObject.SetActive (false);


			if (other.gameObject.tag.Equals ("LeftButton")) {
				MusicGameManager.Instance.left_trigger = false;
			}
			if (other.gameObject.tag.Equals ("RightButton")) {
				MusicGameManager.Instance.right_trigger = false;
			}

			
		}

		/*if (car) {
			
			//set the arrow (and arrow children element) color when the trigger is deactivated
			//GetComponentsInchildren returns also the parent component in the array
			SpriteRenderer[] renderers = this.gameObject.GetComponentsInChildren <SpriteRenderer> ();

			foreach (SpriteRenderer rd in renderers) {
				rd.color = new Color (1, 1, 1, 0.33f);
			}

			Debug.Log ("exit arrow trigger");
			if (this.GetComponent<SpriteRenderer> ().color.Equals (Color.red)) {
				DrivePitchGesture.Instance.decelerate_trigger = false;
			} else {
				DrivePitchGesture.Instance.accelerate_trigger = false;
			}
		} */
		
	}
		



	//after the duck is shooted it falls down
	IEnumerator Fall ()
	{
		SfxManager.Instance.Play ("rumble");
		yield return new WaitForSeconds (max_duck_time);

		Rigidbody2D rb2d = GetComponent<Rigidbody2D> ();
		Collider2D coll2d = GetComponent<Collider2D> ();



		rb2d.isKinematic = false;
		Destroy (coll2d);

		yield return new WaitForSeconds (2f);
		gameObject.SetActive (false);

	}
}
