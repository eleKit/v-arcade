using UnityEngine;
using System.Collections;

public class GoodScript : MonoBehaviour {

	bool good;

	void OnTriggerEnter(Collider other){
		if(other.gameObject.name.Equals("Good")){
			good = true;
			if(base.transform.position.x == MusicSaveData.musicData.GetLeftGuideX())
				GameObject.FindGameObjectWithTag ("GameController").SendMessage ("GreenLeftCircle");
			if(base.transform.position.x == MusicSaveData.musicData.GetRightGuideX())
				GameObject.FindGameObjectWithTag ("GameController").SendMessage ("GreenRightCircle");
		}
	}

	void Update(){
		if(good){
			if(base.transform.position.x == MusicSaveData.musicData.GetLeftGuideX() &&
			   (Input.GetKeyDown(KeyCode.LeftArrow) || MusicSaveData.musicData.GetLeftGesture())){
				GetComponent<AudioSource>().Play();
				GameObject.FindGameObjectWithTag ("GameController").SendMessage ("YeahL");
				GameObject.FindGameObjectWithTag ("GameController").SendMessage ("LeftBackToRed");
				transform.root.gameObject.SendMessage ("DestroyButtonGood");
			}
			if(base.transform.position.x == MusicSaveData.musicData.GetRightGuideX() &&
			   (Input.GetKeyDown(KeyCode.RightArrow) || MusicSaveData.musicData.GetRightGesture())){
				GetComponent<AudioSource>().Play();
				GameObject.FindGameObjectWithTag ("GameController").SendMessage ("YeahR");
				GameObject.FindGameObjectWithTag ("GameController").SendMessage ("RightBackToRed");
				transform.root.gameObject.SendMessage ("DestroyButtonGood");
			}
		}
	}
}
