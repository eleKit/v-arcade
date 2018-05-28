using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TuningManager : MonoBehaviour
{
	const int N = 5;

	[Header ("Images about the hand movement")]
	public GameObject[] m_hands_image = new GameObject[N];

	[Header ("Intructions about the hand movement")]
	public GameObject[] m_instructions_screen = new GameObject[N];

	[Header ("Seconds the player has to wait")]
	public GameObject m_timer_object;
	public Text m_timer;

	public GameObject m_left_result;
	public Text m_left_result_text;
	public GameObject m_right_result;
	public Text m_right_result_text;



	[Header ("Time in which the player moves the hands")]
	[Range (0f, 10f)]
	public float m_timeout_s = 200f;

	[Header ("Time when the player reads the instruction")]
	[Range (0f, 10f)]
	public float m_instruction_timeout_s = 200f;

	//the hand controller prefab in the scene
	public GameObject handController;


	private HandController hc;


	private int index = 0;


	//save the past pitch angles of left and right hand in the previous K frames
	private List<float> left_pitch = new List<float> ();
	private List<float> right_pitch = new List<float> ();

	//save the past yaw angles of left and right hand in the previous K frames
	private List<float> left_yaw = new List<float> ();
	private List<float> right_yaw = new List<float> ();

	//save the past pitch offset to keep an horizontal position of hands
	private List<float> left_roll = new List<float> ();
	private List<float> right_roll = new List<float> ();

	private Counter counter = null;


	Tuning_Phase current_phase = Tuning_Phase.Null;

	Tuning_Phase previous_phase = Tuning_Phase.Null;

	/*
	 * Class for a simple timeout counter.
	 * 
	 * Initialize the counter with a timeout in seconds; at each FixedUpdate call countFrame();
	 * to know whether the time is up, call timeIsUp().
	 */
	class Counter
	{

		float timeout_s;
		float instruction_time_s;
		int counter_frames;

		public Counter (float timeout_s, float instruction_time_s)
		{
			this.timeout_s = timeout_s + instruction_time_s;
			this.instruction_time_s = instruction_time_s;
			this.counter_frames = 0;
		}

		/*
		 * Method to call to count up one frame
		*/
		public void countFrame ()
		{
			this.counter_frames += 1;
		}

		/*
		 * Return true if the counter time is up
		 */
		public bool timeIsUp ()
		{
			return counter_frames / 60 > timeout_s;
		}


		public bool instructionTimeIsUp ()
		{
			return counter_frames / 60 > instruction_time_s;
		}

		public int getCounterFrame ()
		{
			return counter_frames;
		}
	}


	// Use this for initialization
	void Start ()
	{
		

		hc = handController.GetComponent<HandController> ();
		StartLevel ();


	}


	// Update is called once per frame
	void FixedUpdate ()
	{

		switch (current_phase) {

		/* the phases are in this order:
		 * 1. no hands on the leap
		 * 2. Initial instruction of tuning
		 * 3. yaw recording
		 * 4. roll recording
		 * 5. pitch recording
		 * 6. End notes with data results
		 * if the player removes the hand the state becomes again 1. no hands on the leap
		 * but then restarts from the previous (2.-5.) phase and not from the beginning
		 */

		case Tuning_Phase.NoHands_phase:
			if (HandsOk ()) {
				if (previous_phase == Tuning_Phase.Null) {
					index++;
					current_phase = Tuning_Phase.Start_phase;
				} else {
					current_phase = previous_phase;
				}
			}
			break;

		case Tuning_Phase.Start_phase:

			previous_phase = current_phase;

			if (counter == null) {
				counter = new Counter (2f, 0f);
				ClearScreens ();
				m_hands_image [index].SetActive (true);
				m_timer_object.SetActive (true);
				m_timer.text = "Tra poco iniziamo!";
				m_instructions_screen [index].SetActive (true);
		
			}

			if (counter.timeIsUp ()) {
				counter = null;
				index++;
				current_phase = Tuning_Phase.Yaw_phase;
			} else if (!HandsOk ()) {
				counter = null;
				current_phase = Tuning_Phase.NoHands_phase;
			} else {
				counter.countFrame ();
			} 

			break;

		case Tuning_Phase.Yaw_phase:
			
			previous_phase = current_phase;

			if (counter == null) {
				counter = new Counter (m_timeout_s, m_instruction_timeout_s);
				ClearScreens ();
				m_hands_image [index].SetActive (true);
				m_timer_object.SetActive (true);
				m_timer.text = "Iniziamo!";
				m_instructions_screen [index].SetActive (true);
			} else if (counter.instructionTimeIsUp ()) {

				if (counter.timeIsUp ()) {
					counter = null;
					current_phase = Tuning_Phase.Roll_phase;
					index++;
				} else if (!HandsOk ()) {
					counter = null;
					current_phase = Tuning_Phase.NoHands_phase;
				} else {
					counter.countFrame ();

					m_timer.text = counter.getCounterFrame ().ToString ();

					//save yaw gestures

					if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
						left_yaw.Add (hc.GetFrame ().Hands.Leftmost.Direction.Yaw);
					}
					if (hc.GetFrame ().Hands.Rightmost.IsRight) {
						right_yaw.Add (hc.GetFrame ().Hands.Rightmost.Direction.Yaw);
					}
				}
			} else {
				counter.countFrame ();
				//the player has the time to read the intruction screen before playing
			}

			break;

		case Tuning_Phase.Roll_phase:

			previous_phase = current_phase;

			if (counter == null) {
				counter = new Counter (m_timeout_s, m_instruction_timeout_s);
				ClearScreens ();
				m_hands_image [index].SetActive (true);
				m_timer_object.SetActive (true);
				m_timer.text = "Iniziamo!";
				m_instructions_screen [index].SetActive (true);
			} else if (counter.instructionTimeIsUp ()) {

				if (counter.timeIsUp ()) {
					counter = null;
					current_phase = Tuning_Phase.Pitch_phase;
					index++;
				} else if (!HandsOk ()) {
					counter = null;
					current_phase = Tuning_Phase.NoHands_phase;
				} else {
					counter.countFrame ();

					m_timer.text = counter.getCounterFrame ().ToString ();

					//save roll gestures

					if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
						left_roll.Add (hc.GetFrame ().Hands.Leftmost.PalmNormal.Roll);
					}
					if (hc.GetFrame ().Hands.Rightmost.IsRight) {
						right_roll.Add (hc.GetFrame ().Hands.Rightmost.PalmNormal.Roll);
					}
				}
			} else {
				counter.countFrame ();
				//the player has the time to read the intruction screen before playing
			}

			break;

		case Tuning_Phase.Pitch_phase:

			previous_phase = current_phase;

			if (counter == null) {
				counter = new Counter (m_timeout_s, m_instruction_timeout_s);
				ClearScreens ();
				m_hands_image [index].SetActive (true);
				m_timer_object.SetActive (true);
				m_timer.text = "Iniziamo!";
				m_instructions_screen [index].SetActive (true);
			} else if (counter.instructionTimeIsUp ()) {

				if (counter.timeIsUp ()) {
					counter = null;
					current_phase = Tuning_Phase.End_phase;
					index++;
				} else if (!HandsOk ()) {
					counter = null;
					current_phase = Tuning_Phase.NoHands_phase;
				} else {
					counter.countFrame ();

					m_timer.text = counter.getCounterFrame ().ToString ();

					//save pitch gestures

					if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
						left_pitch.Add (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
					}
					if (hc.GetFrame ().Hands.Rightmost.IsRight) {
						right_pitch.Add (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
					}
				}
			} else {
				counter.countFrame ();
				//the player has the time to read the intruction screen before playing
			}

			break;

		case Tuning_Phase.End_phase:

			current_phase = Tuning_Phase.Finished_tuning;

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
			

			break;

		}
		
	}



	void StartLevel ()
	{

		index = 0;

		counter = null;

		current_phase = Tuning_Phase.NoHands_phase;
		previous_phase = Tuning_Phase.Null;

		ClearScreens ();
		m_timer.text = "Posiziona le mani sul Leap";
		m_hands_image [index].SetActive (true);
		m_instructions_screen [index].SetActive (true);
		m_timer_object.SetActive (true);
		
	}





	void ClearScreens ()
	{
		for (int i = 0; i < m_instructions_screen.Length; i++) {
			if (m_instructions_screen [i] != null)
				m_instructions_screen [i].SetActive (false);
		}

		for (int i = 0; i < m_hands_image.Length; i++) {
			if (m_hands_image [i] != null)
				m_hands_image [i].SetActive (false);
		}

		if (m_timer_object != null)
			m_timer_object.SetActive (false);

		if (m_left_result != null)
			m_left_result.SetActive (false);

		if (m_right_result != null)
			m_right_result.SetActive (false);
	}




	bool HandsOk ()
	{
		return (hc.GetFrame ().Hands.Count == 2); 
	}



	enum Tuning_Phase
	{
		Null,
		NoHands_phase,
		Start_phase,
		Yaw_phase,
		Pitch_phase,
		Roll_phase,
		End_phase,
		Finished_tuning,
	};



}
