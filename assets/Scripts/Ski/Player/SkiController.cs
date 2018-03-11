using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SkiController : MonoBehaviour {

	int time = 3;
	float trackStartY;
	float startTime;
	bool tracking, inGame, wait, replaySelected;
	bool leftHandVisible, rightHandVisible;
	string palmDown = "Metti la mano sul leap\ncol palmo rivolto\nverso il basso";
	string palmVert = "Metti la mano sul leap\ncol palmo rivolto\nverso il basso\ne poi ruotala come se\ndovessi battere le mani";

	public GameObject timer, score, track, player, saveManager, handController, info;

	// Use this for initialization
	void Start () {
		timer.SetActive(false);
		trackStartY = track.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		score.GetComponent<TextMesh> ().text = "PUNTEGGIO\n" + PlayerSaveData.playerData.GetScore();
		leftHandVisible = handController.GetComponent<HandController> ().leftHandVisible;
		rightHandVisible = handController.GetComponent<HandController> ().rightHandVisible;

		if(inGame){
			if((PlayerSaveData.playerData.GetOneHandMode() && !PlayerSaveData.playerData.GetRightHand() && !leftHandVisible) ||
			   (PlayerSaveData.playerData.GetOneHandMode() && PlayerSaveData.playerData.GetRightHand() && !rightHandVisible)){
				Time.timeScale = 0f;
			}
			else{
				Time.timeScale = 1f;
				if(wait){
					info.GetComponent<TextMesh>().text = "";
					InvokeRepeating ("Countdown", 1, 1);
					wait = false;
				}
				if(timer != null)
					timer.GetComponent<TextMesh> ().text = time.ToString();

				if(time == 0){
					CancelInvoke();
					timer.SetActive(false);
					time = 3;
					SkiSaveData.skiData.SetStart(true);
					player.GetComponent<AudioSource>().Play ();
					startTime = Time.time;
					if(!tracking)
						tracking = true;
				}
			}
		}

		if(SaveInfos.replay &&  (Time.time - startTime) >= PlayerSaveData.playerData.GetGameTime() && replaySelected){
			SendMessage("EndMenu");
			SkiSaveData.skiData.SetStart(false);
			replaySelected = false;
		}
	}

	void FixedUpdate(){
		if(tracking){
			InvokeRepeating("SaveData", 0f, 0.2f);
			tracking = false;
		}
	}

	void Begin(){
		track.SendMessage ("Init");
		PlayerSaveData.playerData.SetScore (0);
		track.SendMessage ("CreateTrack");
		timer.SetActive(true);
		SaveInfos.ResetData ();
		if(SkiSaveData.skiData.GetSlapMode())
			info.GetComponent<TextMesh>().text = palmVert;
		else
			info.GetComponent<TextMesh>().text = palmDown;
		inGame = true;
		wait = true;
	}

	void BeginSaved(){
		PlayerSaveData.playerData.SetScore (0);
		timer.SetActive(true);
		SaveInfos.ResetData ();
		if(SkiSaveData.skiData.GetSlapMode())
			info.GetComponent<TextMesh>().text = palmVert;
		else
			info.GetComponent<TextMesh>().text = palmDown;
		inGame = true;
		wait = true;
	}

	void Countdown(){
		time--;
	}

	void Pause(){
		SkiSaveData.skiData.SetStart(false);
	}

	void UnPause(){
		timer.SetActive(true);
		InvokeRepeating ("Countdown", 1, 1);
	}


	void Reload(){
		Vector3 temp = new Vector3 (track.transform.position.x, trackStartY, track.transform.position.z);
		track.transform.position = temp;
		player.GetComponent<SkiPlayerScript> ().BackToCenter ();
		PlayerSaveData.playerData.SetScore (0);
		timer.SetActive(true);
		SaveInfos.ResetData ();
		InvokeRepeating ("Countdown", 1, 1);
	}

	void StartReplay(){
		PlayerSaveData.playerData.SetScore (0);
		inGame = true;
		tracking = false;
		SkiSaveData.skiData.SetStart(true);
		player.GetComponent<AudioSource>().Play ();
	}

	public void SaveToXML(){
		PlayerSaveData.playerData.SetGameTime (Time.time - startTime);
		saveManager.SendMessage ("SaveHandsToFile");
	}
	
	void SaveData(){
		saveManager.SendMessage ("SaveData");
	}

	public void StopTracking(){
		tracking = false;
	}

	public void CreatePath(string selectedPath){
		if(selectedPath != "" && !PlayerSaveData.playerData.GetRandomPath()){
			List<Vector3> localPositions = SkiSaveData.skiData.GetPathLocalPositions(selectedPath);
			List<float> xPoss = SkiSaveData.skiData.GetPathPoss(selectedPath);
			
			if(SkiSaveData.skiData.GetRandomPath(selectedPath)){
				track.SendMessage("CreateSavedRandomPath", xPoss);
			}
			
			else if(SkiSaveData.skiData.GetPathStepMode(selectedPath)){
				track.SendMessage("CreateSavedTreeTrack", localPositions);
			}
			else{
				track.SendMessage("CreateSavedFlagTrack", localPositions);
			}
			SkiSaveData.skiData.SetStepMode(SkiSaveData.skiData.GetPathStepMode(selectedPath));
		}
		else if(PlayerSaveData.playerData.GetRandomPath()){
			track.SendMessage ("Init");
			track.SendMessage("CreateTrack");
			startTime = Time.time;
			replaySelected = true;
		}
		if(SaveInfos.replay){
			StartReplay ();
		}
	}
}
