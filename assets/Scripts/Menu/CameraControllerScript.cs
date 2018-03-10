using UnityEngine;
using System.Collections;

public class CameraControllerScript : MonoBehaviour {

	static float mainPosition = 0f;
	static float playerTuningPosition = -30f;

	bool onMain = true;

	public void ChangeFocusToPlayerTuning(){
		Vector3 temp = transform.position;
		temp.x = playerTuningPosition;
		transform.position = temp;
	}

	public void ChangeFocusToMain(){
		Vector3 temp = transform.position;
		temp.x = mainPosition;
		transform.position = temp;
	}

	public bool IsOnMain(){
		return onMain;
	}

	public void FinishedTuning(){
		ChangeFocusToMain ();
		SendMessage ("ShowPlayer");
	}
}
