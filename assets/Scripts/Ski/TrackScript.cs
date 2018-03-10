using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackScript : MonoBehaviour {


	float hardLocalPos = 3.125f;
	float mediumLocalPos = 2f;
	float easyLocalPos = 1.5f;
	float speed;
	bool start;
	int numberOfObstacles;
	int offset = 0;
	int seed;
	int sinX = 0;
	int mulFactor;
	string pieceOfTrack = "Prefabs/Ski/Ski_plane";
	string emptyPieceOfTrack = "Prefabs/Ski/Empty_ski_plane";
	string finishLine = "Prefabs/Ski/Finish_Line";
	string treeGenerator = "Prefabs/Ski/Tree_Generator";
	string flagPrefabPath = "Prefabs/Ski/Flags";
	string treePrefabPath = "Prefabs/Ski/Tree";
	List<GameObject> piecesGos = new List<GameObject>();
	float leftGuide, rightGuide, centerGuide, flagLocalPosRange;
	MersenneTwister mst;

	public int obstaclesPerPiece = 2;
	public float pieceOffset = -15f;
	public GameObject player;

	void Awake(){
		seed = Random.seed;
	}

	// Use this for initialization
	void Start () {
		speed = SkiSaveData.skiData.GetTrackSpeed ();
		numberOfObstacles = SkiSaveData.skiData.GetNumberOfObstacles ();
		leftGuide = -1.75f;
		rightGuide = 1.75f;
		centerGuide = 0f;
//		CreateTrack ();
	}

	void Init(){
		int gameDiff = PlayerSaveData.playerData.GetSkiDifficulty ();
		switch(gameDiff){
		case 1:
			flagLocalPosRange = easyLocalPos;
			break;
		case 2:
			flagLocalPosRange = mediumLocalPos;
			break;
		case 3:
			flagLocalPosRange = hardLocalPos;
			break;
		}
	}

	// Update is called once per frame
	void Update () {
		start = SkiSaveData.skiData.GetStart();
	}

	void FixedUpdate(){
		if(start){
			rigidbody.velocity = transform.TransformDirection(Vector3.up) * speed;
		}
		else
			rigidbody.velocity = transform.TransformDirection(Vector3.zero);
	}

	void CreateEmptyTrack(){
		float localY = pieceOffset * (offset+1);
		Vector3 pos = new Vector3(0f, localY, 0f);
		GameObject pi = (GameObject) Instantiate(Resources.Load(emptyPieceOfTrack), Vector3.zero, Quaternion.identity);
		pi.transform.parent = gameObject.transform;
		pi.transform.localPosition = pos;
		if(offset < 10)
			pi.name = "Ski_plane0" + offset;
		else
			pi.name= "Ski_plane" + offset;
		offset++;
	}

	void CreateTrack(){
		if(!SaveInfos.replay){
			PlayerSaveData.playerData.SetRandomPath(true);
			PlayerSaveData.playerData.SetSeed(seed);
			mst = new MersenneTwister ((uint)seed);
		}
		else
			mst = new MersenneTwister ((uint)PlayerSaveData.playerData.GetSeed());

		mulFactor = (int) mst.NextUInt ((uint)3, (uint)8);
		float localY = 0f;
		int pieces = numberOfObstacles / obstaclesPerPiece;
		PlayerSaveData.playerData.SetTotObstacles (numberOfObstacles);
		for(int i = 0; i <= pieces; i++){
			localY =  pieceOffset * (i+1);
			Vector3 pos = new Vector3(gameObject.transform.position.x, localY, gameObject.transform.position.z);
			GameObject pi = (GameObject) Instantiate(Resources.Load(pieceOfTrack), pos, Quaternion.identity);
			pi.transform.parent = gameObject.transform;
			if(i < 10)
				pi.name = "Ski_plane0" + i;
			else
				pi.name= "Ski_plane" + i;
			//pi.BroadcastMessage("CreatePieceOfTrack", SkiSaveData.skiData.GetStepMode());
			float tempPos = Mathf.Sin(sinX*mulFactor) * flagLocalPosRange;
			Debug.Log (tempPos);
			pi.BroadcastMessage("CreateFirstFlags", tempPos);
			sinX++;
			tempPos = Mathf.Sin(sinX*mulFactor) * flagLocalPosRange;
			Debug.Log (tempPos);
			pi.BroadcastMessage("CreateSecondFlags", tempPos);
			sinX++;
		}
		localY +=  pieceOffset;
		Vector3 posf = new Vector3(gameObject.transform.position.x, localY, gameObject.transform.position.z);
		GameObject fi = (GameObject) Instantiate(Resources.Load(finishLine), posf, Quaternion.identity);
		fi.transform.parent = transform;
	}

	void CreateSavedRandomPath(List<float> xPoss){
		float localY = 0f;
		int obstacles = xPoss.Count;
		int pieces = obstacles / obstaclesPerPiece;
		PlayerSaveData.playerData.SetTotObstacles (obstacles);
		int posCount = 0;
		for(int i = 0; i <= pieces; i++){
			localY =  pieceOffset * (i+1);
			Vector3 pos = new Vector3(gameObject.transform.position.x, localY, gameObject.transform.position.z);
			GameObject pi = (GameObject) Instantiate(Resources.Load(pieceOfTrack), pos, Quaternion.identity);
			pi.transform.parent = gameObject.transform;
			if(i < 10)
				pi.name = "Ski_plane0" + i;
			else
				pi.name= "Ski_plane" + i;
			//pi.BroadcastMessage("CreatePieceOfTrack", SkiSaveData.skiData.GetStepMode());
			if(posCount < xPoss.Count){
				pi.BroadcastMessage("CreateFirstFlags", xPoss[posCount]);
				posCount++;
			}
			if(posCount < xPoss.Count){
				pi.BroadcastMessage("CreateSecondFlags", xPoss[posCount]);
				posCount++;
			}
		}
		localY +=  pieceOffset;
		Vector3 posf = new Vector3(gameObject.transform.position.x, localY, gameObject.transform.position.z);
		GameObject fi = (GameObject) Instantiate(Resources.Load(finishLine), posf, Quaternion.identity);
		fi.transform.parent = transform;
	}

	void CreateSavedTreeTrack(List<Vector3> localPositions){
		float localY = 0f;
		float lastY = localPositions [localPositions.Count - 1].y;
		int pieces = Mathf.CeilToInt(lastY / pieceOffset);
		//Crea percorso vuoto
		for(int i = 0; i <= pieces; i++){
			localY =  pieceOffset * (i+1);
			Vector3 pos = new Vector3(gameObject.transform.position.x, localY, gameObject.transform.position.z);
			GameObject pi = (GameObject) Instantiate(Resources.Load(emptyPieceOfTrack), pos, Quaternion.identity);
			pi.transform.parent = gameObject.transform;
			if(i < 10)
				pi.name = "Ski_plane0" + i;
			else
				pi.name= "Ski_plane" + i;
		}
		localY +=  pieceOffset;
		Vector3 posf = new Vector3(gameObject.transform.position.x, localY, gameObject.transform.position.z);
		GameObject fi = (GameObject) Instantiate(Resources.Load(finishLine), posf, Quaternion.identity);
		fi.transform.parent = transform;
		//Aggiunge tree_generator
		for(int j = 0; j < localPositions.Count; j++){
			GameObject tg = (GameObject) Instantiate(Resources.Load(treeGenerator), Vector3.zero, Quaternion.identity);
			Vector3 loc = new Vector3(0f, localPositions[j].y, localPositions[j].z);
			tg.transform.parent = gameObject.transform;
			tg.transform.localPosition = loc;
			tg.GetComponent<TreeGenerator>().flagParent.SetActive(false);
			GameObject trees = tg.GetComponent<TreeGenerator>().treeParent;
			Debug.Log ("Saved: " + localPositions[j].x);
			for(int k = 0; k < trees.transform.childCount; k++){
				Debug.Log ("Child: " + trees.transform.GetChild(k).localPosition.x); 
				if(localPositions[j].x == leftGuide){
					if(trees.transform.GetChild(k).localPosition.x < 0)
						trees.transform.GetChild(k).gameObject.SetActive(false);
					else
						trees.transform.GetChild(k).gameObject.SetActive(true);
				}
				if(localPositions[j].x == centerGuide){
					if(trees.transform.GetChild(k).localPosition.x == 0)
						trees.transform.GetChild(k).gameObject.SetActive(false);
					else
						trees.transform.GetChild(k).gameObject.SetActive(true);
				}
				if(localPositions[j].x == rightGuide){
					if(trees.transform.GetChild(k).localPosition.x > 0)
						trees.transform.GetChild(k).gameObject.SetActive(false);
					else
						trees.transform.GetChild(k).gameObject.SetActive(true);
				}
			}
//			GameObject ti = (GameObject) Instantiate(Resources.Load(treePrefabPath), Vector3.zero, Quaternion.identity);
//			Vector3 loct = new Vector3(localPositions[j].x, 0f, 0f);
//			ti.transform.parent = pi.transform;
//			ti.transform.localPosition = loct;
		}
	}

	void CreateSavedFlagTrack(List<Vector3> localPositions){
		float localY = 0f;
		float lastY = localPositions [localPositions.Count - 1].y;
		PlayerSaveData.playerData.SetTotObstacles (localPositions.Count);
		int pieces = Mathf.CeilToInt(lastY / pieceOffset);
		for(int i = 0; i <= pieces; i++){
			localY =  pieceOffset * (i+1);
			Vector3 pos = new Vector3(gameObject.transform.position.x, localY, gameObject.transform.position.z);
			GameObject pi = (GameObject) Instantiate(Resources.Load(pieceOfTrack), pos, Quaternion.identity);
			pi.transform.parent = gameObject.transform;
			if(i < 10)
				pi.name = "Ski_plane0" + i;
			else
				pi.name= "Ski_plane" + i;
			pi.BroadcastMessage("EmptyPieceOfTrack");
		}
		localY +=  pieceOffset;
		Vector3 posf = new Vector3(gameObject.transform.position.x, localY, gameObject.transform.position.z);
		GameObject fi = (GameObject) Instantiate(Resources.Load(finishLine), posf, Quaternion.identity);
		fi.transform.parent = transform;
		for(int j = 0; j < localPositions.Count; j++){
			GameObject ti = (GameObject) Instantiate(Resources.Load(flagPrefabPath), Vector3.zero, Quaternion.identity);
			Vector3 loct = new Vector3(localPositions[j].x, localPositions[j].y, 0f);
			ti.transform.parent = gameObject.transform;
			ti.transform.localPosition = loct;
		}
	}
}
