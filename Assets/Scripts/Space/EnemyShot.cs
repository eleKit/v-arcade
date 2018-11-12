using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class EnemyShot : MonoBehaviour
{
	public GameObject explosion;

	public GameObject hit;

	[Header ("points to add every time, if negative points are subtracted")]
	public int points = 10;

	public bool big_alien;
	public int max_big_alien_hits = 1;

	public bool mini_boss;
	public int max_mini_boss_hits = 4;

	public bool boss;
	public int max_boss_hits = 19;

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
		if (other.gameObject.CompareTag ("Shot")) {
			if (!big_alien) {
			
				ExplodeAlien (other);
			} else if (big_alien) {
				if (num_shots >= max_big_alien_hits && !is_shot) {
				
					ExplodeAlien (other);

				} else if (!is_shot) {
				
					num_shots++;
					SfxManager.Instance.Play ("rumble");
					other.gameObject.SetActive (false);
					Instantiate (hit, transform.position, Quaternion.identity);

				}
			} else if (mini_boss) {

				if (num_shots >= max_mini_boss_hits && !is_shot) {

					ExplodeAlien (other);

				} else if (!is_shot) {

					num_shots++;
					SfxManager.Instance.Play ("rumble");
					other.gameObject.SetActive (false);
					Instantiate (hit, transform.position, Quaternion.identity);

				}
			
			} else if (boss) {
				if (num_shots >= max_boss_hits && !is_shot) {

					ExplodeAlien (other);

				} else if (!is_shot) {

					num_shots++;
					SfxManager.Instance.Play ("rumble");
					other.gameObject.SetActive (false);
					Instantiate (hit, transform.position, Quaternion.identity);

				}
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
		SpaceGameManager.Instance.AddPoints (points);

		StartCoroutine (WaitBeforeDeactivate ());
	}

}
