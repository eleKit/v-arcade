using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadAndDownloadDataController : MonoBehaviour
{

	/* WebConnectionController loads and downloads data automatically when the scene is loaded,
	 * instead use this if you want to load and download data manually
	 */

	/* Now i use WebConnectionController inside the Main Menu for Players and 
	 * i use this in the Main Menu for Therapists
	 */

	//public Text m_notification_text;
	public Button m_load_data_button;

	public Button m_download_data_button;

	// Use this for initialization
	void Start ()
	{
		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}





	public void LoadDataInsideTMPFolder ()
	{
		StartCoroutine (LoadDataOnWeb ());
	}

	IEnumerator LoadDataOnWeb ()
	{
		m_load_data_button.interactable = false;
		//to correctly save the data before load and then download new data
		yield return this.GetComponent<LoadDataToWeb> ().LoadData ();
		Debug.Log (" end loading on web");
		//yield return new WaitForSeconds (0.5f);
		//m_notification_text.text = "Finito!";
		m_load_data_button.interactable = true;
	}

	public void DownloadReplays ()
	{
		StartCoroutine (LoadReplaysFromWeb ());
	}

	IEnumerator LoadReplaysFromWeb ()
	{
		m_download_data_button.interactable = false;
		//to correctly save the data before load and then download new data
		yield return this.GetComponent<DownloadAllReplay> ().LoadReplayFilenames ();
		Debug.Log (" end downloading from web");
		//yield return new WaitForSeconds (0.5f);
		//m_notification_text.text = "Finito!";
		m_download_data_button.interactable = true;
	}

}
