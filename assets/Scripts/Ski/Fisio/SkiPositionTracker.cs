using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkiPositionTracker : MonoBehaviour {

	public List<Vector3> positions = new List<Vector3>();
	string flagPrefabPath = "Prefabs/Ski/Flags";
	string treePrefabPath = "Prefabs/Ski/Tree";
	float leftGuide, rightGuide, centerGuide;
	bool onCenter, onLeft,onRight;
	int obsCount = 0;
	
	//Intervallo di tempo (in secondi) tra il posizionamento di una coppia di bandiere e quella successiva.
	float targetInterval = 3f;

	public GameObject controller, track;
	
	// Use this for initialization
	void Start () {
		leftGuide = GetComponent<SkiFisioScript> ().leftGuideX;
		rightGuide = GetComponent<SkiFisioScript> ().rightGuideX;
		centerGuide = GetComponent<SkiFisioScript> ().centerGuideX;
	}
	
	// Update is called once per frame
	void Update () {
		targetInterval = SkiSaveData.skiData.GetObstacleInterval ();
		if (controller.GetComponent<FisioSkiController>().IsTesting())
			CancelInvoke ();
	}
	
	void SaveData(){
		positions.Add (transform.position);
		if(SkiSaveData.skiData.GetStepMode()){
			float tempX = transform.position.x;
			if(tempX > leftGuide/2 && tempX < rightGuide/2){
				tempX = centerGuide;
				onCenter = true;
				onLeft = false;
				onRight = false;
			}
			else if(tempX < leftGuide/2){
				tempX = leftGuide;
				onCenter = false;
				onLeft = true;
				onRight = false;
			}
			else if (tempX > rightGuide/2){
				tempX = rightGuide;
				onCenter = false;
				onLeft = false;
				onRight = true;
			}

			Vector3 temp = new Vector3(tempX, transform.position.y, transform.position.z);
			GameObject go = (GameObject)Instantiate(Resources.Load(treePrefabPath), temp, Quaternion.identity);
			go.transform.parent = track.transform;
			Debug.Log (go.transform.localPosition);
			SkiSaveData.skiData.AddObstacle (go.name, go.transform.localPosition);
			if(obsCount < 10)
				go.name = "Tree0" + obsCount;
			else
				go.name = "Tree" + obsCount;
			obsCount++;
			Destroy(go);
			if(onCenter){
				Vector3 temp1 = new Vector3(leftGuide, transform.position.y, transform.position.z);
				GameObject go1 = (GameObject)Instantiate(Resources.Load(treePrefabPath), temp1, Quaternion.identity);
				go1.transform.parent = track.transform;
				Vector3 temp2 = new Vector3(rightGuide, transform.position.y, transform.position.z);
				GameObject go2 = (GameObject)Instantiate(Resources.Load(treePrefabPath), temp2, Quaternion.identity);
				go2.transform.parent = track.transform;
			}
			if(onLeft){
				Vector3 temp1 = new Vector3(centerGuide, transform.position.y, transform.position.z);
				GameObject go1 = (GameObject)Instantiate(Resources.Load(treePrefabPath), temp1, Quaternion.identity);
				go1.transform.parent = track.transform;
				Vector3 temp2 = new Vector3(rightGuide, transform.position.y, transform.position.z);
				GameObject go2 = (GameObject)Instantiate(Resources.Load(treePrefabPath), temp2, Quaternion.identity);
				go2.transform.parent = track.transform;
			}
			if(onRight){
				Vector3 temp1 = new Vector3(leftGuide, transform.position.y, transform.position.z);
				GameObject go1 = (GameObject)Instantiate(Resources.Load(treePrefabPath), temp1, Quaternion.identity);
				go1.transform.parent = track.transform;
				Vector3 temp2 = new Vector3(centerGuide, transform.position.y, transform.position.z);
				GameObject go2 = (GameObject)Instantiate(Resources.Load(treePrefabPath), temp2, Quaternion.identity);
				go2.transform.parent = track.transform;
			}
		}
		else{
			GameObject go = (GameObject)Instantiate(Resources.Load(flagPrefabPath), transform.position, Quaternion.identity);
			go.transform.parent = track.transform;
			if(obsCount < 10)
				go.name = "Flag0" + obsCount;
			else
				go.name = "Flag" + obsCount;
			SkiSaveData.skiData.AddObstacle (go.name, go.transform.localPosition);
			obsCount++;
		}
	}
	
	public void StartTracking(){
		SkiSaveData.skiData.InitializePath ();
		SkiSaveData.skiData.SetMovementType (SkiSaveData.skiData.GetStepMode());
		InvokeRepeating ("SaveData", targetInterval, targetInterval);
		
	}
	
	public List<Vector3> getPositions(){
		return positions;
	}
	

}
