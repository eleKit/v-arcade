using UnityEngine;
using System.Collections;
//Questo script va attaccato al gameobject HandController
public class LeftYRotationStatsScript : MonoBehaviour {


	public float maxUlnarRotation = 20;
	public float maxRadialRotation = 30;
	public float yExtension = 0;
	private static float Q = 0.000001f;
	private static float R = 0.01f;
	private static float P = 1f, X = 0f, K;

	static int numExtensions = 20;

	float[] yExtensions;
	int count = 0;

	// Use this for initialization
	void Start () {
		yExtensions = new float[numExtensions];

	}
	
	// Recupera l'estensione orizzontale dalla rotazione del polso rispetto all'asse y,
	// la aggiunge all'array delle ultime numExtensions estensioni,
	// su questo applica il filtro di kalman e restituisce l'estensione finale
	void Update () {
		Vector3 rot = gameObject.GetComponent<MyHandController>().leftPalmRotation;
		float yAngle = rot.y;
		//Debug.Log ("Angle: " + yAngle);
		float onScreen = 0f;
		if(yAngle == 0)
			return;

		if (yAngle > 0 && yAngle <= 180){
			onScreen = Mathf.Round(yAngle*100f)/100f;
			//Debug.Log ("Round: " + onScreen);
			
			if(count < numExtensions-1){
				yExtensions[count] = onScreen;
			}
			else{
				ShiftArray(yExtensions);
				yExtensions[numExtensions-1] = onScreen;
			}
			
		}
		else if (yAngle > 180 && yAngle < 360){
			onScreen = Mathf.Round((360 - yAngle)*100f)/100f;
			//Debug.Log ("Round: " + onScreen);
			
			if(count < numExtensions-1){
				yExtensions[count] = -onScreen;
			}
			else{
				ShiftArray(yExtensions);
				yExtensions[numExtensions-1] = -onScreen;
			}
			
		}
		yExtension = PerfomKalmanTest (yExtensions);
		//Debug.Log ("Extension: " + yExtension);
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

	public float GetYExtension(){
		return yExtension;
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
