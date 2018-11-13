using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class TuningManager : MonoBehaviour
{

	[Header ("Line at bottom indicated if leap sees the hands")]
	public Image colour_line;



	[Header ("Images about the hand movement")]
	public GameObject m_simple_hands_image;
	public GameObject m_up_hands_image;
	public GameObject m_down_hands_image;
	public GameObject m_left_hands_image;
	public GameObject m_right_hands_image;

	[Header ("Intructions about the hand movement")]
	public GameObject m_simple_instructions_screen;
	public GameObject m_up_instructions_screen;
	public GameObject m_down_instructions_screen;
	public GameObject m_left_instructions_screen;
	public GameObject m_right_instructions_screen;

	[Header ("Seconds the player has to wait")]
	public GameObject m_timer_object;
	public Text m_timer;


	[Header ("Result canvas")]
	public GameObject m_results_canvas;
	[Header ("Result texts")]
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



	[Header ("accessed by the SaveTuningManager to save the hand values on the PersistendDataPath")]
	public float data_left_extension = 0f;
	public float data_right_extension = 0f;

	public float data_left_flexion = 0f;
	public float data_right_flexion = 0f;

	public float data_left_radial = 0f;
	public float data_right_radial = 0f;

	public float data_left_ulnar = 0f;
	public float data_right_ulnar = 0f;

	private HandController hc;

	//save the past yaw down angles of left and right hand in the previous K frames
	private List<float> left_flexion = new List<float> ();
	private List<float> right_flexion = new List<float> ();

	//save the past yaw up angles of left and right hand in the previous K frames
	private List<float> left_estension = new List<float> ();
	private List<float> right_estension = new List<float> ();

	//save the past pitch radial deviation
	private List<float> left_radial = new List<float> ();
	private List<float> right_radial = new List<float> ();

	//save the past pitch ulnar deviation
	private List<float> left_ulnar = new List<float> ();
	private List<float> right_ulnar = new List<float> ();

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
			return counter_frames / 60;
		}
	}


	// Use this for initialization
	void Start ()
	{
		

		hc = handController.GetComponent<HandController> ();


		colour_line.color = Color.green;


		StartLevel ();


	}



	// Update is called once per frame
	void FixedUpdate ()
	{


		ManageTuningLightBar ();


		switch (current_phase) {

		/* the phases are in this order:
		 * 1. no hands on the leap
		 * 2. Initial instruction of tuning / Error restart tuning
		 * 3. pitch recording (up and down)
		 * 5. yaw recording (left and right)
		 * 6. End notes with data results (if Error restart from 2.)
		 * if the player removes the hand the state becomes again 1. no hands on the leap
		 * but then restarts from the previous (2.-5.) phase and not from the beginning
		 */

		case Tuning_Phase.NoHands_phase:
			if (HandsOk ()) {
				if (previous_phase == Tuning_Phase.Null) {

					current_phase = Tuning_Phase.Start_phase;

				} else {
					current_phase = previous_phase;
				}
			}
			break;




		case Tuning_Phase.Start_phase:

		
			if (counter == null) {
				previous_phase = current_phase;
				counter = new Counter (2f, 0f);

				ClearScreens ();

				m_simple_hands_image.SetActive (true);
				m_timer_object.SetActive (true);
				m_timer.text = "Tra poco iniziamo!";
				m_simple_instructions_screen.SetActive (true);
		
			}
				
			if (counter.timeIsUp ()) {
				
				current_phase = Tuning_Phase.Up_phase;

				counter = null;


			} else if (!HandsOk ()) {
				current_phase = Tuning_Phase.NoHands_phase;
				
				counter = null;

			} else {
				counter.countFrame ();
			} 

			break;

		case Tuning_Phase.Up_phase:
			
			if (counter == null) {

				previous_phase = current_phase;

				left_estension.Clear ();
				right_estension.Clear ();

				counter = new Counter (m_timeout_s, m_instruction_timeout_s);

				ClearScreens ();

				m_up_hands_image.SetActive (true);

				m_timer_object.SetActive (true);
				m_timer.text = "Pronti? ... Iniziamo!";
				m_up_instructions_screen.SetActive (true);

			} else if (counter.instructionTimeIsUp ()) {

				if (counter.timeIsUp ()) {

					current_phase = Tuning_Phase.Down_phase;

					counter = null;

			
				} else if (!HandsOk ()) {

					current_phase = Tuning_Phase.NoHands_phase;
					counter = null;

				} else {
					counter.countFrame ();

					m_timer.text = counter.getCounterFrame ().ToString ();

					//save yaw gestures

					if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
						left_estension.Add (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
					}
					if (hc.GetFrame ().Hands.Rightmost.IsRight) {
						right_estension.Add (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
					}
				}
			} else {
				counter.countFrame ();
				//the player has the time to read the intruction screen before playing
			}

			break;



		case Tuning_Phase.Down_phase:

			if (counter == null) {

				previous_phase = current_phase;

				left_flexion.Clear ();
				right_flexion.Clear ();

				counter = new Counter (m_timeout_s, m_instruction_timeout_s);

				ClearScreens ();

				m_down_hands_image.SetActive (true);

				m_timer_object.SetActive (true);
				m_timer.text = "Pronti? ... Iniziamo!";
				m_down_instructions_screen.SetActive (true);

			} else if (counter.instructionTimeIsUp ()) {

				if (counter.timeIsUp ()) {

					current_phase = Tuning_Phase.Left_phase;

					counter = null;

				
				} else if (!HandsOk ()) {

					current_phase = Tuning_Phase.NoHands_phase;
					counter = null;

				} else {
					counter.countFrame ();

					m_timer.text = counter.getCounterFrame ().ToString ();

					//save yaw gestures

					if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
						left_flexion.Add (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
					}
					if (hc.GetFrame ().Hands.Rightmost.IsRight) {
						right_flexion.Add (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
					}
				}
			} else {
				counter.countFrame ();
				//the player has the time to read the intruction screen before playing
			}

			break;

		

		case Tuning_Phase.Left_phase:



			if (counter == null) {

				previous_phase = current_phase;

				left_ulnar.Clear ();
				right_radial.Clear ();

				counter = new Counter (m_timeout_s, m_instruction_timeout_s);
				ClearScreens ();
				m_left_hands_image.SetActive (true);
				m_timer_object.SetActive (true);
				m_timer.text = "Pronti? ... Iniziamo!";
				m_left_instructions_screen.SetActive (true);

			} else if (counter.instructionTimeIsUp ()) {

				if (counter.timeIsUp ()) {
					
					current_phase = Tuning_Phase.Right_phase;
					counter = null;


				} else if (!HandsOk ()) {
					
					counter = null;
					current_phase = Tuning_Phase.NoHands_phase;

				} else {
					counter.countFrame ();

					m_timer.text = counter.getCounterFrame ().ToString ();

					//save pitch gestures

					if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
						left_ulnar.Add (hc.GetFrame ().Hands.Leftmost.Direction.Yaw);
					}
					if (hc.GetFrame ().Hands.Rightmost.IsRight) {
						right_radial.Add (hc.GetFrame ().Hands.Rightmost.Direction.Yaw);
					}
				}
			} else {
				counter.countFrame ();
				//the player has the time to read the intruction screen before playing
			}

			break;



		case Tuning_Phase.Right_phase:



			if (counter == null) {

				previous_phase = current_phase;

				left_radial.Clear ();
				right_ulnar.Clear ();

				counter = new Counter (m_timeout_s, m_instruction_timeout_s);
				ClearScreens ();

				m_right_hands_image.SetActive (true);

				m_timer_object.SetActive (true);
				m_timer.text = "Pronti? ... Iniziamo!";

				m_right_instructions_screen.SetActive (true);

			} else if (counter.instructionTimeIsUp ()) {

				if (counter.timeIsUp ()) {

					current_phase = Tuning_Phase.End_phase;
					counter = null;


				} else if (!HandsOk ()) {

					counter = null;
					current_phase = Tuning_Phase.NoHands_phase;

				} else {
					counter.countFrame ();

					m_timer.text = counter.getCounterFrame ().ToString ();

					//save pitch gestures

					if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
						left_radial.Add (hc.GetFrame ().Hands.Leftmost.Direction.Yaw);
					}
					if (hc.GetFrame ().Hands.Rightmost.IsRight) {
						right_ulnar.Add (hc.GetFrame ().Hands.Rightmost.Direction.Yaw);
					}
				}
			} else {
				counter.countFrame ();
				//the player has the time to read the intruction screen before playing
			}

			break;

		case Tuning_Phase.End_phase:
			
			ClearScreens ();
			EndTuning ();
			break;

			
		}
				


	}


	public void SkipTuning ()
	{
		current_phase = Tuning_Phase.End_phase;
		previous_phase = Tuning_Phase.End_phase;
	}



	void EndTuning ()
	{

		m_results_canvas.SetActive (true);

		//check for errors
		if (left_estension.Count == 0) {
			left_estension.Add (0f);
		}
		if (right_estension.Count == 0) {
			right_estension.Add (0f);
		} 
		if (left_flexion.Count == 0) {
			left_flexion.Add (0f);
		} 
		if (right_flexion.Count == 0) {
			right_flexion.Add (0f);
		} 
		if (left_radial.Count == 0) {
			left_radial.Add (0f);
		} 
		if (right_radial.Count == 0) {
			right_radial.Add (0f);
		} 
		if (left_ulnar.Count == 0) {
			left_ulnar.Add (0f);
		} 
		if (right_ulnar.Count == 0) {
			right_ulnar.Add (0f);
		}

		current_phase = Tuning_Phase.Finished_tuning;



		data_left_extension = left_estension.Average ();
		data_right_extension = right_estension.Average ();

		data_left_flexion = left_flexion.Average ();
		data_right_flexion = right_flexion.Average ();

		data_left_radial = left_radial.Average ();
		data_right_radial = right_radial.Average ();

		data_left_ulnar = left_ulnar.Average ();
		data_right_ulnar = right_ulnar.Average ();

		m_left_result.SetActive (true);

		m_right_result.SetActive (true);

		m_right_result_text.text =
			"Estensione destra: " + Mathf.Abs (data_right_extension * Mathf.Rad2Deg).ToString ("N1") + "°" + "\n"
		+ "Flessione destra: " + Mathf.Abs (data_right_flexion * Mathf.Rad2Deg).ToString ("N1") + "°" + "\n"
		+ "Dev ulnare destra: " + Mathf.Abs (data_right_ulnar * Mathf.Rad2Deg).ToString ("N1") + "°" + "\n"
		+ "Dev radiale destra: " + Mathf.Abs (data_right_radial * Mathf.Rad2Deg).ToString ("N1") + "°";

		m_left_result_text.text =
			"Estensione sinistra: " + Mathf.Abs (data_left_extension * Mathf.Rad2Deg).ToString ("N1") + "°" + "\n"
		+ "Flessione sinistra: " + Mathf.Abs (data_left_flexion * Mathf.Rad2Deg).ToString ("N1") + "°" + "\n"
		+ "Dev ulnare sinistra: " + Mathf.Abs (data_left_ulnar * Mathf.Rad2Deg).ToString ("N1") + "°" + "\n"
		+ "Dev radiale sinista: " + Mathf.Abs (data_left_radial * Mathf.Rad2Deg).ToString ("N1") + "°";
		
		
	}

	//called by Start() and by the Restart Tuning button
	public void StartLevel ()
	{

		counter = null;

		current_phase = Tuning_Phase.NoHands_phase;
		previous_phase = Tuning_Phase.Null;

		ClearScreens ();


		m_timer.text = "Posiziona le mani sul Leap";
		m_simple_hands_image.SetActive (true);
		m_simple_instructions_screen.SetActive (true);
		m_timer_object.SetActive (true);
		
	}







	void ClearScreens ()
	{
		//clear phases instructions hands images
		if (m_simple_hands_image != null)
			m_simple_hands_image.SetActive (false);
		
		if (m_up_hands_image != null)
			m_up_hands_image.SetActive (false);
		
		if (m_down_hands_image != null)
			m_down_hands_image.SetActive (false);
		
		if (m_left_hands_image != null)
			m_left_hands_image.SetActive (false);
		
		if (m_right_hands_image != null)
			m_right_hands_image.SetActive (false);

		//clear phases instruction texts
		if (m_simple_instructions_screen != null)
			m_simple_instructions_screen.SetActive (false);
		
		if (m_up_instructions_screen != null)
			m_up_instructions_screen.SetActive (false);
		
		if (m_down_instructions_screen != null)
			m_down_instructions_screen.SetActive (false);

		if (m_left_instructions_screen != null)
			m_left_instructions_screen.SetActive (false);
		
		if (m_right_instructions_screen != null)
			m_right_instructions_screen.SetActive (false);

		//clear timer
		if (m_timer_object != null)
			m_timer_object.SetActive (false);

		//clear results 
		if (m_left_result != null)
			m_left_result.SetActive (false);

		if (m_right_result != null)
			m_right_result.SetActive (false);

		if (m_results_canvas != null)
			m_results_canvas.SetActive (false);
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
		Up_phase,
		Down_phase,
		Left_phase,
		Right_phase,
		End_phase,
		Finished_tuning,
	};


	//Script for the light bar in the tuning scene see LightBarMAnager for the light bar inside the games
	void ManageTuningLightBar ()
	{
		
		if (colour_line.color.Equals (Color.green)) {

			if (hc.GetFixedFrame ().Hands.Count == 0 || hc.GetFixedFrame ().Hands.Count == 1) {
				colour_line.color = Color.red;

			}
		}

		if (colour_line.color.Equals (Color.red)) {

			if (hc.GetFixedFrame ().Hands.Count == 2) {
				colour_line.color = Color.green;
			}
		}

	}






}
