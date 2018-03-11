using UnityEngine;
using System.Collections;

public class UpdateTicksScript : MonoBehaviour {

	static string tickTexture = "Textures/Plane/Tick_red";

	GameObject thumbT, indexT, middleT, ringT, pinkyT;
	string transparent;

	// Use this for initialization
	void Start () {
		transparent = PlayerStarshipGameController.transparentTexture;
		thumbT = transform.Find("thumb_tick").gameObject;
		indexT = transform.Find("index_tick").gameObject;
		middleT = transform.Find("middle_tick").gameObject;
		ringT = transform.Find("ring_tick").gameObject;
		pinkyT = transform.Find("pinky_tick").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateTicks(bool thumb, bool index, bool middle, bool ring, bool pinky){
		if(thumb)
			thumbT.GetComponent<Renderer>().material.mainTexture = (Texture) Resources.Load (tickTexture);
		else
			thumbT.GetComponent<Renderer>().material.mainTexture = (Texture) Resources.Load (transparent);

		if(index)
			indexT.GetComponent<Renderer>().material.mainTexture = (Texture) Resources.Load (tickTexture);
		else
			indexT.GetComponent<Renderer>().material.mainTexture = (Texture) Resources.Load (transparent);

		if(middle)
			middleT.GetComponent<Renderer>().material.mainTexture = (Texture) Resources.Load (tickTexture);
		else
			middleT.GetComponent<Renderer>().material.mainTexture = (Texture) Resources.Load (transparent);

		if(ring)
			ringT.GetComponent<Renderer>().material.mainTexture = (Texture) Resources.Load (tickTexture);
		else
			ringT.GetComponent<Renderer>().material.mainTexture = (Texture) Resources.Load (transparent);

		if(pinky)
			pinkyT.GetComponent<Renderer>().material.mainTexture = (Texture) Resources.Load (tickTexture);
		else
			pinkyT.GetComponent<Renderer>().material.mainTexture = (Texture) Resources.Load (transparent);

	}
}
