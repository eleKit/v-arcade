using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerPlaneScript : MonoBehaviour {



	Vector3 startAngles;
	float xAngle, yAngle, zAngle;
	float verticalMax = 70f;
	float horizontalMax = 50f;
//	float[] xAngles, yAngles, zAngles;

	//Player tunings
	float minLeftVertical, maxLeftVertical, minLeftHorizontal, maxLeftHorizontal;
	float minRightVertical, maxRightVertical, minRightHorizontal, maxRightHorizontal;
	//Bonus in base ai tunings
	float minXBonus, maxXBonus, minYBonus, maxYBonus;
	
	bool player, gotTunings, go, track;
	//Utilizzati per calcolare i bonus in caso di modalità con una sola mano
	bool leftMinVerticalWeak, rightMinVerticalWeak, leftMaxVerticalWeak, rightMaxVerticalWeak;
	bool leftMinHorizontalWeak, rightMinHorizontalWeak, leftMaxHorizontalWeak, rightMaxHorizontalWeak;

	float verticalTurnIncrement = 0.3f;
	float horizontalTurnIncrement = 0.5f;
	int count = 0;

	public static float speed = 30f;
	public GameObject saveManager, controller;
	static int numAngles = 15;

	Quaternion startRotation;

	// Assegna le mani del leap e inizializza la posizione di partenza e i bonus
	void Start () {
//		xAngles = new float[numAngles];
//		yAngles = new float[numAngles];
//		zAngles = new float[numAngles];
		startAngles = new Vector3(0f, 180f, 0f);
		startRotation = Quaternion.Euler (startAngles);
		minXBonus = 0f;
		maxXBonus = 0f;
		minYBonus = 0f;
		maxYBonus = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		// Recupera i dati relativi al tuning del giocatore
		if(player && !gotTunings && !SaveInfos.replay){
			GetTunings(PlayerSaveData.playerData.GetUserName());
			UpdateBonus();
			gotTunings = true;
		}



		if(Time.timeScale == 1f){
			// Recupera le rotazioni delle mani rispetto agli assi x,y,z
			float xRight = GetComponent<RightAnglesRetrieveScript> ().GetXExt ();
			float yRight = GetComponent<RightAnglesRetrieveScript> ().GetYExt ();
			//float zRight = GetComponent<RightAnglesRetrieveScript> ().GetZExt ();
			
			float xLeft = GetComponent<LeftAnglesRetrieveScript> ().GetXExt ();
			float yLeft = GetComponent<LeftAnglesRetrieveScript> ().GetYExt ();
			//float zLeft = GetComponent<LeftAnglesRetrieveScript> ().GetZExt ();


			// Aggiorna le estensioni utilizzando i bonus calcolati in base al tuning
			// Le estensioni sono moltiplicate per una costante, per accentuare la rotazione visibile nel gioco.
			if(!PlayerSaveData.playerData.GetOneHandMode()){
				// Fa la media delle estensioni delle due mani 
				// (ho lasciato a zero la rotazione rispetto all'asse z, per semplicità)
				xAngle = (xRight + xLeft) / 2;
				yAngle = (yRight + yLeft) / 2;
				zAngle = 0f;

				float minXDiff = Mathf.Abs (minLeftVertical - minRightVertical);
				float maxXDiff = Mathf.Abs (maxLeftVertical - maxRightVertical);
				float minYDiff = Mathf.Abs (minLeftHorizontal - minRightHorizontal);
				float maxYDiff = Mathf.Abs (maxLeftHorizontal - maxRightHorizontal);

				if(xAngle < 0){
					if(Mathf.Abs(xRight-xLeft) >= maxXDiff)
						xAngle = verticalTurnIncrement*(xAngle - maxXBonus);
					else
						xAngle = verticalTurnIncrement*xAngle;
				}
				else{
					if(Mathf.Abs(xRight-xLeft) >= minXDiff)
						xAngle = verticalTurnIncrement*(xAngle + minXBonus);
					else
						xAngle = verticalTurnIncrement*xAngle;
				}
				
				if(yAngle < 180){
					if(Mathf.Abs(yRight-yLeft) >= minYDiff)
						yAngle = horizontalTurnIncrement*(yAngle - minYBonus);
					else
						yAngle = horizontalTurnIncrement*yAngle;
				}
				else{
					if(Mathf.Abs(yRight-yLeft) >= maxYDiff)
						yAngle = horizontalTurnIncrement*(yAngle + maxYBonus);
					else
						yAngle = horizontalTurnIncrement*yAngle;
				}
			}

			if(PlayerSaveData.playerData.GetOneHandMode() && !PlayerSaveData.playerData.GetRightHand()){
				xAngle = xLeft;
				yAngle = yLeft;
				zAngle = 0f;
				if(xAngle < 0){
					if(leftMaxVerticalWeak)
						xAngle = verticalTurnIncrement*(xAngle - maxXBonus);
					else
						xAngle = verticalTurnIncrement*xAngle;
				}
				else{
					if(leftMinVerticalWeak)
						xAngle = verticalTurnIncrement*(xAngle + minXBonus);
					else
						xAngle = verticalTurnIncrement*xAngle;
				}
				
				if(yAngle < 180){
					if(leftMinHorizontalWeak)
						yAngle = horizontalTurnIncrement*(yAngle - minYBonus);
					else
						yAngle = horizontalTurnIncrement*yAngle;
				}
				else{
					if(leftMaxHorizontalWeak)
						yAngle = horizontalTurnIncrement*(yAngle + maxYBonus);
					else
						yAngle = horizontalTurnIncrement*yAngle;
				}
			}
			
			if(PlayerSaveData.playerData.GetOneHandMode() && PlayerSaveData.playerData.GetRightHand()){
				xAngle = xRight;
				yAngle = yRight;
				zAngle = 0f;
				if(xAngle < 0){
					if(rightMaxVerticalWeak)
						xAngle = verticalTurnIncrement*(xAngle - maxXBonus);
					else
						xAngle = verticalTurnIncrement*xAngle;
				}
				else{
					if(rightMinVerticalWeak)
						xAngle = verticalTurnIncrement*(xAngle + minXBonus);
					else
						xAngle = verticalTurnIncrement*xAngle;
				}
				
				if(yAngle < 180){
					if(rightMinHorizontalWeak)
						yAngle = horizontalTurnIncrement*(yAngle - minYBonus);
					else
						yAngle = horizontalTurnIncrement*yAngle;
				}
				else{
					if(rightMaxHorizontalWeak)
						yAngle = horizontalTurnIncrement*(yAngle + maxYBonus);
					else
						yAngle = horizontalTurnIncrement*yAngle;
				}
			}

//			if(xAngle < 0)
//				xAngle = (xAngle - minXBonus) * turnIncrement;
//			else
//				xAngle = (xAngle + maxXBonus) * turnIncrement;
//			
//			if(yAngle < 0)
//				yAngle = (yAngle - minYBonus) * turnIncrement;
//			else
//				yAngle = (yAngle + maxYBonus) * turnIncrement;

//			if(count < numAngles-1){
//				xAngles[count] = xAngle;
//				yAngles[count] = yAngle;
//				zAngles[count] = zAngle;
//			}
//			else{
//				ShiftArray(yAngles);
//				ShiftArray(xAngles);
//				xAngles[numAngles-1] = xAngle;
//				yAngles[numAngles-1] = yAngle;
//				zAngles[numAngles-1] = zAngle;
//			}

			//Versione non disaccoppiata
			if(go){
				Vector3 updatedAngles = new Vector3(startAngles.x - xAngle, startAngles.y + yAngle, startAngles.z + zAngle);
				transform.eulerAngles = updatedAngles;
			}


			if(count < numAngles-1)
				count++;
		}
	}


	void FixedUpdate(){
		if(go){
			rigidbody.velocity = transform.TransformDirection(Vector3.back) * speed;
		}
		else
			rigidbody.velocity = transform.TransformDirection(Vector3.zero);
	}


	public void SetTunings(){
		GetTunings(PlayerSaveData.playerData.GetUserName());
		UpdateBonus();
		gotTunings = true;
	}

	float PerfomKalmanTest(float[] DATA)
	{
		float[] temp = new float[numAngles];
		
		for (int i = 0; i < numAngles; i++)
		{
			//Debug.Log(Mathf.Round(SimpleKalman.update(DATA[i])) + ",");
			temp[i] = SimpleKalman.update(DATA[i]);
		}
		return temp [count];
	}

//	void ShiftArray(float[] arr){
//		float[] tempArray = new float[arr.Length];
//		for (int i = 1; i < arr.Length; i++)
//			tempArray[i - 1] = arr[i];
//		tempArray.CopyTo (arr, 0);
//	}

	public void ResetAngles(){
		transform.eulerAngles = startAngles;
	}

	public void SetStartAngles(Vector3 start){
		startAngles = start;
		PlayerSaveData.playerData.SetStartAngles (start);
	}

	public void SetStartAngles(){
		startAngles = new Vector3 (-xAngle, -yAngle, 0f);
		PlayerSaveData.playerData.SetStartAngles (startAngles);
	}

	public void SetPlayer(bool pl){
		player = pl;
	}

	// Definisce i bonus da aggiungere all'estensione delle mani.
	// Il bonus è pari a un quarto della differenza tra le due estensioni.
	void UpdateBonus(){
		if(PlayerSaveData.playerData.GetOneHandMode()){
			if(minLeftHorizontal > minRightHorizontal){
				leftMinHorizontalWeak = true;
				rightMinHorizontalWeak = false;
			}
			else{
				leftMinHorizontalWeak = false;
				rightMinHorizontalWeak = true;
			}
			if(maxRightHorizontal < maxLeftHorizontal){
				leftMaxHorizontalWeak = false;
				rightMaxHorizontalWeak = true;
			}
			else{
				leftMaxHorizontalWeak = true;
				rightMaxHorizontalWeak = false;
			}
			if(minLeftVertical < minRightVertical){
				leftMinVerticalWeak = true;
				rightMinVerticalWeak = false;
			}
			else{
				leftMinVerticalWeak = false;
				rightMinVerticalWeak = true;
			}
			if(maxLeftVertical > maxRightVertical){
				leftMaxVerticalWeak = true;
				rightMaxVerticalWeak = false;
			}
			else{
				leftMaxVerticalWeak = false;
				rightMaxVerticalWeak = true;
			}
		}

		float minXDiff = Mathf.Abs (minLeftVertical - minRightVertical);
		float maxXDiff = Mathf.Abs (maxLeftVertical - maxRightVertical);
		float minYDiff = Mathf.Abs (minLeftHorizontal - minRightHorizontal);
		float maxYDiff = Mathf.Abs (maxLeftHorizontal - maxRightHorizontal);

		minXBonus = minXDiff / 4;
		minYBonus = minYDiff / 4;
		maxXBonus = maxXDiff / 4;
		maxYBonus = maxYDiff / 4;
		PlayerSaveData.playerData.SetBonus (minXBonus, maxXBonus, minYBonus, maxYBonus);
	}

	// Recupera le informazioni salvate durante la fase di tuning
	void GetTunings(string pl){
		Hashtable tunings = PlayerSaveData.playerData.GetPlayerTunings();
		minLeftVertical = (float)tunings ["Min Left Vertical"];
		maxLeftVertical = (float)tunings ["Max Left Vertical"];
		minLeftHorizontal = (float)tunings ["Min Left Horizontal"];
		maxLeftHorizontal = (float)tunings ["Max Left Horizontal"];
		minRightVertical = (float)tunings ["Min Right Vertical"];
		maxRightVertical = (float)tunings ["Max Right Vertical"];
		minRightHorizontal = (float)tunings ["Min Right Horizontal"];
		maxRightHorizontal = (float)tunings ["Max Right Horizontal"];
	}


	public void Go(){
		go = true;

	}

	public void Stop(){
		go = false;
	}
}
