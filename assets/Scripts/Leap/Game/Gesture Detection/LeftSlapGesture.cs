using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LeftSlapGesture : MonoBehaviour {

	//Numero di angoli da considerare per rilevare il movimento
	static int numAngles = 30;
	static int relAngles = 5;
	//Di quanti gradi deve muoversi il polso negli ultimi numAngles frame
	//per accettare il movimento (1f => 10°)
	static float minLeftMovement = 0f;
	static float defaultMinLeftMovement = 7f;
	//Angolazione minima della mano per poter effettuare il salto
	static float minAngle = 0f;
	static float defaultMinAngle = 10f;
	//Poiché si vuole ricreare idealmente il funzionamento di un bottone che mandi un singolo
	//impulso quando attivato (e non uno continuato, come ad esempio quello della tastiera)
	//si controlla che, dopo aver riconosciuto un movimento, la mano sia tornata in una posizione
	// nell'intorno (-minOffsetToRelease, +minOffsetTorelease), prima di poter riconoscere nuovamente lo stesso movimento
	static float minOffsetToRelease = 5f;

	static float zRef = 90f;
	static float offset = 45f;
	
	float leftMovement, relMovement;
	float minLeftVertical, maxLeftVertical;
	List<float> lAngles = new List<float>();
	float curZ;
	bool tuningsSet;
	bool canSlapLeft = true;
	bool canSlapRight = true;
	bool slapLeft = true;
	bool slapRight = true;
	
	// Use this for initialization
	void Start () {
//		GetTunings(PlayerSaveData.playerData.GetUserName());
	}
	
	// Update is called once per frame
	void Update () {
		float yLeft = GameObject.Find ("HandController").GetComponent<LeftYRotationStatsScript> ().GetYExtension ();
		curZ = GameObject.Find ("HandController").GetComponent<LeftRotationScript> ().GetLeftRotation ().z;

		leftMovement = UpdateLeftAngles (yLeft);

		if(yLeft > -minOffsetToRelease && yLeft < minOffsetToRelease){
			slapLeft = true;
			slapRight = true;
		}

		if(curZ > (zRef - offset) && curZ < (zRef + offset)){
			if(tuningsSet){
				if(yLeft > -maxLeftVertical/4 && (leftMovement > -maxLeftVertical/3 || relMovement > -maxLeftVertical/3)){
					if(canSlapLeft && slapLeft){
						lAngles = new List<float>();
						canSlapRight = false;
						slapLeft = false;
						StartCoroutine("NowSlapRight");
						SendMessage("SlapLeft", false);
					}
				}
				
				if(yLeft < -minLeftVertical/4 && (leftMovement < -minLeftVertical/3 || relMovement < -minLeftVertical/3)){
					if(canSlapRight && slapRight){
						lAngles = new List<float>();
						canSlapLeft = false;
						slapRight = false;
						StartCoroutine("NowSlapLeft");
						SendMessage("SlapRight", false);
					}
				}
			}
			else{
				if(yLeft > defaultMinAngle && (leftMovement > defaultMinLeftMovement || relMovement > defaultMinLeftMovement)){
					if(canSlapLeft && slapLeft){
						lAngles = new List<float>();
						canSlapRight = false;
						slapLeft = false;
						StartCoroutine("NowSlapRight");
						SendMessage("SlapLeft", false);
					}
				}
				
				if(yLeft < -defaultMinAngle && (leftMovement < -defaultMinLeftMovement || relMovement < -defaultMinLeftMovement)){
					if(canSlapRight && slapRight){
						lAngles = new List<float>();
						canSlapLeft = false;
						slapRight = false;
						StartCoroutine("NowSlapLeft");
						SendMessage("SlapRight", false);
					}
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
//		minLeftVertical = (float)tunings ["Min Left Vertical"];
//		maxLeftVertical = (float)tunings ["Max Left Vertical"];
//	}

	IEnumerator NowSlapLeft(){
		yield return new WaitForSeconds(0.75f);
		canSlapLeft = true;
	}
	
	IEnumerator NowSlapRight(){
		yield return new WaitForSeconds(0.75f);
		canSlapRight = true;
	}

	public void SetTunings(float minVertical, float maxVertical){
		minLeftVertical = minVertical;
		maxLeftVertical = maxVertical;
		tuningsSet = true;
	}
}
