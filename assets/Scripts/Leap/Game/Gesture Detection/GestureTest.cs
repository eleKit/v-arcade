using UnityEngine;
using System.Collections;

public class GestureTest : MonoBehaviour {

	void JumpUp(bool isRight){
		if(isRight)
			Debug.Log ("Jump right");
		else
			Debug.Log ("Jump left");
	}

	void GoDown(bool isRight){
		if(isRight)
			Debug.Log ("Down right");
		else
			Debug.Log ("Down left");
	}

	void SlideRight(bool isRight){
		if(isRight)
			Debug.Log ("Right slide right");
		else
			Debug.Log ("Left slide right");
	}

	void SlideLeft(bool isRight){
		if(isRight)
			Debug.Log ("Right slide left");
		else
			Debug.Log ("Left slide left");
	}

	void SlapRight(bool isRight){
		if(isRight)
			Debug.Log ("Right slap right");
		else
			Debug.Log ("Left slap right");
	}
	
	void SlapLeft(bool isRight){
		if(isRight)
			Debug.Log ("Right slap left");
		else
			Debug.Log ("Left slap left");
	}

}
