using UnityEngine;
using System.Collections;
//Questo script va attaccato al gameobject HandController
public class LeftZRotationStatsScript : MonoBehaviour {

	public float maxUlnarRotation = 20;
	public float maxRadialRotation = 30;
	public float zExtension = 0;
	private static float Q = 0.000001f;
	private static float R = 0.01f;
	private static float P = 1f, X = 0f, K;
	
	static int numExtensions = 20;
	
	float[] zExtensions;
	int count = 0;
	
	// Use this for initialization
	void Start () {
		zExtensions = new float[numExtensions];

	}
	
	//Calcola l'estensione dalla rotazione del polso rispetto all'asse z
	// Per il momento non viene utilizzatonel calcolo finale, perché non è una vera e propria estensione del polso
	void Update () {
		Vector3 rot = gameObject.GetComponent<HandController> ().leftPalmRotation;
//		Vector3 temp = transform.eulerAngles;
//		temp.z = 0f;
//		transform.eulerAngles = temp;

		float zAngle = rot.z;
		float onScreen = 0f;
		
		if (zAngle > 0 && zAngle <= 180){
			onScreen = Mathf.Round(zAngle*100f)/100f;
			
			if(count < numExtensions-1){
				zExtensions[count] = -onScreen;
			}
			else{
				ShiftArray(zExtensions);
				zExtensions[numExtensions-1] = -onScreen;
			}
		}
		else if (zAngle > 180 && zAngle < 360){
			onScreen = Mathf.Round((360 - zAngle)*100f)/100f;
			
			if(count < numExtensions-1){
				zExtensions[count] = onScreen;
			}
			else{
				ShiftArray(zExtensions);
				zExtensions[numExtensions-1] = onScreen;
			}
		}
		
		zExtension = PerfomKalmanTest (zExtensions);
		if(count < numExtensions-1)
			count++;
		
	}
	

	
	void ShiftArray(float[] arr){
		float[] tempArray = new float[arr.Length];
		for (int i = 1; i < arr.Length; i++)
			tempArray[i - 1] = arr[i];
		tempArray.CopyTo (arr, 0);
	}
	
	float MeanArray(float[] array){
		float sum = 0;
		for(int i = 0; i < array.Length; i++)
			sum += array[i];
		return (sum / array.Length);
	}
	
	public float GetZExtension(){
		return zExtension;
	}

	float PerfomKalmanTest(float[] DATA)
	{
		float[] temp = new float[numExtensions];
		
		for (int i = 0; i < numExtensions; i++)
		{
			//Debug.Log(Mathf.Round(SimpleKalman.update(DATA[i])) + ",");
			temp[i] = UpdateK(DATA[i]);
		}
		return temp [count];
	}
	
	private static void MeasurementUpdate()
	{
		K = (P + Q) / (P + Q + R);
		P = R * (P + Q) / (R + P + Q);
	}
	
	public static float UpdateK(float measurement)
	{
		MeasurementUpdate();
		float result = X + (measurement - X) * K;
		X = result;
		return result;
	}
}
