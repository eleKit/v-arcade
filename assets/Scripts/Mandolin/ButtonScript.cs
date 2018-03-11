using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	float speed;

	bool start;

	// Use this for initialization
	void Start () {
		speed = MusicSaveData.musicData.GetButtonSpeed();
	}
	
	// Update is called once per frame
	void Update () {
		start = MusicSaveData.musicData.GetStart ();
	}

	void FixedUpdate(){
		if(start){
			GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.down) * speed;
		}
		else
			GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.down) * 0f;
	}


	public void DestroyButtonBad(){
		Destroy (gameObject);
	}

	public void DestroyButtonGood(){
		PlayerSaveData.playerData.SetScore(PlayerSaveData.playerData.GetScore() + MusicSaveData.musicData.GetButtonScore());
		Destroy (gameObject);
	}
}
