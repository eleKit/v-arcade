using UnityEngine;
using System.Collections;

public class RadarScript : MonoBehaviour {

	public bool xz, yz;
	public GameObject spaceship;

	Vector3 xzOffset = new Vector3(0f,500f,70f);
	Vector3 yzOffset = new Vector3(-500f,0f,100f);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GameObject.FindGameObjectWithTag("GameController").GetComponent<FisioStarshipGameController>().IsInGame()){
			if(xz)
				transform.position = spaceship.transform.position + xzOffset;

			if(yz)
				transform.position = spaceship.transform.position + yzOffset;
		}
		else{
			Vector3 tempXZ = xzOffset;
			tempXZ.z = -xzOffset.z;

			Vector3 tempYZ = yzOffset;
			tempYZ.z = -yzOffset.z;

			if(xz)
				transform.position = spaceship.transform.position + tempXZ;
			
			if(yz)
				transform.position = spaceship.transform.position + tempYZ;
		}
	}
}
