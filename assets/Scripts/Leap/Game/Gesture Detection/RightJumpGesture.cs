using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RightJumpGesture : MonoBehaviour {

	//Numero di angoli da considerare per rilevare il movimento
	static int numAngles = 30;
	static int rerAngles = 5;
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
	float minRightVertical, maxRightVertical;
	List<float> rAngles = new List<float>();
	bool tuningsSet;
	bool canJump = true;
	bool canGoDown = true;
	bool goDown = true;
	bool jumpUp = true;
	
	// Use this for initialization
	void Start () {
//		GetTunings(PlayerSaveData.playerData.GetUserName());
	}
	
	// Update is called once per frame
	void Update () {
		float xRight = GameObject.Find ("HandController").GetComponent<RightRotationScript> ().GetXExtension ();

		rightMovement = UpdateRightAngles (xRight);

		if(xRight > -minOffsetToRelease && xRight < minOffsetToRelease){
			goDown = true;
			jumpUp = true;
		}

		if(tuningsSet){
			if(xRight > minRightVertical/2 && (rightMovement > minRightVertical/2 || relMovement > minRightVertical/2)){
				if(canGoDown && goDown){
					rAngles = new List<float>();
					Debug.Log ("Down " + Time.time + " rAngle: " + xRight + " rMov: " + rightMovement + " rRMov: " + relMovement);
					canJump = false;
					goDown = false;
					StartCoroutine("NowJump");
					SendMessage("GoDown", true);
				}
			}

			if(xRight < maxRightVertical/2 && (rightMovement < maxRightVertical/2 || relMovement < maxRightVertical/2)){
				if(canJump && jumpUp){
					Debug.Log ("Up " + Time.time + " angle: " + xRight + " rMov: " + rightMovement + " rRMov: " + relMovement);
					canGoDown = false;
					rAngles = new List<float>();
					jumpUp = false;
					StartCoroutine("NowDown");
					SendMessage("JumpUp", true);
				}
			}
		}
		else{
			if(xRight > defaultMinAngle && (rightMovement > defaultMinRightMovement || relMovement > defaultMinRightMovement)){
				if(canGoDown && goDown){
					rAngles = new List<float>();
					Debug.Log ("Down " + Time.time + " angle: " + xRight + " rMov: " + rightMovement + " rRMov: " + relMovement);
					canJump = false;
					goDown = false;
					StartCoroutine("NowJump");
					SendMessage("GoDown", true);
				}
			}
			
			if(xRight < -defaultMinAngle && (rightMovement < -defaultMinRightMovement || relMovement < -defaultMinRightMovement)){
				if(canJump && jumpUp){
					Debug.Log ("Up " + Time.time + " angle: " + xRight + " rMov: " + rightMovement + " rRMov: " + relMovement);
					canGoDown = false;
					rAngles = new List<float>();
					jumpUp = false;
					StartCoroutine("NowDown");
					SendMessage("JumpUp", true);
				}
			}
		}
	}
	
	float UpdateRightAngles(float angle){
		if(!GameObject.Find ("HandController").GetComponent<HandController>().rightHandVisible){
			rAngles = new List<float>();
			return 0;
		}
		rAngles.Add (angle);
		
		if(rAngles.Count < rerAngles)
			relMovement = rAngles[rAngles.Count -1] - rAngles[0];
		else
			relMovement = rAngles[rAngles.Count -1] - rAngles[rAngles.Count - rerAngles];
		
		if(rAngles.Count < numAngles)
			return rAngles[rAngles.Count -1] - rAngles[0];
		else
			return rAngles[rAngles.Count -1] - rAngles[rAngles.Count - numAngles];
	}


//	void GetTunings(string pl){
//		Hashtable tunings = PlayerSaveData.playerData.GetPlayerTunings();
//		minRightVertical = (float)tunings ["Min Right Vertical"];
//		maxRightVertical = (float)tunings ["Max Right Vertical"];
//	}

	IEnumerator NowJump(){
		yield return new WaitForSeconds(0.75f);
		canJump = true;
	}
	
	IEnumerator NowDown(){
		yield return new WaitForSeconds(0.75f);
		canGoDown = true;
	}

	public void SetTunings(float minVertical, float maxVertical){
		minRightVertical = minVertical;
		maxRightVertical = maxVertical;
		tuningsSet = true;
	}
}
