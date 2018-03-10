using UnityEngine;
using System.Collections;

public class YopScript : MonoBehaviour {

	public GameObject yeahL, opsL, yeahR, opsR;

	// Use this for initialization
	void Start () {
		yeahL.SetActive (false);
		opsL.SetActive(false);
		yeahR.SetActive (false);
		opsR.SetActive(false);
	}
	

	public void YeahL(){
		yeahL.SetActive (true);
		StartCoroutine("DelYeahL");
		SendMessage ("PlayGoodSound");
	}

	IEnumerator DelYeahL(){
		yield return new WaitForSeconds(1.1f);
		yeahL.SetActive (false);
	}

	public void YeahR(){
		yeahR.SetActive (true);
		StartCoroutine("DelYeahR");
		SendMessage ("PlayGoodSound");
	}
	
	IEnumerator DelYeahR(){
		yield return new WaitForSeconds(1.1f);
		yeahR.SetActive (false);
	}

	public void OpsL(){
		opsL.SetActive (true);
		StartCoroutine("DelOpsL");
		SendMessage ("PlayBadSound");
	}
	
	IEnumerator DelOpsL(){
		yield return new WaitForSeconds(1.1f);
		opsL.SetActive (false);
	}

	public void OpsR(){
		opsR.SetActive (true);
		StartCoroutine("DelOpsR");
		SendMessage ("PlayBadSound");
	}
	
	IEnumerator DelOpsR(){
		yield return new WaitForSeconds(1.1f);
		opsR.SetActive (false);
	}
}
