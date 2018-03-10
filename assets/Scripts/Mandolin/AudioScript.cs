using UnityEngine;
using System.Collections;

public class AudioScript : MonoBehaviour {

	public AudioClip badSound, goodSound;

	// Use this for initialization
	void Start () {
	
	}
	

	public void PlayBadSound(){
		audio.clip = badSound;
		audio.Play ();
	}

	public void PlayGoodSound(){
		audio.clip = goodSound;
		audio.Play ();
	}

}
