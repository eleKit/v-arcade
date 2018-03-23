using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TuningController : MonoBehaviour
{

	static int maxTime = 5;

	bool startTuning, startMaxVertical, startMinVertical, startMaxHorizontal, startMinHorizontal, endTuning, incoherent;
	bool isTuning, timer;
	float leftMinVertical, leftMaxVertical, leftMinHorizontal, leftMaxHorizontal;
	float rightMinVertical, rightMaxVertical, rightMinHorizontal, rightMaxHorizontal;
	float distance;
	int time;
	float maxDistance = 250f;
	float minDistance = 150f;
	GameObject rightHand, leftHand;

	public GameObject tuningInfos, hands, acceptText, redoText, handController, background, reText, leText, rfText, lfText, rudText, rrdText, ludText, lrdText;

	// Use this for initialization
	void Start ()
	{
		acceptText.SetActive (false);
		redoText.SetActive (false);
		time = maxTime;
	}
	
	// Update is called once per frame
	void Update ()
	{
//		if(Input.GetKeyDown(KeyCode.Space))
//			Time.timeScale = 1f;


		if (handController.GetComponent<HandController> ().visibleHands == 2) {
			distance = Mathf.Abs (handController.GetComponent<HandController> ().hands [0].PalmPosition.x - handController.GetComponent<HandController> ().hands [1].PalmPosition.x);
		}

		if (PlayerSaveData.playerData.GetFirstTimePlaying () && background.GetComponent<Renderer> ().isVisible && !isTuning) {
			tuningInfos.GetComponent<TuningInfos> ().SetFirstTimeText ();
			isTuning = true;
			startTuning = true;
		}
		if (PlayerSaveData.playerData.GetTuningTooOld () && background.GetComponent<Renderer> ().isVisible && !isTuning) {
			tuningInfos.GetComponent<TuningInfos> ().SetLongAgoText ();
			isTuning = true;
			startTuning = true;
		}

		if (startTuning) {
			if (handController.GetComponent<HandController> ().visibleHands == 2) {
				if (distance >= minDistance && distance <= maxDistance) {
					startMaxVertical = true;
					hands.GetComponent<AnimatedTextureExtendedUV> ().rowNumber = 1;
					startTuning = false;
				}
			}
		}

		if (startMaxVertical) {
			tuningInfos.GetComponent<TuningInfos> ().SetHandsUpText ();
			if (handController.GetComponent<HandController> ().visibleHands == 2) {
				if (!timer) {
					GameObject.Find ("Countdown Text").GetComponent<TextMesh> ().text = time.ToString ();
					InvokeRepeating ("Countdown", 1f, 1f);
					timer = true;
				}
			}
		}

		if (startMinVertical) {
			tuningInfos.GetComponent<TuningInfos> ().SetHandsDownText ();
			if (handController.GetComponent<HandController> ().visibleHands == 2) {
				if (!timer) {
					GameObject.Find ("Countdown Text").GetComponent<TextMesh> ().text = time.ToString ();
					InvokeRepeating ("Countdown", 1f, 1f);
					timer = true;
				}
			}
		}

		if (startMaxHorizontal) {
			tuningInfos.GetComponent<TuningInfos> ().SetHandsRightText ();
			if (handController.GetComponent<HandController> ().visibleHands == 2) {
				if (!timer) {
					GameObject.Find ("Countdown Text").GetComponent<TextMesh> ().text = time.ToString ();
					InvokeRepeating ("Countdown", 1f, 1f);
					timer = true;
				}
			}
		}

		if (startMinHorizontal) {
			tuningInfos.GetComponent<TuningInfos> ().SetHandsLeftText ();
			if (handController.GetComponent<HandController> ().visibleHands == 2) {
				if (!timer) {
					GameObject.Find ("Countdown Text").GetComponent<TextMesh> ().text = time.ToString ();
					InvokeRepeating ("Countdown", 1f, 1f);
					timer = true;
				}
			}
		}


		if (time == 0 && startMaxVertical) {
			CancelInvoke ();
			GameObject.Find ("Countdown Text").GetComponent<TextMesh> ().text = "";
			time = maxTime;
			rightMaxVertical = GameObject.Find ("HandController").GetComponent<RightXRotationStatsScript> ().GetXExtension ();
			leftMaxVertical = GameObject.Find ("HandController").GetComponent<LeftXRotationStatsScript> ().GetXExtension ();
			hands.GetComponent<AnimatedTextureExtendedUV> ().rowNumber = 2;
			startMaxVertical = false;
			startMinVertical = true;
			timer = false;

		}

		if (time == 0 && startMinVertical) {
			CancelInvoke ();
			GameObject.Find ("Countdown Text").GetComponent<TextMesh> ().text = "";
			time = maxTime;
			rightMinVertical = GameObject.Find ("HandController").GetComponent<RightXRotationStatsScript> ().GetXExtension ();
			leftMinVertical = GameObject.Find ("HandController").GetComponent<LeftXRotationStatsScript> ().GetXExtension ();
			hands.GetComponent<AnimatedTextureExtendedUV> ().rowNumber = 3;
			startMinVertical = false;
			startMaxHorizontal = true;
			timer = false;
		}

		if (time == 0 && startMaxHorizontal) {
			CancelInvoke ();
			GameObject.Find ("Countdown Text").GetComponent<TextMesh> ().text = "";
			time = maxTime;
			rightMaxHorizontal = GameObject.Find ("HandController").GetComponent<RightYRotationStatsScript> ().GetYExtension ();
			leftMaxHorizontal = GameObject.Find ("HandController").GetComponent<LeftYRotationStatsScript> ().GetYExtension ();
			hands.GetComponent<AnimatedTextureExtendedUV> ().rowNumber = 4;
			startMaxHorizontal = false;
			startMinHorizontal = true;
			timer = false;
		}

		if (time == 0 && startMinHorizontal) {
			CancelInvoke ();
			GameObject.Find ("Countdown Text").GetComponent<TextMesh> ().text = "";
			time = maxTime;
			rightMinHorizontal = GameObject.Find ("HandController").GetComponent<RightYRotationStatsScript> ().GetYExtension ();
			leftMinHorizontal = GameObject.Find ("HandController").GetComponent<LeftYRotationStatsScript> ().GetYExtension ();
			startMinHorizontal = false;
			timer = false;
			hands.GetComponent<AnimatedTextureExtendedUV> ().rowNumber = 0;

			Debug.Log ("Destra - MaxVerticale: " + rightMaxVertical + " MinVerticale: " + rightMinVertical + " MaxOrizzontale: " + rightMaxHorizontal + " MinOrizzontale: " + rightMinHorizontal);
			Debug.Log ("Sinistra - MaxVerticale: " + leftMaxVertical + " MinVerticale: " + leftMinVertical + " MaxOrizzontale: " + leftMaxHorizontal + " MinOrizzontale: " + leftMinHorizontal);
			if (CheckCoherence ())
				endTuning = true;
			else {
				hands.SetActive (false);
				tuningInfos.GetComponent<TuningInfos> ().SetIncoherentText ();
				redoText.SetActive (true);
			}

		}

		if (endTuning) {
			tuningInfos.GetComponent<TuningInfos> ().SetEndText ();
			hands.SetActive (false);
			StampStats ();
			acceptText.SetActive (true);
			redoText.SetActive (true);
		}
	}


	public void SaveStats ()
	{
		GeneralSaveData.generalData.UpdatePlayerTunings (PlayerSaveData.playerData.GetUserName (), leftMinHorizontal, leftMaxHorizontal,
			leftMinVertical, leftMaxVertical, rightMinHorizontal, rightMaxHorizontal, rightMinVertical, rightMaxVertical);
		PlayerSaveData.playerData.SetPlayer (GeneralSaveData.generalData.GetPlayer (PlayerSaveData.playerData.GetUserName ()));
		SceneManager.LoadSceneAsync ("Main_Menu");
	}

	void Countdown ()
	{
		time--;
		GameObject.Find ("Countdown Text").GetComponent<TextMesh> ().text = time.ToString ();
	}

	public void Redo ()
	{
		endTuning = false;
		HideStats ();
		tuningInfos.GetComponent<TuningInfos> ().SetRedoText ();
		hands.SetActive (true);
		startTuning = true;
	}

	void StampStats ()
	{
		reText.GetComponent<TextMesh> ().text = "Estensione destra: " + Mathf.Abs (rightMaxVertical).ToString ("n2") + "°";
		rfText.GetComponent<TextMesh> ().text = "Flessione destra: " + Mathf.Abs (rightMinVertical).ToString ("n2") + "°";
		rudText.GetComponent<TextMesh> ().text = "Dev ulnare destra: " + Mathf.Abs (rightMaxHorizontal).ToString ("n2") + "°";
		rrdText.GetComponent<TextMesh> ().text = "Dev radiale destra: " + Mathf.Abs (rightMinHorizontal).ToString ("n2") + "°";
		leText.GetComponent<TextMesh> ().text = "Estensione sinistra: " + Mathf.Abs (leftMaxVertical).ToString ("n2") + "°";
		lfText.GetComponent<TextMesh> ().text = "Flessione sinistra: " + Mathf.Abs (leftMinVertical).ToString ("n2") + "°";
		ludText.GetComponent<TextMesh> ().text = "Dev ulnare sinistra: " + Mathf.Abs (leftMinHorizontal).ToString ("n2") + "°";
		lrdText.GetComponent<TextMesh> ().text = "Dev radiale sinistra: " + Mathf.Abs (leftMaxHorizontal).ToString ("n2") + "°";
	}

	void HideStats ()
	{
		reText.GetComponent<TextMesh> ().text = "";
		rfText.GetComponent<TextMesh> ().text = "";
		rudText.GetComponent<TextMesh> ().text = "";
		rrdText.GetComponent<TextMesh> ().text = "";
		leText.GetComponent<TextMesh> ().text = "";
		lfText.GetComponent<TextMesh> ().text = "";
		ludText.GetComponent<TextMesh> ().text = "";
		lrdText.GetComponent<TextMesh> ().text = "";
	}

	bool CheckCoherence ()
	{
		if (rightMaxVertical * rightMinVertical > 0 || rightMinHorizontal * rightMaxHorizontal > 0
		    || leftMinVertical * leftMaxVertical > 0 || leftMinHorizontal * leftMaxHorizontal > 0)
			return false;
		else if (rightMaxVertical == 0 || rightMinVertical == 0 || rightMinHorizontal == 0 || rightMaxHorizontal == 0
		         || leftMinVertical == 0 || leftMaxVertical == 0 || leftMinHorizontal == 0 || leftMaxHorizontal == 0)
			return false;
		else
			return true;
	}
}
