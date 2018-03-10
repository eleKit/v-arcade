using UnityEngine;
using System.Collections;
using System;
using Leap;

public class HandPositionTrackerScript : MonoBehaviour {

	static string leftGreenTexture = "Textures/Plane/Left_hand_green_01";
	static string rightGreenTexture = "Textures/Plane/Right_hand_green_01";
	static string leftRedTexture = "Textures/Plane/Left_hand_red_01";
	static string rightRedTexture = "Textures/Plane/Right_hand_red_01";

	GameObject rightHand, leftHand, handAlert, fingerAlert;
	//UnityHand leapLeftHand, leapRightHand;
	float handDistance, tempDistance, startDistance, maxDistance, minDistance, placeHolderStartPos;
	float percentage = 0.35f;
	bool gotHands;
	float leftPlStartX, rightPlStartX;
	float leftPlStartY, rightPlStartY;
	HandList hands;


	public GameObject leftPlaceholder, rightPlaceholder, leftOverlay, rightOverlay, handController, leapPlaceholder;

	// Use this for initialization
	void Start () {
		leftPlStartX = leftPlaceholder.transform.localPosition.x;
		rightPlStartX = rightPlaceholder.transform.localPosition.x;
		leftPlStartY = leftPlaceholder.transform.localPosition.y;
		rightPlStartY = rightPlaceholder.transform.localPosition.y;
		placeHolderStartPos = Mathf.Abs(leftPlaceholder.transform.position.x - rightPlaceholder.transform.position.x);
		handAlert = GameObject.Find ("Hand Alert");
		fingerAlert = GameObject.Find ("Finger Alert");
		startDistance = PlayerSaveData.playerData.GetHandDistance();
		minDistance = startDistance - (startDistance * percentage);
		maxDistance = startDistance + (startDistance * percentage);
	}
	
	// Update is called once per frame
	void Update () {
		hands = handController.GetComponent<HandController> ().hands;
		if(hands != null){
			if(!PlayerSaveData.playerData.GetOneHandMode()){
				//Se entrambe le mani sono sul leap, controlla la loro distanza e mostra
				//un alert nel caso le mani siano troppo vicine o troppo lontane
				if(hands.Count == 2){
					if(handDistance == 0){
						handDistance = Mathf.Abs(hands[0].PalmPosition.x - hands[1].PalmPosition.x);
						tempDistance = handDistance;
					}
					else{
						tempDistance = handDistance;
						handDistance = Mathf.Abs(hands[0].PalmPosition.x - hands[1].PalmPosition.x);
					}
					if(tempDistance != handDistance)
						UpdatePlaceholderPositions(startDistance, handDistance);
					if(handDistance < minDistance){
						handAlert.GetComponent<TextMesh>().text = "Mani troppo vicine!";
						leftOverlay.renderer.material.mainTexture = (Texture) Resources.Load (leftRedTexture);
						rightOverlay.renderer.material.mainTexture = (Texture) Resources.Load (rightRedTexture);
					}
					else if( handDistance > maxDistance){
						handAlert.GetComponent<TextMesh>().text = "Mani troppo lontane!";
						leftOverlay.renderer.material.mainTexture = (Texture) Resources.Load (leftRedTexture);
						rightOverlay.renderer.material.mainTexture = (Texture) Resources.Load (rightRedTexture);
					}
					else{
						handAlert.GetComponent<TextMesh>().text = "";
						leftOverlay.renderer.material.mainTexture = (Texture) Resources.Load (leftGreenTexture);
						rightOverlay.renderer.material.mainTexture = (Texture) Resources.Load (rightGreenTexture);
					}

					// Nella modalità a pugno chiuso, conta le dita visibili e mostra un alert
					// se il numero è maggiore di zero
					if(!PlayerSaveData.playerData.GetOpenHandMode()){
						int leftFingers = -1;
						int rightFingers = -1;
						
						try{
							//leftFingers = leapLeftHand.hand.Fingers.Count;
							//rightFingers = leapRightHand.hand.Fingers.Count;
							if(hands[0].IsLeft){
								FingerList leftExtendedFingers = hands[0].Fingers.Extended();
								FingerList rightExtendedFingers = hands[1].Fingers.Extended();
								leftFingers = leftExtendedFingers.Count;
								rightFingers = rightExtendedFingers.Count;
							}
							else{
								FingerList leftExtendedFingers = hands[1].Fingers.Extended();
								FingerList rightExtendedFingers = hands[0].Fingers.Extended();
								leftFingers = leftExtendedFingers.Count;
								rightFingers = rightExtendedFingers.Count;
							}
						}catch(Exception e){
							Debug.Log (e);
						};

						if(leftFingers > 0 && rightFingers > 0){
							fingerAlert.GetComponent<TextMesh>().text = "Chiudi bene le mani.";
						}
						else{
							if(leftFingers > 0)
								fingerAlert.GetComponent<TextMesh>().text = "Chiudi bene la mano sinistra.";
							if(rightFingers > 0)
								fingerAlert.GetComponent<TextMesh>().text = "Chiudi bene la mano destra.";
						}

						if(leftFingers < 1 && rightFingers < 1)
							fingerAlert.GetComponent<TextMesh>().text = "";
					}
				}
			}
			else{
				if(!PlayerSaveData.playerData.GetRightHand()){
					if(handController.GetComponent<HandController>().leftHandVisible){
						Vector3 temp = new Vector3(leapPlaceholder.transform.localPosition.x, leapPlaceholder.transform.localPosition.y, leftPlaceholder.transform.localPosition.z);
						if(hands[0].IsLeft){
							temp.x += (hands[0].PalmPosition.x * 0.05f);
							temp.y -= (hands[0].PalmPosition.z * 0.05f);
							leftPlaceholder.transform.localPosition = temp;
						}
						else{
							temp.x += (hands[1].PalmPosition.x * 0.05f);
							temp.y -= (hands[1].PalmPosition.z * 0.05f);
							leftPlaceholder.transform.localPosition = temp;
						}
					}

					if(!PlayerSaveData.playerData.GetOpenHandMode() &&
					   handController.GetComponent<HandController>().leftHandVisible){
						int leftFingers = -1;
						
						try{
							//leftFingers = leapLeftHand.hand.Fingers.Count;
							//rightFingers = leapRightHand.hand.Fingers.Count;
							if(hands[0].IsLeft){
								FingerList leftExtendedFingers = hands[0].Fingers.Extended();
								leftFingers = leftExtendedFingers.Count;
							}
							else{
								FingerList leftExtendedFingers = hands[1].Fingers.Extended();
								leftFingers = leftExtendedFingers.Count;
							}
						}catch(Exception e){
							Debug.Log (e);
						};
						
						if(leftFingers > 0){
							fingerAlert.GetComponent<TextMesh>().text = "Chiudi bene la mano.";
						}
						
						if(leftFingers < 1)
							fingerAlert.GetComponent<TextMesh>().text = "";
					}
				}
				if(PlayerSaveData.playerData.GetRightHand()){
					if(handController.GetComponent<HandController>().rightHandVisible){
						Vector3 temp = new Vector3(leapPlaceholder.transform.localPosition.x, leapPlaceholder.transform.localPosition.y, rightPlaceholder.transform.localPosition.z);
						if(hands[0].IsRight){
							temp.x += (hands[0].PalmPosition.x * 0.05f);
							temp.y -= (hands[0].PalmPosition.z * 0.05f);
							rightPlaceholder.transform.localPosition = temp;
						}
						else{
							temp.x += (hands[1].PalmPosition.x * 0.05f);
							temp.y -= (hands[1].PalmPosition.z * 0.05f);
							rightPlaceholder.transform.localPosition = temp;
						}
					}
					if(!PlayerSaveData.playerData.GetOpenHandMode() &&
					   handController.GetComponent<HandController>().rightHandVisible){
						int rightFingers = -1;
						
						try{
							//leftFingers = leapLeftHand.hand.Fingers.Count;
							//rightFingers = leapRightHand.hand.Fingers.Count;
							if(hands[0].IsRight){
								FingerList rightExtendedFingers = hands[0].Fingers.Extended();
								rightFingers = rightExtendedFingers.Count;
							}
							else{
								FingerList rightExtendedFingers = hands[1].Fingers.Extended();
								rightFingers = rightExtendedFingers.Count;
							}
						}catch(Exception e){
							Debug.Log (e);
						};
						
						if(rightFingers > 0){
							fingerAlert.GetComponent<TextMesh>().text = "Chiudi bene la mano.";
						}
						
						if(rightFingers < 1)
							fingerAlert.GetComponent<TextMesh>().text = "";
					}
				}
			}
		}
	}

	// Aggiorna la posizione delle mani sulla parte bassa dello schermo
	void UpdatePlaceholderPositions(float startDist, float handsDist){
		float newPlaceHolderDist = (handsDist*placeHolderStartPos)/startDist;
		float increment = placeHolderStartPos - newPlaceHolderDist;
		float xLeft = leftPlStartX + increment / 2;
		float xRight = rightPlStartX - increment / 2;
		Vector3 leftTemp = leftPlaceholder.transform.localPosition;
		leftTemp.x = xLeft;
		Vector3 rightTemp = rightPlaceholder.transform.localPosition;
		rightTemp.x = xRight;
		leftPlaceholder.transform.localPosition = leftTemp;
		rightPlaceholder.transform.localPosition = rightTemp;
	}

	public float GetHandDistance(){
		return handDistance;
	}
}
