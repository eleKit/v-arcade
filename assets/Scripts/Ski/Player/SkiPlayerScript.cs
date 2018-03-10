using UnityEngine;
using System.Collections;

public class SkiPlayerScript : MonoBehaviour {

	float leftGuideX = -3f;
	float rightGuideX = 3f;
	float centerGuideX = 0f;
	float movBonus = 0.075f;
	float movSpeed = 3f;

	bool goLeft, goRight, onLeft, onCenter, onRight;
	Vector3 dest;
	public bool leftText, rightText, gotTunings;
	public bool standText = true;

	public Material stand, left, right;

	public GameObject controller, handController;

	//Player tunings
	float minLeftVertical, maxLeftVertical, minLeftHorizontal, maxLeftHorizontal;
	float minRightVertical, maxRightVertical, minRightHorizontal, maxRightHorizontal;

	public float playerSpeed = 1.5f;

	public AudioClip turn, crash, loop, good;

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag.Equals("Tree")){
			SkiSaveData.skiData.SetStart(false);
			audio.clip = crash;
			audio.loop = false;
			audio.Play();
			InvokeRepeating("BackToLoopAudioEnd", audio.clip.length, 1);
			if(SkiSaveData.skiData.GetRandomTreePath())
				SkiSaveData.skiData.UpdateSkiTreeHighscore(PlayerSaveData.playerData.GetUserName(),PlayerSaveData.playerData.GetScore());
			else{
				SkiSaveData.skiData.UpdateHighscore(PlayerSaveData.playerData.GetCurrentPathName(),PlayerSaveData.playerData.GetUserName(), PlayerSaveData.playerData.GetScore());
				GeneralSaveData.generalData.UpdatePlayerSkiHighscore(PlayerSaveData.playerData.GetUserName(), PlayerSaveData.playerData.GetCurrentPathName(), PlayerSaveData.playerData.GetScore());
			}
			controller.SendMessage("StopTracking");
			controller.SendMessage("EndMenu");
		}
		else if(other.gameObject.tag.Equals("Finish_Line")){
			SkiSaveData.skiData.SetStart(false);
			if(SkiSaveData.skiData.GetRandomFlagPath())
				SkiSaveData.skiData.UpdateSkiFlagHighscore(PlayerSaveData.playerData.GetUserName(),PlayerSaveData.playerData.GetScore());
			else if(SkiSaveData.skiData.GetRandomTreePath())
				SkiSaveData.skiData.UpdateSkiTreeHighscore(PlayerSaveData.playerData.GetUserName(),PlayerSaveData.playerData.GetScore());
			else{
				SkiSaveData.skiData.UpdateHighscore(PlayerSaveData.playerData.GetCurrentPathName(),PlayerSaveData.playerData.GetUserName(), PlayerSaveData.playerData.GetScore());
				GeneralSaveData.generalData.UpdatePlayerSkiHighscore(PlayerSaveData.playerData.GetUserName(), PlayerSaveData.playerData.GetCurrentPathName(), PlayerSaveData.playerData.GetScore());
			}
			controller.SendMessage("StopTracking");
			controller.SendMessage("EndMenu");
		}
		else if((other.gameObject.tag.Equals("Tree_Generator") && !SkiSaveData.skiData.GetSmoothMode()) || other.gameObject.tag.Equals("Flags")){
			audio.clip = good;
			audio.loop = false;
			audio.Play();
			InvokeRepeating("BackToLoopAudio", audio.clip.length, 1);
			PlayerSaveData.playerData.SetScore(PlayerSaveData.playerData.GetScore() + SkiSaveData.skiData.GetPoints());
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(PlayerSaveData.playerData.GetPlayer() != null && !gotTunings & !SaveInfos.replay){
			GetTunings (PlayerSaveData.playerData.GetUserName());
			GetComponent<LeftSlapGesture> ().SetTunings (minLeftVertical, maxLeftVertical);
			GetComponent<RightSlapGesture> ().SetTunings (minRightVertical, maxRightVertical);
			GetComponent<LeftSlideGesture> ().SetTunings (minLeftHorizontal, maxLeftHorizontal);
			GetComponent<RightSlideGesture> ().SetTunings (minRightHorizontal, maxRightHorizontal);
			gotTunings = true;
		}
		else if(SaveInfos.replay && PlayerSaveData.playerData.GetPlayerTunings().Count > 0){
			GetTunings (PlayerSaveData.playerData.GetUserName());
			GetComponent<LeftSlapGesture> ().SetTunings (minLeftVertical, maxLeftVertical);
			GetComponent<RightSlapGesture> ().SetTunings (minRightVertical, maxRightVertical);
			GetComponent<LeftSlideGesture> ().SetTunings (minLeftHorizontal, maxLeftHorizontal);
			GetComponent<RightSlideGesture> ().SetTunings (minRightHorizontal, maxRightHorizontal);
			gotTunings = true;
		}

		if(SkiSaveData.skiData.GetStepMode()){
			if(standText){
				renderer.material = stand;
			}
			else if(rightText){
				renderer.material = right;
			}
			else if(leftText){
				renderer.material = left;
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

			if(goLeft && !onLeft){
				if(onCenter){
					animation.Play("Center_Left_Ski");
					goLeft = false;
				}
				if(onRight){
					animation.Play("Right_Center_Ski");
					goLeft = false;
				}
				//transform.Translate (Vector3.left * playerSpeed * Time.deltaTime);
			}
			if(goRight && !onRight){
				if(onCenter){
					animation.Play("Center_Right_Ski");
					goRight = false;
				}
				if(onLeft){
					animation.Play("Left_Center_Ski");
					goRight = false;
				}
				//transform.Translate (Vector3.left * playerSpeed * Time.deltaTime);
			}
		}

		if(SkiSaveData.skiData.GetSmoothMode()){
			float dt = Time.deltaTime;
			float tmpx = 0f;

			if(SkiSaveData.skiData.GetSlapMode()){
				if(handController.GetComponent<HandController>().rightHandVisible && PlayerSaveData.playerData.GetRightHand())
					tmpx = handController.GetComponent<RightYRotationStatsScript>().GetYExtension();
				else if(handController.GetComponent<HandController>().leftHandVisible && !PlayerSaveData.playerData.GetRightHand())
					tmpx = handController.GetComponent<LeftYRotationStatsScript>().GetYExtension();

			}
			else{
				if(handController.GetComponent<HandController>().rightHandVisible && PlayerSaveData.playerData.GetRightHand())
					tmpx = handController.GetComponent<RightYRotationStatsScript>().GetYExtension();
				else if(handController.GetComponent<HandController>().leftHandVisible && !PlayerSaveData.playerData.GetRightHand())
					tmpx = handController.GetComponent<LeftYRotationStatsScript>().GetYExtension();
			}

			if(!gotTunings){
				if(tmpx*movBonus < leftGuideX)
					tmpx = leftGuideX/movBonus;

				if(tmpx*movBonus > rightGuideX)
					tmpx = rightGuideX/movBonus;
				dest = new Vector3 ( tmpx*movBonus, gameObject.transform.position.y,gameObject.transform.position.z);
			}
			else{
				if(SkiSaveData.skiData.GetSlapMode()){
					if(handController.GetComponent<HandController>().rightHandVisible && PlayerSaveData.playerData.GetRightHand()){
						if(tmpx < 0){
							tmpx = -(tmpx*leftGuideX)/minRightVertical;
							if(tmpx < leftGuideX)
								tmpx = leftGuideX;
						}
						else{
							tmpx = -(tmpx*rightGuideX)/maxRightVertical;
							if(tmpx > rightGuideX)
								tmpx = rightGuideX;
						}
					}
					else if(handController.GetComponent<HandController>().leftHandVisible && !PlayerSaveData.playerData.GetRightHand()){
						if(tmpx < 0){
							tmpx = (tmpx*leftGuideX)/maxLeftVertical;
							if(tmpx < leftGuideX)
								tmpx = leftGuideX;
						}
						else{
							tmpx = (tmpx*rightGuideX)/minLeftVertical;
							if(tmpx > rightGuideX)
								tmpx = rightGuideX;
						}
					}
				}
				else{
					if(handController.GetComponent<HandController>().rightHandVisible && PlayerSaveData.playerData.GetRightHand()){
						if(tmpx < 0){
							tmpx = (tmpx*leftGuideX)/minRightHorizontal;
							if(tmpx < leftGuideX)
								tmpx = leftGuideX;
						}
						else{
							tmpx = (tmpx*rightGuideX)/maxRightHorizontal;
							if(tmpx > rightGuideX)
								tmpx = rightGuideX;
						}
					}
					else if(handController.GetComponent<HandController>().leftHandVisible && !PlayerSaveData.playerData.GetRightHand()){
						if(tmpx < 0){
							tmpx = (tmpx*leftGuideX)/minLeftHorizontal;
							if(tmpx < leftGuideX)
								tmpx = leftGuideX;
						}
						else{
							tmpx = (tmpx*rightGuideX)/maxLeftHorizontal;
							if(tmpx > rightGuideX)
								tmpx = rightGuideX;
						}
					}
				}
				dest = new Vector3 ( tmpx, gameObject.transform.position.y,gameObject.transform.position.z);
			}



			
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

	void BackToLoopAudio(){
		audio.clip = loop;
		audio.loop = true;
		CancelInvoke ();
	}
	void BackToLoopAudioEnd(){
		audio.clip = loop;
		audio.loop = true;
		audio.Stop ();
		CancelInvoke ();
	}

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
}
