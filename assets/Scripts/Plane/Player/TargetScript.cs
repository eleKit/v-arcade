using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour {

	GameObject controller;

	// Use this for initialization
	void Start () {
		controller = GameObject.FindGameObjectWithTag("GameController");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if(controller.GetComponent<PlayerStarshipGameController>() != null){
			controller.GetComponent<PlayerStarshipGameController>().HitTarget();
			Destroy(this.collider);
			transform.Find("Points").GetComponent<TextMesh>().text = "+" + PathSaveData.pathData.GetTargetPoints().ToString();
			StartCoroutine(Wait());
		}
	}

	IEnumerator Wait(){
		yield return new WaitForSeconds (0.4f);
		transform.Find ("Points").GetComponent<TextMesh> ().text = "";
		Destroy (gameObject);
	}
}
