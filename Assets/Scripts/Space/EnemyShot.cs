using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class EnemyShot : MonoBehaviour
{
	public GameObject explosion;

	public GameObject hit;

	public bool big_alien;

	public bool is_shot;

	int num_shots;

	void Awake ()
	{
		is_shot = false;
		num_shots = 0;
	}


	// Use this for initialization
	void OnTriggerEnter2D (Collider2D other)
	{
		if (!big_alien) {
			
			ExplodeAlien (other);
		} else {
			if (num_shots >= 1 && !is_shot) {
				
				ExplodeAlien (other);

			} else if (!is_shot) {
				
				num_shots++;
				SfxManager.Instance.Play ("rumble");
				other.gameObject.SetActive (false);
				Instantiate (hit, transform.position, Quaternion.identity);

			}
		}

	}

	IEnumerator WaitBeforeDeactivate ()
	{
		yield return new WaitForSeconds (0.5f);
		this.gameObject.SetActive (false);
	}





	void ExplodeAlien (Collider2D other)
	{

		is_shot = true;
		Instantiate (explosion, transform.position, Quaternion.identity);

		SfxManager.Instance.Play ("rumble");
		other.gameObject.SetActive (false);
		Collider2D coll = GetComponent <Collider2D> ();
		coll.enabled = false;
		SpaceGameManager.Instance.AddPoints ();

		StartCoroutine (WaitBeforeDeactivate ());
	}

}
