using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using UnityEngine.UI;

public class WebConnectionController : MonoBehaviour
{
	public Button go_on_button;

	public Text m_welcome_text;

	// Use this for initialization
	void Start ()
	{
		go_on_button.interactable = false;

		string HtmlText = GetHtmlFromUri ("http://google.com");
		if (HtmlText == "") {
			Debug.Log ("no connection");
			m_welcome_text.text = "Benvenuti!";
			go_on_button.interactable = true;

		} else if (!HtmlText.Contains ("schema.org/WebPage")) {
			//Redirecting since the beginning of googles html contains that 
			//phrase and it was not found
			m_welcome_text.text = "Benvenuti!";
			go_on_button.interactable = true;
		} else {
			Debug.Log ("connection success");

			StartCoroutine (DoWebWork ());

		}

	}

	// Update is called once per frame
	void Update ()
	{

	}

	IEnumerator DoWebWork ()
	{
		m_welcome_text.text = "Scaricamento dati...";
		yield return this.GetComponent<LoadNicknamesFromWeb> ().LoadFileOfNicknames ();
		yield return this.GetComponent<LoadPathsFromWeb> ().LoadFilenames ();
		yield return this.GetComponent<LoadDataToWeb> ().LoadData ();
		Debug.Log (" end downloading paths");
		yield return new WaitForSeconds (0.5f);
		m_welcome_text.text = "Benvenuti!";
		go_on_button.interactable = true;
	}

	/* from https://answers.unity.com/questions/567497/how-to-100-check-internet-availability.html
	 */

	public string GetHtmlFromUri (string resource)
	{
		string html = string.Empty;
		HttpWebRequest req = (HttpWebRequest)WebRequest.Create (resource);
		try {
			using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse ()) {
				bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
				if (isSuccess) {
					using (StreamReader reader = new StreamReader (resp.GetResponseStream ())) {
						//We are limiting the array to 80 so we don't have
						//to parse the entire html document feel free to 
						//adjust (probably stay under 300)
						char[] cs = new char[80];
						reader.Read (cs, 0, cs.Length);
						foreach (char ch in cs) {
							html += ch;
						}
					}
				}
			}
		} catch {
			return "";
		}
		return html;
	}
}
