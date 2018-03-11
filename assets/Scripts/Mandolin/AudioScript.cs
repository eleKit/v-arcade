using UnityEngine;
using System.Collections;

public class AudioScript : MonoBehaviour {

	public AudioClip badSound, goodSound;

	// Use this for initialization
	void Start () {
	
	}
	

	public void PlayBadSound(){
		GetComponent<AudioSource>().clip = badSound;
		GetComponent<AudioSource>().Play ();
	}

	public void PlayGoodSound(){
		GetComponent<AudioSource>().clip = goodSound;
		GetComponent<AudioSource>().Play ();
	}

}
