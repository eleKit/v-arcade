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
			thumbT.renderer.material.mainTexture = (Texture) Resources.Load (tickTexture);
		else
			thumbT.renderer.material.mainTexture = (Texture) Resources.Load (transparent);

		if(index)
			indexT.renderer.material.mainTexture = (Texture) Resources.Load (tickTexture);
		else
			indexT.renderer.material.mainTexture = (Texture) Resources.Load (transparent);

		if(middle)
			middleT.renderer.material.mainTexture = (Texture) Resources.Load (tickTexture);
		else
			middleT.renderer.material.mainTexture = (Texture) Resources.Load (transparent);

		if(ring)
			ringT.renderer.material.mainTexture = (Texture) Resources.Load (tickTexture);
		else
			ringT.renderer.material.mainTexture = (Texture) Resources.Load (transparent);

		if(pinky)
			pinkyT.renderer.material.mainTexture = (Texture) Resources.Load (tickTexture);
		else
			pinkyT.renderer.material.mainTexture = (Texture) Resources.Load (transparent);

	}
}
