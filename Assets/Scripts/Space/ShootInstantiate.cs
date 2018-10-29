using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class ShootInstantiate : MonoBehaviour
{


	public GameObject shot;


	public Color[] m_loading_bar_color;
	public Color m_loading_bar_shoot_color;

	public GameObject[] loading_bar;




	[Range (0f, 10f)]
	public float delta_t = 2f;

	float spawn_time = 0f;

	int index = 0;
	float one_step_value;
	float steps_counter = 0;

	bool coroutine_started;

	float t, t1;

	/*player gameobject used to instantiate the shot in the correct position even if the game is paused but the Shoot () instantiation 
	 * Coroutine is already started
	 */
	GameObject pl;

	void Awake ()
	{
		pl = GameObject.FindGameObjectWithTag ("Player");
	}

	// Use this for initialization
	void Start ()
	{
		t = TimerManager.Instance.m_time_of_Timer - delta_t;

		coroutine_started = false;
		one_step_value = delta_t / loading_bar.Length;

		t1 = TimerManager.Instance.m_time_of_Timer - one_step_value;

		ClearLoadingBar ();
		ResetLoadingBar ();


	}



	// Update is called once per frame
	void Update ()
	{



		if (GameManager.Instance.Get_Is_Playing ()) {
			
			if (!(pl.GetComponent<SpriteRenderer> ().color.Equals (pl.GetComponent<SpaceGesture> ().transparent_white)
			    || pl.GetComponent<SpriteRenderer> ().color.Equals (pl.GetComponent<SpaceGesture> ().medium_white))) {


				if (TimerManager.Instance.GetTimer () < t1) {
					
					if (index < loading_bar.Length) {
						//ClearLoadingBar ();
						loading_bar [index].GetComponent<SpriteRenderer> ().color = m_loading_bar_color [index];
					}
					index++;
					t1 = t1 - one_step_value;
				}

				if (TimerManager.Instance.GetTimer () < t && !coroutine_started) {

					StartCoroutine (Shoot ());
				}
			}
		}
	}





	IEnumerator Shoot ()
	{
		coroutine_started = true;

		if (index < loading_bar.Length) {
			//ClearLoadingBar ();
			loading_bar [index].GetComponent<SpriteRenderer> ().color = m_loading_bar_color [index];
		}

		yield return new WaitForSeconds (0.5f);

		ColorLoadingBarShoot ();


		//if (pl != null) {
		Transform player_pos = pl.GetComponent<Transform> ();
		Instantiate (shot, player_pos.position, Quaternion.identity);
		SfxManager.Instance.Play ("laser");
		//}


		yield return new WaitForSeconds (0.5f);


		t = TimerManager.Instance.GetTimer () - delta_t;
		ClearLoadingBar ();
		ResetLoadingBar ();
		coroutine_started = false;
	}




	void ClearLoadingBar ()
	{
		
		for (int i = 0; i < loading_bar.Length; i++) {
			loading_bar [i].GetComponent<SpriteRenderer> ().color = Color.white;

		}
			
	}

	void ResetLoadingBar ()
	{
		index = 0;
		t1 = TimerManager.Instance.GetTimer () - one_step_value;
		//loading_bar [index].GetComponent<SpriteRenderer> ().color = m_loading_bar_color;
	}


	void ColorLoadingBarShoot ()
	{
		for (int i = 0; i < loading_bar.Length; i++) {
			loading_bar [i].GetComponent<SpriteRenderer> ().color = m_loading_bar_shoot_color;

		}
	}



}
