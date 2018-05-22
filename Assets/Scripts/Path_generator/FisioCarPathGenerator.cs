using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

public class FisioCarPathGenerator : Singleton<FisioCarPathGenerator>
{

	public const int M = 3;


	public Button[] start = new Button[M];

	public Button[] middle = new Button[M];

	public Button[] final = new Button[M];

	public Text[] diamonds_text = new Text[M];

	public Slider[] diamonds_slider = new Slider[M];

	public Slider amplitude_slider;
	public Text amplitude_text;

	public Button button_go_on;

	public Button middle_empty_button;

	public Button final_empty_button;


	int[] car_path = new int[M] { -3, -3, -3 };


	int[] n_items = new int[M];


	bool start_ok;

	bool middle_ok;

	bool final_ok;

	bool activated;

	// Use this for initialization
	void Start ()
	{ 
		ResetAll ();
		
	}

	// Update is called once per frame
	void Update ()
	{
		if (start_ok && middle_ok && final_ok && !activated) {
			activated = true;
			button_go_on.interactable = true;
		}
		
	}


	public void ResetAll ()
	{
		start_ok = false;

		middle_ok = false;

		final_ok = false;

		activated = false;



		for (int i = 0; i < M; i++) {
			start [i].interactable = true;

			middle [i].interactable = true;

			final [i].interactable = true;

			diamonds_slider [i].value = diamonds_slider [i].minValue;
			diamonds_text [i].text = diamonds_slider [i].minValue.ToString ();
		}

		button_go_on.interactable = false;
	}

	int FromCurveToIndex (int value)
	{
		return value + 1;
	}


	public void StartPressed (int i)
	{
		start_ok = true;

		car_path [0] = i;

		for (int h = 0; h < start.Length; h++) {
			start [h].interactable = true;
		}

		start [FromCurveToIndex (i)].interactable = false;

	}


	public void MiddlePressed (int i)
	{
		middle_ok = true;

		car_path [(M - 1) / 2] = i;

		for (int h = 0; h < middle.Length; h++) {
			middle [h].interactable = true;
		}

		if (!middle_empty_button.interactable) {
			ActivateButtons ();
		}


		middle [FromCurveToIndex (i)].interactable = false;

	}


	public void FinalPressed (int i)
	{
		final_ok = true;

		car_path [M - 1] = i;

		for (int h = 0; h < final.Length; h++) {
			final [h].interactable = true;
		}

		if (!final_empty_button.interactable) {
			final_empty_button.interactable = true;

			diamonds_slider [M - 1].interactable = true;
		}

		final [FromCurveToIndex (i)].interactable = false;

	}


	void ActivateButtons ()
	{

		middle_empty_button.interactable = true;

		//middle
		diamonds_slider [(M - 1) / 2].interactable = true;

		for (int h = 0; h < final.Length; h++) {
			final [h].interactable = true;
		}

		//final
		final_empty_button.interactable = true;

		diamonds_slider [M - 1].interactable = true;

		//deactivate the "continua" button
		final_ok = false;

		button_go_on.interactable = false;

		activated = false;
	}


	void DeactivateButtons ()
	{
		middle_ok = true;
		final_ok = true;

		middle_empty_button.interactable = false;

		diamonds_slider [(M - 1) / 2].interactable = false;

		for (int h = 0; h < final.Length; h++) {
			final [h].interactable = false;
		}

		final_empty_button.interactable = false;

		diamonds_slider [M - 1].interactable = false;
	}




	public void MiddleEmpty ()
	{
		DeactivateButtons ();

		for (int h = 0; h < middle.Length; h++) {
			middle [h].interactable = true;
		}

		car_path [(M - 1) / 2] = -2;
		
	}

	public void FinalEmpty ()
	{
		final_ok = true;
		for (int h = 0; h < final.Length; h++) {
			final [h].interactable = true;
		}

		final_empty_button.interactable = false;

		diamonds_slider [M - 1].interactable = false;

		car_path [M - 1] = -2;
	}



	public void ManageSlider (int index)
	{
		diamonds_text [index].text = 
			Mathf.RoundToInt (diamonds_slider [index].value)
				.ToString ();
	}



	public void SaveCarPath ()
	{
		Debug.Log (car_path [0].ToString () + " " +
		car_path [1].ToString () + " " +
		car_path [2].ToString ());

		for (int h = 0; h < M; h++) {
			n_items [h] = Mathf.RoundToInt (diamonds_slider [h].value);
			Debug.Log (n_items [h].ToString ());
		}
	}


}
