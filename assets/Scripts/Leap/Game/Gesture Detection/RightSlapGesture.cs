using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RightSlapGesture : MonoBehaviour
{
	
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

	static float zRef = 270f;
	static float offset = 45f;

	float rightMovement, relMovement;
	float minRightVertical, maxRightVertical;
	List<float> rAngles = new List<float>();
	float curZ;
	bool tuningsSet;
	bool canSlapLeft = true;
	bool canSlapRight = true;
	bool slapLeft = true;
	bool slapRight = true;
	
	// Use this for initialization
	void Start()
	{
//		GetTunings(PlayerSaveData.playerData.GetUserName());
	}
	
	// Update is called once per frame
	void Update()
	{
		float yRight = GameObject.Find("MyHandController").GetComponent<RightRotationScript>().GetYExtension();
		curZ = GameObject.Find("MyHandController").GetComponent<RightRotationScript>().GetRightRotation().z;

		rightMovement = UpdateRightAngles(yRight);

		if (yRight > -minOffsetToRelease && yRight < minOffsetToRelease)
		{
			slapLeft = true;
			slapRight = true;
		}

		if (curZ > (zRef - offset) && curZ < (zRef + offset))
		{
			if (tuningsSet)
			{
				if (yRight > -maxRightVertical / 4 && (rightMovement > -maxRightVertical / 3 || relMovement > -maxRightVertical / 3))
				{
					if (canSlapRight && slapRight)
					{
						rAngles = new List<float>();
						canSlapLeft = false;
						slapRight = false;
						StartCoroutine("NowSlapLeft");
						SendMessage("SlapRight", true);
					}
				}
				
				if (yRight < -minRightVertical / 4 && (rightMovement < -minRightVertical / 3 || relMovement < -minRightVertical / 3))
				{
					if (canSlapLeft && slapLeft)
					{
						rAngles = new List<float>();
						canSlapRight = false;
						slapLeft = false;
						StartCoroutine("NowSlapRight");
						SendMessage("SlapLeft", true);
					}
				}
			}
			else
			{
				if (yRight > defaultMinAngle && (rightMovement > defaultMinRightMovement || relMovement > defaultMinRightMovement))
				{
					if (canSlapRight && slapRight)
					{
						rAngles = new List<float>();
						canSlapLeft = false;
						slapRight = false;
						StartCoroutine("NowSlapLeft");
						SendMessage("SlapRight", true);
					}
				}
				
				if (yRight < -defaultMinAngle && (rightMovement < -defaultMinRightMovement || relMovement < -defaultMinRightMovement))
				{
					if (canSlapLeft && slapLeft)
					{
						rAngles = new List<float>();
						canSlapRight = false;
						slapLeft = false;
						StartCoroutine("NowSlapRight");
						SendMessage("SlapLeft", true);
					}
				}
			}
		}
	}

	float UpdateRightAngles(float angle)
	{
		if (!GameObject.Find("MyHandController").GetComponent<MyHandController>().rightHandVisible)
		{
			rAngles = new List<float>();
			return 0;
		}
		rAngles.Add(angle);
		
		if (rAngles.Count < relAngles)
			relMovement = rAngles[rAngles.Count - 1] - rAngles[0];
		else
			relMovement = rAngles[rAngles.Count - 1] - rAngles[rAngles.Count - relAngles];
		
		if (rAngles.Count < numAngles)
			return rAngles[rAngles.Count - 1] - rAngles[0];
		else
			return rAngles[rAngles.Count - 1] - rAngles[rAngles.Count - numAngles];
	}

	//	void GetTunings(string pl){
	//		Hashtable tunings = PlayerSaveData.playerData.GetPlayerTunings();
	//		minRightVertical = (float)tunings ["Min Right Vertical"];
	//		maxRightVertical = (float)tunings ["Max Right Vertical"];
	//	}
	
	IEnumerator NowSlapLeft()
	{
		yield return new WaitForSeconds(0.75f);
		canSlapLeft = true;
	}

	IEnumerator NowSlapRight()
	{
		yield return new WaitForSeconds(0.75f);
		canSlapRight = true;
	}

	public void SetTunings(float minVertical, float maxVertical)
	{
		minRightVertical = minVertical;
		maxRightVertical = maxVertical;
		tuningsSet = true;
	}
}
