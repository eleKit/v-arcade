using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LeftSlideGesture : MonoBehaviour {

	//Numero di angoli da considerare per rilevare il movimento
	static int numAngles = 30;
	static int relAngles = 5;
	//Di quanti gradi deve muoversi il polso negli ultimi numAngles frame
	//per accettare il movimento (1f => 10°)
	static float minLeftMovement = 0f;
	static float defaultMinLeftMovement = 5f;
	//Angolazione minima della mano per poter effettuare il salto
	static float minAngle = 0f;
	static float defaultMinAngle = 5f;
	//Poiché si vuole ricreare idealmente il funzionamento di un bottone che mandi un singolo
	//impulso quando attivato (e non uno continuato, come ad esempio quello della tastiera)
	//si controlla che, dopo aver riconosciuto un movimento, la mano sia tornata in una posizione
	// nell'intorno (-minOffsetToRelease, +minOffsetTorelease), prima di poter riconoscere nuovamente lo stesso movimento
	static float minOffsetToRelease = 5f;
	
	float leftMovement, relMovement;
	float minLeftHorizontal, maxLeftHorizontal;
	List<float> lAngles = new List<float>();
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
		float yLeft = GameObject.Find ("HandController").GetComponent<LeftYRotationStatsScript> ().GetYExtension ();
		
		leftMovement = UpdateLeftAngles (yLeft);

		if(yLeft > -minOffsetToRelease && yLeft < minOffsetToRelease){
			slideLeft = true;
			slideRight = true;
		}
		if(tuningsSet){
			if(yLeft > maxLeftHorizontal/4 && (leftMovement > maxLeftHorizontal/3 || relMovement > maxLeftHorizontal/3)){
				if(canSlideRight && slideRight){
					lAngles = new List<float>();
					canSlideLeft = false;
					slideRight = false;
					StartCoroutine("NowSlideLeft");
					SendMessage("SlideRight", false);
				}
			}
			
			if(yLeft < minLeftHorizontal/4 && (leftMovement < minLeftHorizontal/3 || relMovement < minLeftHorizontal/3)){
				if(canSlideLeft && slideLeft){
					lAngles = new List<float>();
					canSlideRight = false;
					slideLeft = false;
					StartCoroutine("NowSlideRight");
					SendMessage("SlideLeft", false);
				}
			}
		}
		else{
			if(yLeft > defaultMinAngle && (leftMovement > defaultMinLeftMovement || relMovement > defaultMinLeftMovement)){
				if(canSlideRight && slideRight){
					lAngles = new List<float>();
					canSlideLeft = false;
					slideRight = false;
					StartCoroutine("NowSlideLeft");
					SendMessage("SlideRight", false);
				}
			}
			
			if(yLeft < -defaultMinAngle && (leftMovement < -defaultMinLeftMovement || relMovement < -defaultMinLeftMovement)){
				if(canSlideLeft && slideLeft){
					lAngles = new List<float>();
					canSlideRight = false;
					slideLeft = false;
					StartCoroutine("NowSlideRight");
					SendMessage("SlideLeft", false);
				}
			}
		}
	}
	
	float UpdateLeftAngles(float angle){
		if(!GameObject.Find ("HandController").GetComponent<HandController>().leftHandVisible){
			lAngles = new List<float>();
			return 0;
		}
		lAngles.Add (angle);
		
		if(lAngles.Count < relAngles)
			relMovement = lAngles[lAngles.Count -1] - lAngles[0];
		else
			relMovement = lAngles[lAngles.Count -1] - lAngles[lAngles.Count - relAngles];
		
		if(lAngles.Count < numAngles)
			return lAngles[lAngles.Count -1] - lAngles[0];
		else
			return lAngles[lAngles.Count -1] - lAngles[lAngles.Count - numAngles];
	}

//	void GetTunings(string pl){
//		Hashtable tunings = PlayerSaveData.playerData.GetPlayerTunings();
//		minLeftHorizontal = (float)tunings ["Min Left Horizontal"];
//		maxLeftHorizontal = (float)tunings ["Max Left Horizontal"];
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
		minLeftHorizontal = minHorizontal;
		maxLeftHorizontal = maxHorizontal;
		tuningsSet = true;
	}
}
