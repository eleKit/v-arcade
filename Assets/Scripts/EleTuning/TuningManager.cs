using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TuningManager : MonoBehaviour
{

	[Header ("Images about the hand movement")]
	public GameObject[] m_hands_images;


	[Header ("Intructions about the hand movement")]
	public GameObject[] m_instructions_screen;

	[Header ("Seconds the player has to wait")]
	public GameObject m_timer_object;
	public Text m_timer;

	public GameObject m_left_result;
	public Text m_left_result_text;
	public GameObject m_right_result;
	public Text m_right_result_text;

	[Range (0f, 5f)]
	public float waiting_time = 1f;

	[Header ("Time in which the player moves the hands")]
	[Range (0f, 200f)]
	public float timeLeft = 200f;

	//the hand controller prefab in the scene
	public GameObject handController;


	private HandController hc;


	private float timer = 0f;

	private int count = 0;

	private int wait_counter = 0;

	private const int MAX = 200;

	private bool is_playing = false;

	//save the past pitch angles of left and right hand in the previous K frames
	private LinkedList<float> left_pitch = new LinkedList<float> ();
	private LinkedList<float> right_pitch = new LinkedList<float> ();

	//save the past yaw angles of left and right hand in the previous K frames
	private LinkedList<float> left_yaw = new LinkedList<float> ();
	private LinkedList<float> right_yaw = new LinkedList<float> ();

	//save the past pitch offset to keep an horizontal position of hands
	private LinkedList<float> left_horizontal_pitch = new LinkedList<float> ();
	private LinkedList<float> right_horizontal_pitch = new LinkedList<float> ();



	// Use this for initialization
	void Start ()
	{
		count = 0;

		hc = handController.GetComponent<HandController> ();

		ClearScreens ();
		m_hands_images [count].SetActive (true);
		m_instructions_screen [count].SetActive (true);
		m_timer_object.SetActive (true);

		timeLeft++;

		timer = timeLeft;

		Debug.Log (timer.ToString ());

		StartCoroutine ("WaitToStart");
		count++;
		is_playing = true;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (Input.GetKeyDown ("escape") && is_playing) {
			PauseLevel ();
		}

		if (hc.GetFrame ().Hands.Count == 2 && is_playing) {
			if (count < m_hands_images.Length && count < m_instructions_screen.Length) {

				if ((int)Mathf.Round (timer) % 60 < 0) {
					count++;
					timer = timeLeft;
					Debug.Log ("end phase");
				} else {

					if (Mathf.Approximately (timer, timeLeft)) {
						//wait to set active the next screen
						//StartCoroutine ("StartPhase");
						ClearScreens ();
						m_hands_images [count].SetActive (true);
						m_timer_object.SetActive (true);
						m_timer.text = "Iniziamo!";
						m_instructions_screen [count].SetActive (true);
						Debug.Log ("start phase, timer " + timer.ToString ());

					}

					//counting the time left
					float deltaTime = Time.deltaTime;
					timer = timer - deltaTime;
					int sec = (int)Mathf.Round (timer) % 60;
					if (sec == 0) {
						m_timer.text = "Bene!";
					} else {
						if (!Mathf.Approximately (timer, (timeLeft - deltaTime))) {
							m_timer.text = sec.ToString ();
							//Debug.Log ("timer " + timer.ToString ("n2") + " timeLeft - deltaTime " + (timeLeft - deltaTime).ToString ("n2"));
						}
					}

					//Debug.Log ("Sono qui " + count.ToString () + " sec " + sec.ToString ());

					// max yaw count = 1
					// min yaw count = 2
					// max pitch count = 3
					// min pitch count = 4
					// horizontal starting point = 5

					if (count <= 2) { 
						if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
							left_yaw.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Yaw);
						}
						if (hc.GetFrame ().Hands.Rightmost.IsRight) {
							right_yaw.AddLast (hc.GetFrame ().Hands.Rightmost.Direction.Yaw);
						}
					} else if (count <= 4) {
						if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
							left_pitch.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
						}
						if (hc.GetFrame ().Hands.Rightmost.IsRight) {
							right_pitch.AddLast (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
						}
					} else if (count == 5) {
						if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
							left_horizontal_pitch.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
						}
						if (hc.GetFrame ().Hands.Rightmost.IsRight) {
							right_horizontal_pitch.AddLast (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
						}
					}

				}

			} else {
				ClearScreens ();

				float left_max_pitch = left_pitch.Max ();
				float right_max_pitch = right_pitch.Max ();

				float left_min_pitch = left_pitch.Min ();
				float right_min_pitch = right_pitch.Min ();

				float left_max_yaw = left_yaw.Max ();
				float right_max_yaw = left_yaw.Max ();

				float left_min_yaw = left_yaw.Min ();
				float right_min_yaw = left_yaw.Min ();

				m_left_result.SetActive (true);

				m_right_result.SetActive (true);

				m_right_result_text.text =
					"Estensione destra: " + Mathf.Abs (right_max_pitch * Mathf.Rad2Deg).ToString ("n2") + "°" + "\n"
				+ "Flessione destra: " + Mathf.Abs (right_min_pitch * Mathf.Rad2Deg).ToString ("n2") + "°" + "\n"
				+ "Dev ulnare destra: " + Mathf.Abs (right_max_yaw * Mathf.Rad2Deg).ToString ("n2") + "°" + "\n"
				+ "Dev radiale destra: " + Mathf.Abs (right_min_yaw * Mathf.Rad2Deg).ToString ("n2") + "°";

				m_left_result_text.text =
					"Estensione sinistra: " + Mathf.Abs (left_max_pitch * Mathf.Rad2Deg).ToString ("n2") + "°" + "\n"
				+ "Flessione sinistra: " + Mathf.Abs (left_min_pitch * Mathf.Rad2Deg).ToString ("n2") + "°" + "\n"
				+ "Dev ulnare sinistra: " + Mathf.Abs (left_max_yaw * Mathf.Rad2Deg).ToString ("n2") + "°" + "\n"
				+ "Dev radiale sinista: " + Mathf.Abs (left_min_yaw * Mathf.Rad2Deg).ToString ("n2") + "°";





			}
		}
			
		
	}


	IEnumerator WaitToStart ()
	{
		yield return new WaitForSeconds (waiting_time);
	}

	IEnumerator StartPhase ()
	{
		ClearScreens ();
		m_hands_images [count].SetActive (true);
		m_timer_object.SetActive (true);
		m_instructions_screen [count].SetActive (true);

		yield return new WaitForSeconds (0f);
	}

	IEnumerator EndPhase ()
	{
		yield return new WaitForSeconds (2f);
		wait_counter = 0;
	}



	void PauseLevel ()
	{

		is_playing = false;
		Debug.Log ("paused");

		//menu_GUI.pause = true;
		
	}

	void ClearScreens ()
	{
		for (int i = 0; i < m_instructions_screen.Length; i++) {
			if (m_instructions_screen [i] != null)
				m_instructions_screen [i].SetActive (false);
		}

		for (int i = 0; i < m_hands_images.Length; i++) {
			if (m_hands_images [i] != null)
				m_hands_images [i].SetActive (false);
		}

		if (m_timer_object != null)
			m_timer_object.SetActive (false);

		if (m_left_result != null)
			m_left_result.SetActive (false);

		if (m_right_result != null)
			m_right_result.SetActive (false);
	}
}
