using UnityEngine;
using System.Collections;

public class AltimeterAnimator : MonoBehaviour {

	public Transform lancetta, water, plane;
	public GameObject unita, decine, centinaia, migliaia, altimeter;

	float metersToDegrees = 360f / 1200f;

	string zeroTexture = "Textures/Plane/Altimetro/zero";
	string unoTexture = "Textures/Plane/Altimetro/uno";
	string dueTexture = "Textures/Plane/Altimetro/due";
	string treTexture = "Textures/Plane/Altimetro/tre";
	string quattroTexture = "Textures/Plane/Altimetro/quattro";
	string cinqueTexture = "Textures/Plane/Altimetro/cinque";
	string seiTexture = "Textures/Plane/Altimetro/sei";
	string setteTexture = "Textures/Plane/Altimetro/sette";
	string ottoTexture = "Textures/Plane/Altimetro/otto";
	string noveTexture = "Textures/Plane/Altimetro/nove";

	string[] textures = new string[10];

	

	// Use this for initialization
	void Start () {
		textures [0] = zeroTexture;
		textures [1] = unoTexture;
		textures [2] = dueTexture;
		textures [3] = treTexture;
		textures [4] = quattroTexture;
		textures [5] = cinqueTexture;
		textures [6] = seiTexture;
		textures [7] = setteTexture;
		textures [8] = ottoTexture;
		textures [9] = noveTexture;
	}
	
	// Update is called once per frame
	void Update () {
		float z = altimeter.transform.position.z+0.001f;
		Vector3 temp = new Vector3 (altimeter.transform.position.x, altimeter.transform.position.y, z);
		transform.position = temp;
		float distance = Mathf.Abs (plane.position.y - water.position.y);
		if (water.position.y > plane.position.y)
			distance = 0;
		lancetta.rotation = Quaternion.Euler (0f, 0f , distance * metersToDegrees);
		int dist = (int)Mathf.Round (distance);
		int mil = dist / 1000;
		int cent = (dist - (1000 * mil))/100;
		int dec = (dist - (1000 * mil) - (100 * cent)) / 10;
		int unit = (dist - (1000 * mil) - (100 * cent)) % 10;
		unita.GetComponent<Renderer>().material.mainTexture = (Texture)Resources.Load (textures[unit]);
		decine.GetComponent<Renderer>().material.mainTexture = (Texture)Resources.Load (textures[dec]);
		centinaia.GetComponent<Renderer>().material.mainTexture = (Texture)Resources.Load (textures[cent]);
		migliaia.GetComponent<Renderer>().material.mainTexture = (Texture)Resources.Load (textures[mil]);
	}
}
