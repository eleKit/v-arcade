using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Leap;

public class PlayerStarshipGameController : MonoBehaviour {

	static string arrowUpTexture = "Textures/Plane/Arrow_up";
	static string arrowDownTexture = "Textures/Plane/Arrow_down";
	static string arrowLeftTexture = "Textures/Plane/Arrow_left";
	static string arrowRightTexture = "Textures/Plane/Arrow_right";

	public static string transparentTexture = "Textures/Transparent";

	Vector3 startPosition;
	bool inGame, tuning, timer, pause, track, replaySelected; 
	int startObstacles, obstacleCount, points;
	string targetPrefabPath = "Prefabs/Plane/concretePipe";
	string startInfo = "Metti le mani sul leap,\ncon i palmi rivolti verso il basso.\nTienile il piu' possibile parallele al tavolo.";
	GameObject[] obstacles;
	Vector3[] positions, angles;
	float lastZ;
	float startTime;
	int time = 3;
	GameObject verticalArrow, horizontalArrow, info;
	int visibleHands = 0;
	bool leftHandVisible, rightHandVisible, sameHands;
	Color[] colors = new Color[3];
	string[] flowers = new string[3];

	
	public GameObject webcam, plane, saveManager;

	// Use this for initialization
	void Start () {
		colors[0] = Color.yellow;
		colors[1] = Color.blue;
		colors[2] = Color.red;
		flowers[0] = "Flower01";
		flowers[1] = "Flower02";
		flowers[2] = "Flower03";
		plane.GetComponent<PlayerPlaneScript> ().SetPlayer (true);
		verticalArrow = GameObject.Find ("Arrow vertical");
		horizontalArrow = GameObject.Find ("Arrow horizontal");
		info = GameObject.Find ("Info");
		//info.GetComponent<TextMesh>().text = startInfo;
		startPosition = plane.transform.position;
		Time.timeScale = 0;
		webcam.SetActive (false);
		points = 0;
		//GameObject.Find ("Punteggio").GetComponent<TextMesh> ().text = "Punteggio: " + points.ToString ();

		Debug.Log ("User:" + PlayerSaveData.playerData.GetUserName());
	}
	
	// Update is called once per frame
	void Update () {
		visibleHands = GameObject.Find ("HandController").GetComponent<HandController> ().visibleHands;
		if(visibleHands == 2){
			if(GameObject.Find ("HandController").GetComponent<HandController> ().hands[0].IsRight 
			   && GameObject.Find ("HandController").GetComponent<HandController> ().hands[1].IsRight)
				sameHands = true;
			else if(GameObject.Find ("HandController").GetComponent<HandController> ().hands[0].IsLeft 
			   && GameObject.Find ("HandController").GetComponent<HandController> ().hands[1].IsLeft)
				sameHands = true;
			else
				sameHands = false;
		}
		leftHandVisible = GameObject.Find ("HandController").GetComponent<HandController> ().leftHandVisible;
		rightHandVisible = GameObject.Find ("HandController").GetComponent<HandController> ().rightHandVisible;
		if(inGame){
			UpdateArrows();

			//Se tutte e due le mani non sono sul leap, non succede nulla.
			if((!PlayerSaveData.playerData.GetOneHandMode() && visibleHands < 2) ||
			   (!PlayerSaveData.playerData.GetOneHandMode() && sameHands) ||
			   (PlayerSaveData.playerData.GetOneHandMode() && !PlayerSaveData.playerData.GetRightHand() && !leftHandVisible) ||
			   (PlayerSaveData.playerData.GetOneHandMode() && PlayerSaveData.playerData.GetRightHand() && !rightHandVisible) || 
			   	pause){
				if(!PlayerSaveData.playerData.GetOneHandMode() && sameHands)
					info.GetComponent<TextMesh>().text = "Sono state rilevate due mani destre (o sinistre)\nper favore, riposiziona le mani sul leap";
				Time.timeScale = 0f;
			}
			else{
				Time.timeScale = 1f;
				if(!sameHands)
					info.GetComponent<TextMesh>().text = "";
				//Inizia tuning
				if(tuning){
					info.GetComponent<TextMesh>().fontSize = 110;
					info.GetComponent<TextMesh>().text = time.ToString();
					if(!timer){
						InvokeRepeating("Countdown", 1f, 1f);
						timer = true;
					}
				}
				//Allo scadere del conto alla rovescia, salva gli angoli a riposo e fa partire il gioco.
				if(tuning && time == 0){
					CancelInvoke();
					tuning = false;
					timer = false;
					time = 3;
					info.GetComponent<TextMesh>().text = "";
					info.GetComponent<TextMesh>().fontSize = 80;
					plane.GetComponent<PlayerPlaneScript> ().SetStartAngles();
					startTime = Time.time;
					Debug.Log (startTime);
					plane.GetComponent<PlayerPlaneScript> ().Go();
					if(!track)
						track = true;
				}
			}

		}

		if(SaveInfos.replay &&  (Time.time - startTime) >= PlayerSaveData.playerData.GetGameTime() && replaySelected){
			Time.timeScale = 0f;
			SendMessage("EndMenu");
			replaySelected = false;
		}

		if(startObstacles > 0 && plane.transform.position.z < lastZ){
			EndGame ();
			CancelInvoke();
		}
	}

	void FixedUpdate(){
		if(track){
			InvokeRepeating("SaveData", 0f, 0.2f);
			track = false;
		}
	}

	//Rimuove i menu e attiva la scelta della modalità.
	public void Begin(){
		SaveInfos.ResetData ();
		//webcam.SetActive (true);
		plane.GetComponent<PlayerPlaneScript> ().Stop();
		inGame = true;
		tuning = true;
		timer = false;
		pause = false;
	}


	//Resetta il livello.
	public void ResetPath(){
		points = 0;
		GameObject.Find ("Punteggio").GetComponent<TextMesh> ().text = "Punteggio: " + points.ToString ();
		Reload ();
		plane.transform.position = startPosition;
		plane.GetComponent<PlayerPlaneScript> ().ResetAngles ();
		Begin ();
	}

	public void SetObstacleCount(int count){
		startObstacles = count;
		obstacleCount = startObstacles;
	}


	public void HitTarget(){
		obstacleCount--;
		points += PathSaveData.pathData.GetTargetPoints();
		GameObject.Find ("Punteggio").GetComponent<TextMesh> ().text = "Punteggio: " + points.ToString ();
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
				//go.renderer.material.color = colors[i%3];
				int co = i%3;
				go.transform.FindChild(flowers[co]).gameObject.SetActive(true);
			}
			GameObject[] obst = GameObject.FindGameObjectsWithTag("Obstacle").OrderBy( go => go.name ).ToArray();
			SetObstacleCount(obst.Length);
			PlayerSaveData.playerData.SetTotObstacles (obst.Length);
			obstacles = obst;
			lastZ = obst[obst.Length -1].transform.position.z;
			if(SaveInfos.replay)
				plane.GetComponent<PlayerPlaneScript> ().SetTunings();
		}
	}


	// Ricarica il percorso
	public void Reload(){
		GameObject[] obs = GameObject.FindGameObjectsWithTag ("Obstacle");
		foreach(GameObject ob in obs){
			Destroy(ob);
		}
		CreatePath(PlayerSaveData.playerData.GetCurrentPathName());
		tuning = false;
	}

	//Gestisce le frecce che indicano la direzione da seguire quando
	//l'aereo è fuori dal raggio dell'ostacolo successivo.
	void UpdateArrows(){
		if(obstacleCount > 0){
			GameObject nextObstacle = obstacles[startObstacles - obstacleCount];
			if(nextObstacle){
				float minX = nextObstacle.renderer.bounds.min.x; 
				float maxX = nextObstacle.renderer.bounds.max.x;
				float minY = nextObstacle.renderer.bounds.min.y; 
				float maxY = nextObstacle.renderer.bounds.max.y;
				float planeX = plane.transform.position.x;
				float planeY = plane.transform.position.y;

				if(planeX < minX)
					horizontalArrow.renderer.material.mainTexture = (Texture) Resources.Load (arrowLeftTexture);
				else if(planeX > maxX)
					horizontalArrow.renderer.material.mainTexture = (Texture) Resources.Load (arrowRightTexture);
				else
					horizontalArrow.renderer.material.mainTexture = (Texture) Resources.Load (transparentTexture);

				if(planeY < minY)
					verticalArrow.renderer.material.mainTexture = (Texture) Resources.Load (arrowUpTexture);
				else if(planeY > maxY)
					verticalArrow.renderer.material.mainTexture = (Texture) Resources.Load (arrowDownTexture);
				else
					verticalArrow.renderer.material.mainTexture = (Texture) Resources.Load (transparentTexture);
			}
		}
	}

	void Countdown(){
		time--;
		info.GetComponent<TextMesh>().text = time.ToString();
	}

	//Restituisce il prossimo ostacolo che l'aereo dovrà attraversare
	// Se non ci sono più ostacoli, restituisce un GameObject vuoto con posizione (0,0,0)
	public GameObject NextObstacle(){
		if(obstacleCount > 0)
			return obstacles[startObstacles - obstacleCount];
		else{
			GameObject obj = new GameObject();
			obj.transform.position = new Vector3(0f,0f,0f);
			return obj;
		}

	}

	public void SaveToXML(){
		PlayerSaveData.playerData.SetGameTime (Time.time - startTime);
		saveManager.SendMessage ("SaveHandsToFile");
	}

	void SaveData(){
		saveManager.SendMessage ("SaveData");
	}

	void Pause(){
		pause = true;
	}

	void EndGame(){
		pause = true;
		PlayerSaveData.playerData.SetScore (points);
		PathSaveData.pathData.UpdateHighscore(PlayerSaveData.playerData.GetCurrentPathName(),PlayerSaveData.playerData.GetUserName(), PlayerSaveData.playerData.GetScore());
		GeneralSaveData.generalData.UpdatePlayerPlaneHighscore(PlayerSaveData.playerData.GetUserName(), PlayerSaveData.playerData.GetCurrentPathName(), PlayerSaveData.playerData.GetScore());
		Time.timeScale = 0f;
		SendMessage ("EndMenu");
	}

	public bool GetTrack(){
		return track;
	}

	public void Go(){
		startTime = Time.time;
		Debug.Log (startTime);
		replaySelected = true;
		plane.GetComponent<PlayerPlaneScript> ().Go();
	}

	public void SetStartAngles(Vector3 sta){
		plane.GetComponent<PlayerPlaneScript> ().SetStartAngles(sta);
	}
}
