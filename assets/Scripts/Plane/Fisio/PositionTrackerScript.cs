using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PositionTrackerScript : MonoBehaviour {

	public ArrayList positions, angles;
	string targetPrefabPath = "Prefabs/Plane/concretePipe";
	int pipeCount = 0;

	//Intervallo di tempo (in secondi) tra il posizionamento di un anello e quello successivo.
	float targetInterval = 3f;

	// Use this for initialization
	void Start () {
		positions = new ArrayList();
		angles = new ArrayList();
	}
	
	// Update is called once per frame
	void Update () {
		targetInterval = PathSaveData.pathData.GetObstacleInterval ();
		if (GameObject.FindGameObjectWithTag("GameController").GetComponent<FisioStarshipGameController>().IsTesting())
			CancelInvoke ();
	}

	void SaveData(){
		positions.Add (transform.position);
		angles.Add (transform.eulerAngles);
		GameObject go = (GameObject)Instantiate(Resources.Load(targetPrefabPath), transform.position, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z)) ;
		if(pipeCount < 10)
			go.name = "Target0" + pipeCount;
		else
			go.name = "Target" + pipeCount;
		PathSaveData.pathData.AddObstacle (go.name, go.transform.position, go.transform.eulerAngles);
		pipeCount++;
	}

	public void StartTracking(){
		PathSaveData.pathData.InitializePath ();
		InvokeRepeating ("SaveData", targetInterval, targetInterval);

	}

	public ArrayList getPositions(){
		return positions;
	}

	public ArrayList getAngles(){
		return angles;
	}
}
