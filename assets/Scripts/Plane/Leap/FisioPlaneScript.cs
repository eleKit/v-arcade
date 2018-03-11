using UnityEngine;
using System.Collections;

public class FisioPlaneScript : MonoBehaviour {

	Vector3 startAngles;
	float xAngle, yAngle, zAngle;
//	float[] xAngles, yAngles, zAngles;
	bool anglesSet;
	
	float verticalTurnIncrement = 0.3f;
	float horizontalTurnIncrement = 0.5f;
	int count = 0;
	
	float speed = 30f;
	static int numAngles = 15;
	
	// Use this for initialization
	void Start () {
//		xAngles = new float[numAngles];
//		yAngles = new float[numAngles];
//		zAngles = new float[numAngles];
		startAngles = new Vector3(0f, 180f, 0f);
	}
	
	// Quasi uguale al metodo update del PlayerPlaneScript, manca solo la parte relativa ai bonus
	void Update () {
		speed = PlayerSaveData.playerData.GetPlaneSpeed ();
		if(Time.timeScale == 1f){


			float xRight = GetComponent<RightAnglesRetrieveScript> ().GetXExt ();
			float yRight = GetComponent<RightAnglesRetrieveScript> ().GetYExt ();
			//float zRight = GetComponent<RightAnglesRetrieveScript> ().GetZExt ();
			
			float xLeft = GetComponent<LeftAnglesRetrieveScript> ().GetXExt ();
			float yLeft = GetComponent<LeftAnglesRetrieveScript> ().GetYExt ();
			//float zLeft = GetComponent<LeftAnglesRetrieveScript> ().GetZExt ();

			if(!PlayerSaveData.playerData.GetOneHandMode()){
				xAngle = verticalTurnIncrement * (xRight + xLeft) / 2;
				yAngle = horizontalTurnIncrement * (yRight + yLeft) / 2;
				zAngle = 0f;
			}

			if(PlayerSaveData.playerData.GetOneHandMode() && !PlayerSaveData.playerData.GetRightHand()){
				xAngle = verticalTurnIncrement * xLeft;
				yAngle = horizontalTurnIncrement * yLeft;
				zAngle = 0f;
			}

			if(PlayerSaveData.playerData.GetOneHandMode() && PlayerSaveData.playerData.GetRightHand()){
				xAngle = verticalTurnIncrement * xRight;
				yAngle = horizontalTurnIncrement * yRight;
				zAngle = 0f;
			}
//			if(count < numAngles-1){
//				xAngles[count] = xAngle;
//				yAngles[count] = yAngle;
//				zAngles[count] = zAngle;
//			}
//			else{
//				ShiftArray(yAngles);
//				ShiftArray(xAngles);
//				ShiftArray(zAngles);
//				xAngles[numAngles-1] = xAngle;
//				yAngles[numAngles-1] = yAngle;
//				zAngles[numAngles-1] = zAngle;
//			}
			
			if(anglesSet){
				Vector3 updatedAngles = new Vector3(startAngles.x - xAngle, startAngles.y + yAngle, startAngles.z + zAngle);
				transform.eulerAngles = updatedAngles;
			}
			
			if(count < numAngles-1)
				count++;
		}
	}
	
	
	void FixedUpdate(){
		if(anglesSet)
			GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward) * speed;
	}
	
	float PerfomKalmanTest(float[] DATA)
	{
		float[] temp = new float[numAngles];
		
		for (int i = 0; i < numAngles; i++)
		{
			//Debug.Log(Mathf.Round(SimpleKalman.update(DATA[i])) + ",");
			temp[i] = SimpleKalman.update(DATA[i]);
		}
		return temp [count];
	}
	
//	void ShiftArray(float[] arr){
//		float[] tempArray = new float[arr.Length];
//		for (int i = 1; i < arr.Length; i++)
//			tempArray[i - 1] = arr[i];
//		tempArray.CopyTo (arr, 0);
//	}
	
	public void ResetAngles(){
		transform.eulerAngles = startAngles;
	}

	public void SetStartAngles(){
		if(!anglesSet)
			startAngles = new Vector3 (-xAngle, -yAngle, 0f);
		anglesSet = true;
		GetComponent<PositionTrackerScript> ().StartTracking ();
	}
}
