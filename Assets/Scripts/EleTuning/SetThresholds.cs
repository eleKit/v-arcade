using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SetThresholds : MonoBehaviour
{

	[Header ("Left result sliders and text")]
	public Slider left_flexion_extension_slider;
	public Text left_flexion_extension_text;
	public Slider left_ulnar_radial_slider;
	public Text left_ulnar_radial_text;



	[Header ("Right result sliders and text")]
	public Slider right_flexion_extension_slider;
	public Text right_flexion_extension_text;
	public Slider right_ulnar_radial_slider;
	public Text right_ulnar_radial_text;

	public GameObject m_menu_canvas;
	public GameObject m_settings_canvas;

	// Use this for initialization
	void Start ()
	{
		left_flexion_extension_slider.value = GlobalPlayerData.globalPlayerData.player_data.left_pitch_scale * Mathf.Rad2Deg;
		left_ulnar_radial_slider.value = GlobalPlayerData.globalPlayerData.player_data.left_yaw_scale * Mathf.Rad2Deg;
		right_flexion_extension_slider.value = GlobalPlayerData.globalPlayerData.player_data.right_pitch_scale * Mathf.Rad2Deg;
		right_ulnar_radial_slider.value = GlobalPlayerData.globalPlayerData.player_data.right_yaw_scale * Mathf.Rad2Deg;


		LeftFlexionExtensionSlider ();
		LeftUlnarRadialSlider ();
		RightFlexionExtensionSlider ();
		RightUlnarRadialSlider ();

		LoadMain ();
	}


	public void LeftFlexionExtensionSlider ()
	{
		left_flexion_extension_text.text = left_flexion_extension_slider.value.ToString ("N2");
	}



	public void LeftUlnarRadialSlider ()
	{
		left_ulnar_radial_text.text = left_ulnar_radial_slider.value.ToString ("N2");
	}

	public void RightFlexionExtensionSlider ()
	{
		right_flexion_extension_text.text = right_flexion_extension_slider.value.ToString ("N2");
	}



	public void RightUlnarRadialSlider ()
	{
		right_ulnar_radial_text.text = right_ulnar_radial_slider.value.ToString ("N2");
	}


	public void SaveDataAndLoadMain ()
	{
		//save data in the global player object
		GlobalPlayerData.globalPlayerData.player_data.left_pitch_scale = left_flexion_extension_slider.value * Mathf.Deg2Rad;
		GlobalPlayerData.globalPlayerData.player_data.left_yaw_scale = left_ulnar_radial_slider.value * Mathf.Deg2Rad;

		GlobalPlayerData.globalPlayerData.player_data.right_pitch_scale = right_flexion_extension_slider.value * Mathf.Deg2Rad;
		GlobalPlayerData.globalPlayerData.player_data.right_yaw_scale = right_ulnar_radial_slider.value * Mathf.Deg2Rad;

		LoadMain ();

	}


	void LoadMain ()
	{
		ClearScreens ();
		m_menu_canvas.SetActive (true);
	}

	public void LoadSettings ()
	{
		ClearScreens ();
		m_settings_canvas.SetActive (true);
	}


	public void ClearScreens ()
	{
		if (m_settings_canvas != null)
			m_settings_canvas.SetActive (false);

		if (m_menu_canvas != null)
			m_menu_canvas.SetActive (false);
		
	}
}
