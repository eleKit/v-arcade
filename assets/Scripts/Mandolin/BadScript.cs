using UnityEngine;
using System.Collections;

public class BadScript : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if(other.gameObject.name.Equals("Bad") && base.transform.position.x == MusicSaveData.musicData.GetLeftGuideX()){
			GameObject.FindGameObjectWithTag ("GameController").SendMessage ("OpsL");
			GameObject.FindGameObjectWithTag ("GameController").SendMessage ("LeftBackToRed");
			transform.root.gameObject.SendMessage("DestroyButtonBad");
		}

		if(other.gameObject.name.Equals("Bad") && base.transform.position.x == MusicSaveData.musicData.GetRightGuideX()){
			GameObject.FindGameObjectWithTag ("GameController").SendMessage ("OpsR");
			GameObject.FindGameObjectWithTag ("GameController").SendMessage ("RightBackToRed");
			transform.root.gameObject.SendMessage("DestroyButtonBad");
		}
	}
}
