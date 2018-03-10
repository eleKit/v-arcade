using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkiEditorController : MonoBehaviour {

	int mode, obstacles, diff, mulFactor;

	Dictionary<int, SectorInfos> sectors = new Dictionary<int, SectorInfos>();

	SectorInfos sectorOne, sectorTwo, sectorThree, sectorFour, sectorFive;

	int minObs = 10;
	int sinX = 0;
	float hardLocalPos = 3.125f;
	float mediumLocalPos = 2f;
	float easyLocalPos = 1.5f;
	float sideEasyLocalPos = 0.5f;
	float sideMediumLocalPos = 1f;
	float sideHardLocalPos = 1.5f;
	float flagLocalPosRange;

	List<float> poss;

	// Use this for initialization
	void Start () {
		poss = new List<float> ();
		sectorOne = new SectorInfos ();
		sectorTwo = new SectorInfos ();
		sectorThree = new SectorInfos ();
		sectorFour = new SectorInfos ();
		sectorFive = new SectorInfos ();
		sectors.Add (1, sectorOne);
		sectors.Add (2, sectorTwo);
		sectors.Add (3, sectorThree);
		sectors.Add (4, sectorFour);
		sectors.Add (5, sectorFive);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetMode(int m, int sector){
		switch(m){
		case 0:
			sectors[sector].empty = true;
			sectors[sector].center = false;
			sectors[sector].left = false;
			sectors[sector].right = false;
			break;
		case 1:
			sectors[sector].empty = false;
			sectors[sector].center = true;
			sectors[sector].left = false;
			sectors[sector].right = false;
			break;
		case 2:
			sectors[sector].empty = false;
			sectors[sector].center = false;
			sectors[sector].left = true;
			sectors[sector].right = false;
			break;
		case 3:
			sectors[sector].empty = false;
			sectors[sector].center = false;
			sectors[sector].left = false;
			sectors[sector].right = true;
			break;
		}
	}

	public void DecreaseObstacles(int obs, int sector){
		sectors [sector].obstacles = obs;
		obstacles -= obs;
		if (obstacles < sectors.Count * minObs)
			obstacles = sectors.Count * minObs;
	}

	public void IncreaseObstacles(int obs, int sector){
		sectors [sector].obstacles = obs;
		obstacles += obs;
	}

	public void SetDifficulty(int dif, int sector){
		sectors[sector].difficulty = dif;
	}

	public void CreatePath(){
		mulFactor = (int)Random.Range (5f, 8f); 
		int sectorCount = sectors.Count;
		for(int i = 1; i < sectorCount + 1; i++){
			sinX = 0;
			if(sectors[i].center){
				switch(sectors[i].difficulty){
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
				for(int j = 0; j < sectors[i].obstacles +1; j++){
					float tempPos = Mathf.Sin(sinX*mulFactor) * flagLocalPosRange;
					poss.Add(tempPos);
					sinX++;
				}
			}
			else if(sectors[i].left){
				switch(sectors[i].difficulty){
				case 1:
					flagLocalPosRange = sideEasyLocalPos;
					break;
				case 2:
					flagLocalPosRange = sideMediumLocalPos;
					break;
				case 3:
					flagLocalPosRange = sideHardLocalPos;
					break;
				}
				for(int j = 0; j < sectors[i].obstacles +1; j++){
					float tempPos = (Mathf.Sin(sinX*mulFactor) * flagLocalPosRange) - (hardLocalPos/2);
					poss.Add(tempPos);
					sinX++;
				}
			}
			else if(sectors[i].right){
				switch(sectors[i].difficulty){
				case 1:
					flagLocalPosRange = sideEasyLocalPos;
					break;
				case 2:
					flagLocalPosRange = sideMediumLocalPos;
					break;
				case 3:
					flagLocalPosRange = sideHardLocalPos;
					break;
				}
				for(int j = 0; j < sectors[i].obstacles +1; j++){
					float tempPos = (Mathf.Sin(sinX*mulFactor) * flagLocalPosRange) + (hardLocalPos/2);
					poss.Add(tempPos);
					sinX++;
				}
			}
			else
				return;
		}
	}

	public void PathSetup(){
		CreatePath ();
		SkiSaveData.skiData.SetRandomPath ();
		SkiSaveData.skiData.SetXPoss (poss);
	}
}

public class SectorInfos{
	public bool center, left, right;
	public bool empty = true;
	public int difficulty  = 1;
	public int obstacles = 10;
}
