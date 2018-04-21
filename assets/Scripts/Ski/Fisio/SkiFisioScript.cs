using UnityEngine;
using System.Collections;

public class SkiFisioScript : MonoBehaviour {

	public float leftGuideX = -3f;
	public float rightGuideX = 3f;
	public float centerGuideX = 0f;
	float movBonus = 0.075f;
	float movSpeed = 3f;
	
	bool goLeft, goRight, onLeft, onCenter, onRight;
	public bool leftText, rightText;
	public bool standText = true;
	
	public Material stand, left, right;
	
	public GameObject controller, handController;
	
	public float playerSpeed = 1.5f;
	
	public AudioClip turn, crash, loop, good;

	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(SkiSaveData.skiData.GetStepMode()){
			if(standText){
				GetComponent<Renderer>().material = stand;
			}
			else if(rightText){
				GetComponent<Renderer>().material = right;
			}
			else if(leftText){
				GetComponent<Renderer>().material = left;
			}
			
			if(transform.position.x == leftGuideX){
				onLeft = true;
				onCenter = false;
				onRight = false;
			}
			else if(transform.position.x == rightGuideX){
				onLeft = false;
				onCenter = false;
				onRight = true;
			}
			else if(transform.position.x == centerGuideX){
				onLeft = false;
				onCenter = true;
				onRight = false;
			}
			else{
				onLeft = false;
				onCenter = false;
				onRight = false;
			}
			
			if(goLeft && !onLeft && !controller.GetComponent<FisioSkiController>().IsTesting()){
				if(onCenter){
					GetComponent<Animation>().Play("Center_Left_Ski_D");
					goLeft = false;
				}
				if(onRight){
					GetComponent<Animation>().Play("Right_Center_Ski_D");
					goLeft = false;
				}
				//transform.Translate (Vector3.left * playerSpeed * Time.deltaTime);
			}
			if(goRight && !onRight && !controller.GetComponent<FisioSkiController>().IsTesting()){
				if(onCenter){
					GetComponent<Animation>().Play("Center_Right_Ski_D");
					goRight = false;
				}
				if(onLeft){
					GetComponent<Animation>().Play("Left_Center_Ski_D");
					goRight = false;
				}
				//transform.Translate (Vector3.left * playerSpeed * Time.deltaTime);
			}

			if(goLeft && !onLeft && controller.GetComponent<FisioSkiController>().IsTesting()){
				if(onCenter){
					GetComponent<Animation>().Play("Center_Left_Ski");
					goLeft = false;
				}
				if(onRight){
					GetComponent<Animation>().Play("Right_Center_Ski");
					goLeft = false;
				}
				//transform.Translate (Vector3.left * playerSpeed * Time.deltaTime);
			}
			if(goRight && !onRight && controller.GetComponent<FisioSkiController>().IsTesting()){
				if(onCenter){
					GetComponent<Animation>().Play("Center_Right_Ski");
					goRight = false;
				}
				if(onLeft){
					GetComponent<Animation>().Play("Left_Center_Ski");
					goRight = false;
				}
				//transform.Translate (Vector3.left * playerSpeed * Time.deltaTime);
			}
		}
		
		if(SkiSaveData.skiData.GetSmoothMode()){
			float dt = Time.deltaTime;
			float tmpx = 0f;
			
			if(SkiSaveData.skiData.GetSlapMode()){
				if(handController.GetComponent<MyHandController>().rightHandVisible && PlayerSaveData.playerData.GetRightHand())
					tmpx = handController.GetComponent<RightYRotationStatsScript>().GetYExtension();
				else if(handController.GetComponent<MyHandController>().leftHandVisible && !PlayerSaveData.playerData.GetRightHand())
					tmpx = handController.GetComponent<LeftYRotationStatsScript>().GetYExtension();
			}
			else{
				if(handController.GetComponent<MyHandController>().rightHandVisible && PlayerSaveData.playerData.GetRightHand())
					tmpx = handController.GetComponent<RightYRotationStatsScript>().GetYExtension();
				else if(handController.GetComponent<MyHandController>().leftHandVisible && !PlayerSaveData.playerData.GetRightHand())
					tmpx = handController.GetComponent<LeftYRotationStatsScript>().GetYExtension();
			}
			
			if(tmpx*movBonus < leftGuideX)
				tmpx = leftGuideX/movBonus;
			
			if(tmpx*movBonus > rightGuideX)
				tmpx = rightGuideX/movBonus;
			
			Vector3 dest = new Vector3 ( tmpx*movBonus, gameObject.transform.position.y,gameObject.transform.position.z);
			
			gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, dest, movSpeed * dt);
			
		}
	}
	
	public void SlapLeft(bool rightHand){
		if((rightHand && PlayerSaveData.playerData.GetRightHand()) || (!rightHand && !PlayerSaveData.playerData.GetRightHand())){
			if(SkiSaveData.skiData.GetStart() && SkiSaveData.skiData.GetSlapMode())
				goLeft = true;
		}
		Debug.Log ("Slap left");
	}
	
	public void SlapRight(bool rightHand){
		if((rightHand && PlayerSaveData.playerData.GetRightHand()) || (!rightHand && !PlayerSaveData.playerData.GetRightHand())){
			if(SkiSaveData.skiData.GetStart() && SkiSaveData.skiData.GetSlapMode())
				goRight = true;
		}
		Debug.Log ("Slap right");
	}
	
	
	public void SlideLeft(bool rightHand){
		if((rightHand && PlayerSaveData.playerData.GetRightHand()) || (!rightHand && !PlayerSaveData.playerData.GetRightHand())){
			if(SkiSaveData.skiData.GetStart() && SkiSaveData.skiData.GetSlideMode())
				goLeft = true;
		}
		Debug.Log ("Slide left");
	}
	
	public void SlideRight(bool rightHand){
		if((rightHand && PlayerSaveData.playerData.GetRightHand()) || (!rightHand && !PlayerSaveData.playerData.GetRightHand())){
			if(SkiSaveData.skiData.GetStart() && SkiSaveData.skiData.GetSlideMode())
				goRight = true;
		}
		Debug.Log ("Slide right");
	}
	
	public void BackToCenter(){
		Vector3 temp = new Vector3 (centerGuideX, transform.position.y, transform.position.z);
		transform.position = temp;
	}

}
