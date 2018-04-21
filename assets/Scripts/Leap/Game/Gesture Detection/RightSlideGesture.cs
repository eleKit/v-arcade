using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RightSlideGesture : MonoBehaviour {

	//Numero di angoli da considerare per rilevare il movimento
	static int numAngles = 30;
	static int relAngles = 5;
	//Di quanti gradi deve muoversi il polso negli ultimi numAngles frame
	//per accettare il movimento (1f => 10°)
	static float minRightMovement = 0f;
	static float defaultMinRightMovement = 7f;
	//Angolazione minima della mano per poter effettuare il salto
	static float minAngle = 0f;
	static float defaultMinAngle = 10f;
	//Poiché si vuole ricreare idealmente il funzionamento di un bottone che mandi un singolo
	//impulso quando attivato (e non uno continuato, come ad esempio quello della tastiera)
	//si controlla che, dopo aver riconosciuto un movimento, la mano sia tornata in una posizione
	// nell'intorno (-minOffsetToRelease, +minOffsetTorelease), prima di poter riconoscere nuovamente lo stesso movimento
	static float minOffsetToRelease = 5f;
	
	float rightMovement, relMovement;
	float minRightHorizontal, maxRightHorizontal;
	List<float> rAngles = new List<float>();
	bool tuningsSet;
	bool canSlideLeft = true;
	bool canSlideRight = true;
	bool slideLeft = true;
	bool slideRight = true;
	
	// Use this for initialization
	void Start () {
//		GetTunings(PlayerSaveData.playerData.GetUserName());
	}
	
	// Update is called once per frame
	void Update () {
		float yRight = GameObject.Find("MyHandController").GetComponent<RightYRotationStatsScript> ().GetYExtension ();
		
		rightMovement = UpdateRightAngles (yRight);

		if(yRight > -minOffsetToRelease && yRight < minOffsetToRelease){
			slideLeft = true;
			slideRight = true;
		}

		if(tuningsSet){
			if(yRight > maxRightHorizontal/4 && (rightMovement > maxRightHorizontal/3 || relMovement > maxRightHorizontal/3)){
				if(canSlideRight && slideRight){
					rAngles = new List<float>();
					canSlideLeft = false;
					slideRight = false;
					StartCoroutine("NowSlideLeft");
					SendMessage("SlideRight", true);
				}
			}
			
			if(yRight < minRightHorizontal/4 && (rightMovement < minRightHorizontal/3 || relMovement < minRightHorizontal/3)){
				if(canSlideLeft && slideLeft){
					rAngles = new List<float>();
					canSlideRight = false;
					slideLeft = false;
					StartCoroutine("NowSlideRight");
					SendMessage("SlideLeft", true);
				}
			}
		}
		else{
			if(yRight > defaultMinAngle && (rightMovement > defaultMinRightMovement || relMovement > defaultMinRightMovement)){
				if(canSlideRight && slideRight){
					rAngles = new List<float>();
					canSlideLeft = false;
					slideRight = false;
					StartCoroutine("NowSlideLeft");
					SendMessage("SlideRight", true);
				}
			}
			
			if(yRight < -defaultMinAngle && (rightMovement < -defaultMinRightMovement || relMovement < -defaultMinRightMovement)){
				if(canSlideLeft && slideLeft){
					rAngles = new List<float>();
					canSlideRight = false;
					slideLeft = false;
					StartCoroutine("NowSlideRight");
					SendMessage("SlideLeft", true);
				}
			}
		}
	}
	
	float UpdateRightAngles(float angle){
		if(!GameObject.Find("MyHandController").GetComponent<MyHandController>().rightHandVisible){
			rAngles = new List<float>();
			return 0;
		}
		rAngles.Add (angle);
		
		if(rAngles.Count < relAngles)
			relMovement = rAngles[rAngles.Count -1] - rAngles[0];
		else
			relMovement = rAngles[rAngles.Count -1] - rAngles[rAngles.Count - relAngles];
		
		if(rAngles.Count < numAngles)
			return rAngles[rAngles.Count -1] - rAngles[0];
		else
			return rAngles[rAngles.Count -1] - rAngles[rAngles.Count - numAngles];
	}
	
	void ShiftAngles(float[] angles){
		float[] tempArray = new float[angles.Length];
		for (int i = 1; i < angles.Length; i++)
			tempArray[i - 1] = angles[i];
		tempArray.CopyTo (angles, 0);
	}

//	void GetTunings(string pl){
//		Hashtable tunings = PlayerSaveData.playerData.GetPlayerTunings();
//		minRightHorizontal = (float)tunings ["Min Right Horizontal"];
//		maxRightHorizontal = (float)tunings ["Max Right Horizontal"];
//	}

	IEnumerator NowSlideLeft(){
		yield return new WaitForSeconds(0.75f);
		canSlideLeft = true;
	}
	
	IEnumerator NowSlideRight(){
		yield return new WaitForSeconds(0.75f);
		canSlideRight = true;
	}

	public void SetTunings(float minHorizontal, float maxHorizontal){
		minRightHorizontal = minHorizontal;
		maxRightHorizontal = maxHorizontal;
		tuningsSet = true;
	}
}
