using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FisioSkiController : MonoBehaviour {

	string flagPrefabPath = "Prefabs/Ski/Flags";
	string treePrefabPath = "Prefabs/Ski/Tree";
	bool testing, inGame, start, timer, pause; 

	int time = 3;
	bool rightHandVisible;
	Vector3 playerStartPosition;

	public GameObject info, track, player;

	string infoText = "Traccia il percorso\nutilizzando\nla mano destra";

	// Use this for initialization
	void Start () {
		playerStartPosition = player.transform.position;

	}
	
	void Update () {
		rightHandVisible = GameObject.Find ("HandController").GetComponent<HandController> ().rightHandVisible;
		if(inGame){
			if(!rightHandVisible || pause)
				Time.timeScale = 0f;
			else{
				Time.timeScale = 1f;
				if(!timer && !SkiSaveData.skiData.GetStart()){
					info.GetComponent<TextMesh>().text = time.ToString();
					InvokeRepeating("Countdown", 1f, 1f);
					timer = true;
				}
			}
			
		}
		
		if(time == 0){
			info.GetComponent<TextMesh>().text = "";
			CancelInvoke();
			time = 3;
			timer = false;
			player.GetComponent<SkiPositionTracker> ().StartTracking ();
			SkiSaveData.skiData.SetStart(true);
			if(!testing)
				InvokeRepeating("CreateEmpty", 0f, 2f);
		}
		
		//		if(Input.GetKeyDown("escape"))
		//			PauseMenu();
	}

	public bool IsInGame(){
		return inGame;
	}
	
	void Begin(){
		SkiSaveData.skiData.SetStart(false);
		player.GetComponent<Rigidbody>().detectCollisions = false;
		player.transform.position = playerStartPosition;
		info.GetComponent<TextMesh>().text = infoText;
		testing = false;
		inGame = true;
	}
	
	public void BeginTest(){
		SkiSaveData.skiData.SetStart(false);
		Vector3 temp = new Vector3 (playerStartPosition.x, - playerStartPosition.y, playerStartPosition.z);
		player.transform.position = temp;
		player.GetComponent<Rigidbody>().detectCollisions = true;
		testing = true;
		inGame = true;
	}
	
	public bool IsTesting(){
		return testing;
	}
	
	void Countdown(){
		time--;
		info.GetComponent<TextMesh>().text = time.ToString();
	}
	void Pause(bool p){
		pause = p;
	}
	
	public void CreatePath(string selectedPath){
		if(selectedPath != ""){
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
	}

	void CreateEmpty(){
		track.SendMessage ("CreateEmptyTrack");
	}
}
