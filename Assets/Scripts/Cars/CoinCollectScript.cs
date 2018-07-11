using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

public class CoinCollectScript : MonoBehaviour
{

	/* Shooting game attributes */

	//the time after the duck must fall down
	[Range (0, 10)]
	public float max_duck_time = 0.5f;




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
			/*	this.gameObject.SetActive (false);
			CarManager.Instance.AddPoints ();
			Debug.Log ("Collect diamond");
			SfxManager.Instance.Play ("pickup");
			Debug.Log ("music");*/

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
		} 

		if (SceneManager.GetActiveScene ().name.Equals ("Music_game")) {

			Debug.Log ("inside trigger");


			if (other.gameObject.tag.Equals ("LeftButton")) {
				MusicGameManager.Instance.left_trigger = true;
				MusicGameManager.Instance.left_hand_to_delete = other.gameObject;
			}
			
			if (other.gameObject.tag.Equals ("RightButton")) {
				MusicGameManager.Instance.right_trigger = true;
				MusicGameManager.Instance.right_hand_to_delete = other.gameObject;
			}
		}


		if (SceneManager.GetActiveScene ().name.Equals ("Shooting_game")) {
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
		
		if (SceneManager.GetActiveScene ().name.Equals ("Music_game")) {

			//deactivate the pink hand | blue hand
			other.gameObject.SetActive (false);


			if (other.gameObject.tag.Equals ("LeftButton")) {
				MusicGameManager.Instance.left_trigger = false;
			}
			if (other.gameObject.tag.Equals ("RightButton")) {
				MusicGameManager.Instance.right_trigger = false;
			}

			
		}

		if (SceneManager.GetActiveScene ().name.Equals ("Car_game")) {
			/*	this.gameObject.SetActive (false);
			CarManager.Instance.AddPoints ();
			Debug.Log ("Collect diamond");
			SfxManager.Instance.Play ("pickup");
			Debug.Log ("music");*/

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
		} 
		
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
