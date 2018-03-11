using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FisioStarshipGameController : MonoBehaviour {

	string targetPrefabPath = "Prefabs/Plane/concretePipe";
	bool testing, inGame, start, timer, pause; 
	int time = 3;
	int visibleHands = 0;
	bool leftHandVisible, rightHandVisible;
	Color[] colors = new Color[3];

	string infoText = "Traccia il percorso utilizzando la mano destra";

	public GameObject gui, plane, info, altimeter;

	// Use this for initialization
	void Start () {
		Time.timeScale = 0;
		colors[0] = Color.yellow;
		colors[1] = Color.blue;
		colors[2] = Color.red;
		gui.SetActive (false);
		altimeter.SetActive (false);
		info.GetComponent<TextMesh>().text = "";
	}
	
	// Update is called once per frame
	void Update () {
		visibleHands = GameObject.Find ("HandController").GetComponent<HandController> ().visibleHands;
		leftHandVisible = GameObject.Find ("HandController").GetComponent<HandController> ().leftHandVisible;
		rightHandVisible = GameObject.Find ("HandController").GetComponent<HandController> ().rightHandVisible;
		if(inGame){
			if((!PlayerSaveData.playerData.GetOneHandMode() && visibleHands < 2) ||
			   (PlayerSaveData.playerData.GetOneHandMode() && !PlayerSaveData.playerData.GetRightHand() && !leftHandVisible) ||
			   (PlayerSaveData.playerData.GetOneHandMode() && PlayerSaveData.playerData.GetRightHand() && !rightHandVisible) || pause)
				Time.timeScale = 0f;
			else{
				Time.timeScale = 1f;
				if(!start && !timer){
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
			start = true;
			timer = false;
			plane.GetComponent<FisioPlaneScript>().SetStartAngles();
		}

//		if(Input.GetKeyDown("escape"))
//			PauseMenu();
	}


	public bool IsInGame(){
		return inGame;
	}

	void Begin(){
		if(!PlayerSaveData.playerData.GetOneHandMode())
			info.GetComponent<TextMesh>().text = "";
		else
			info.GetComponent<TextMesh>().text = infoText;
		testing = false;
		gui.SetActive (true);
		altimeter.SetActive (true);
		inGame = true;
	}

	public void BeginTest(){
		testing = true;
		gui.SetActive (true);
		altimeter.SetActive (true);
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
			List<Vector3> positions = PathSaveData.pathData.GetPathPositions(selectedPath);
			List<Vector3> angles = PathSaveData.pathData.GetPathAngles(selectedPath);
			for(int i = 0; i < positions.Count; i++){
				GameObject go = (GameObject)Instantiate(Resources.Load(targetPrefabPath), positions[i], Quaternion.Euler(angles[i].x, angles[i].y, angles[i].z));
				if(i < 10)
					go.name = "Target0" + i;
				else
					go.name = "Target" + i;
				go.GetComponent<Renderer>().material.color = colors[i%3];
			}
		}
	}
}
