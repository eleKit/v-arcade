using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

public class FisioCarPathGenerator : Singleton<FisioCarPathGenerator>
{

	public Button[] start;

	public Button[] middle;

	public Button[] final;

	public Slider start_slider;
	public Text start_diamonds_text;

	public Slider middle_slider;
	public Text middle_diamonds_text;

	public Slider final_slider;
	public Text final_diamonds_text;

	public Slider amplitude_slider;
	public Text amplitude_text;

	public Button button_go_on;

	public Button middle_empty_button;

	public Button final_empty_button;

	public const int M = 3;


	int[] temporary_car_path = new int[M];


	int[] car_path = new int[M];

	int[] n_items = new int[M];

	// Use this for initialization
	void Start ()
	{ 

	
		for (int i = 0; i < start.Length; i++) {
			start [i].interactable = true;
		}

		for (int i = 0; i < middle.Length; i++) {
			middle [i].interactable = true;
		}

		for (int i = 0; i < final.Length; i++) {
			final [i].interactable = true;
		}

		button_go_on.interactable = false;

		start_slider.interactable = true;
		start_slider.value = start_slider.minValue;
		start_diamonds_text.text = start_slider.minValue.ToString ();

		middle_slider.interactable = true;
		middle_slider.value = middle_slider.minValue;
		middle_diamonds_text.text = middle_slider.minValue.ToString ();

		final_slider.interactable = true;
		final_slider.value = final_slider.minValue;
		final_diamonds_text.text = final_slider.minValue.ToString ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}



	int FromCurveToIndex (int value)
	{
		return value + 1;
	}


	public void StartPressed (int i)
	{
		temporary_car_path [0] = i;

		for (int h = 0; h < start.Length; h++) {
			start [h].interactable = true;
		}

		start [FromCurveToIndex (i)].interactable = false;

	}


	public void MiddlePressed (int i)
	{
		temporary_car_path [1] = i;

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
		temporary_car_path [2] = i;

		for (int h = 0; h < final.Length; h++) {
			final [h].interactable = true;
		}

		if (!final_empty_button.interactable) {
			final_empty_button.interactable = true;

			final_slider.interactable = true;
		}

		final [FromCurveToIndex (i)].interactable = false;

	}


	void ActivateButtons ()
	{

		middle_empty_button.interactable = true;

		middle_slider.interactable = true;

		for (int h = 0; h < final.Length; h++) {
			if (FromCurveToIndex (temporary_car_path [2]) != h)
				final [h].interactable = true;
		}

		final_empty_button.interactable = true;

		final_slider.interactable = true;
	}


	void DeactivateButtons ()
	{

		middle_empty_button.interactable = false;

		middle_slider.interactable = false;

		for (int h = 0; h < final.Length; h++) {
			final [h].interactable = false;
		}

		final_empty_button.interactable = false;

		final_slider.interactable = false;
	}




	public void MiddleEmpty ()
	{
		DeactivateButtons ();

		for (int h = 0; h < middle.Length; h++) {
			middle [h].interactable = true;
		}

		temporary_car_path [1] = -2;
		
	}

	public void FinalEmpty ()
	{
		
		for (int h = 0; h < final.Length; h++) {
			final [h].interactable = true;
		}

		final_empty_button.interactable = false;

		final_slider.interactable = false;

		temporary_car_path [2] = -2;
	}



	public void ManageStartSlider (float value)
	{
		start_diamonds_text.text = Mathf.RoundToInt (value).ToString ();
	}

	public void ManageMiddleSlider (float value)
	{
		middle_diamonds_text.text = Mathf.RoundToInt (value).ToString ();
	}

	public void ManageFinalSlider (float value)
	{
		final_diamonds_text.text = Mathf.RoundToInt (value).ToString ();
	}


}
