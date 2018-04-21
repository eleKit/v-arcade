using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LeftJumpGesture : MonoBehaviour
{

	//Numero di angoli da considerare per rilevare il movimento
	static int numAngles = 30;
	static int relAngles = 5;
	//Di quanti gradi deve muoversi il polso negli ultimi numAngles frame
	//per accettare il movimento (1f => 1°)
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

	float leftMovement, relMovement;
	float minLeftVertical, maxLeftVertical;
	List<float> lAngles = new List<float>();
	bool tuningsSet;
	bool canJump = true;
	bool canGoDown = true;
	bool goDown = true;
	bool jumpUp = true;
	
	// Use this for initialization
	void Start()
	{
//		GetTunings(PlayerSaveData.playerData.GetUserName());
	}
	
	// Update is called once per frame
	void Update()
	{
		float xLeft = GameObject.Find("MyHandController").GetComponent<LeftRotationScript>().GetXExtension();
		leftMovement = UpdateLeftAngles(xLeft);

		if (xLeft > -minOffsetToRelease && xLeft < minOffsetToRelease)
		{
			goDown = true;
			jumpUp = true;
		}

		if (tuningsSet)
		{
			if (xLeft > minLeftVertical / 2 && (leftMovement > minLeftVertical / 2 || relMovement > minLeftVertical / 2))
			{
				if (canGoDown && goDown)
				{
					lAngles = new List<float>();
					Debug.Log("Down " + Time.time + " angle: " + xLeft + " lMov: " + leftMovement + " lRMov: " + relMovement);
					canJump = false;
					StartCoroutine("NowJump");
					goDown = false;
					SendMessage("GoDown", false);
				}
			}

			if (xLeft < maxLeftVertical / 2 && (leftMovement < maxLeftVertical / 2 || relMovement < maxLeftVertical / 2))
			{
				if (canJump && jumpUp)
				{
					Debug.Log("Up " + Time.time + " angle: " + xLeft + " lMov: " + leftMovement + " lRMov: " + relMovement);
					canGoDown = false;
					lAngles = new List<float>();
					StartCoroutine("NowDown");
					jumpUp = false;
					SendMessage("JumpUp", false);
				}
			}
		}
		else
		{
			if (xLeft > defaultMinAngle && (leftMovement > defaultMinLeftMovement || relMovement > defaultMinLeftMovement))
			{
				if (canGoDown && goDown)
				{
					lAngles = new List<float>();
					Debug.Log("Down " + Time.time + " angle: " + xLeft + " lMov: " + leftMovement + " lRMov: " + relMovement);
					canJump = false;
					StartCoroutine("NowJump");
					goDown = false;
					SendMessage("GoDown", false);
				}
			}
			
			if (xLeft < -defaultMinAngle && (leftMovement < -defaultMinLeftMovement || relMovement < -defaultMinLeftMovement))
			{
				if (canJump && jumpUp)
				{
					Debug.Log("Up " + Time.time + " angle: " + xLeft + " lMov: " + leftMovement + " lRMov: " + relMovement);
					canGoDown = false;
					lAngles = new List<float>();
					StartCoroutine("NowDown");
					jumpUp = false;
					SendMessage("JumpUp", false);
				}
			}
		}
	}

	float UpdateLeftAngles(float angle)
	{
		if (!GameObject.Find("MyHandController").GetComponent<MyHandController>().leftHandVisible)
		{
			lAngles = new List<float>();
			return 0;
		}

		lAngles.Add(angle);

		if (lAngles.Count < relAngles)
			relMovement = lAngles[lAngles.Count - 1] - lAngles[0];
		else
			relMovement = lAngles[lAngles.Count - 1] - lAngles[lAngles.Count - relAngles];

		if (lAngles.Count < numAngles)
			return lAngles[lAngles.Count - 1] - lAngles[0];
		else
			return lAngles[lAngles.Count - 1] - lAngles[lAngles.Count - numAngles];
	}


	//	void GetTunings(string pl){
	//		Hashtable tunings = PlayerSaveData.playerData.GetPlayerTunings();
	//		minLeftVertical = (float)tunings ["Min Left Vertical"];
	//		maxLeftVertical = (float)tunings ["Max Left Vertical"];
	//	}

	IEnumerator NowJump()
	{
		yield return new WaitForSeconds(0.75f);
		canJump = true;
	}

	IEnumerator NowDown()
	{
		yield return new WaitForSeconds(0.75f);
		canGoDown = true;
	}

	public void SetTunings(float minVertical, float maxVertical)
	{
		minLeftVertical = minVertical;
		maxLeftVertical = maxVertical;
		tuningsSet = true;
	}
}
