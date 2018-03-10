using UnityEngine;
using System.Collections;

public class CameraFollowScript : MonoBehaviour {

	public GameObject plane, leftOverlay, leftPlaceholder, rightOverlay, rightPlaceholder, leapPlaceholder;
	float yoffset = 0f;
	float zoffset = 0f;
	Vector3 leftOverlayLocalPosition, rightOverlayLocalPosition;

	// Use this for initialization
	void Start () {
		if(leftOverlay != null){
			leftOverlay.SetActive(false);
			leftPlaceholder.SetActive(false);
			rightOverlay.SetActive(false);
			rightPlaceholder.SetActive(false);
			leapPlaceholder.SetActive(false);
		}

		yoffset = transform.position.y - plane.transform.position.y;
		zoffset = transform.position.z - plane.transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		float newX = plane.transform.position.x;
		float newY = plane.transform.position.y + yoffset;
		float newZ = plane.transform.position.z + zoffset;

		transform.position = new Vector3 (newX,newY,newZ);
		if(GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerStarshipGameController>()!= null){
			if(GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerStarshipGameController>().GetTrack()){
				if(PlayerSaveData.playerData.GetOneHandMode() && Application.loadedLevelName.Equals("Player_Plane")){
					leftOverlay.SetActive(true);
					leftPlaceholder.SetActive(true);
					rightOverlay.SetActive(true);
					rightPlaceholder.SetActive(true);
					leapPlaceholder.SetActive(true);
					leftOverlayLocalPosition = leftOverlay.transform.localPosition;
					rightOverlayLocalPosition = rightOverlay.transform.localPosition;
					if(PlayerSaveData.playerData.GetRightHand()){
						leftOverlay.SetActive(false);
						leftPlaceholder.SetActive(false);
						rightOverlay.SetActive(false);
						Vector3 tmp = rightPlaceholder.transform.localPosition;
						tmp.x = leapPlaceholder.transform.localPosition.x;
						rightPlaceholder.transform.localPosition = tmp;
					}
					else{
						leftOverlay.SetActive(false);
						rightOverlay.SetActive(false);
						rightPlaceholder.SetActive(false);
						Vector3 tmp = leftPlaceholder.transform.localPosition;
						tmp.x = leapPlaceholder.transform.localPosition.x;
						leftPlaceholder.transform.localPosition = tmp;
					}
					
				}
				else if(!PlayerSaveData.playerData.GetOneHandMode() && Application.loadedLevelName.Equals("Player_Plane")){
					leftOverlay.SetActive(true);
					leftPlaceholder.SetActive(true);
					rightOverlay.SetActive(true);
					rightPlaceholder.SetActive(true);
					leapPlaceholder.SetActive(false);
				}
			}
		}

//		if(PlayerSaveData.playerData.GetOneHandMode() && Application.loadedLevelName.Equals("Player_Plane")){
//			if(PlayerSaveData.playerData.GetRightHand()){
//				leftOverlay.SetActive(false);
//				leftPlaceholder.SetActive(false);
//				rightOverlay.SetActive(false);
//				rightPlaceholder.SetActive(true);
//			}
//			else{
//				leftOverlay.SetActive(false);
//				rightOverlay.SetActive(false);
//				rightPlaceholder.SetActive(false);
//				leftPlaceholder.SetActive(true);
//			}
//			
//		}
//		else if(!PlayerSaveData.playerData.GetOneHandMode() && Application.loadedLevelName.Equals("Player_Plane"))
//			leapPlaceholder.SetActive(false);
	}
}
