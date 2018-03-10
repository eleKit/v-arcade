using UnityEngine;
using System.Collections;
using System;
//Questo script va attaccato al gameobject HandController
public class LeftXRotationStatsScript : MonoBehaviour {

	public float maxExtension = 70;
	public float maxFlexion = 75;
	public float xExtension = 0;
	private static float Q = 0.000001f;
	private static float R = 0.01f;
	private static float P = 1f, X = 0f, K;
	HandModel hand;

	float[] xExtensions;
	int count = 0;

	
	static int numExtensions = 20;

	// Use this for initialization
	void Start () {
		xExtensions = new float[numExtensions];
		gameObject.transform.eulerAngles = Vector3.zero;

	}

	// Recupera l'estensione verticale dalla rotazione del polso rispetto all'asse x,
	// la aggiunge all'array delle ultime numExtensions estensioni,
	// su questo applica il filtro di kalman e restituisce l'estensione finale
	void Update () {
		Vector3 rot = gameObject.GetComponent<HandController> ().leftPalmRotation;
		float xAngle = rot.x;
		float onScreen = 0f;
		
		if (xAngle > 0 && xAngle <= 180){
			onScreen = Mathf.Round(xAngle*100f)/100f;
			
			if(count < numExtensions-1){
				xExtensions[count] = onScreen;
			}
			else{
				ShiftArray(xExtensions);
				xExtensions[numExtensions-1] = onScreen;
			}

		}
		else if (xAngle > 180 && xAngle < 360){
			onScreen = Mathf.Round((360 - xAngle)*100f)/100f;
			
			if(count < numExtensions-1){
				xExtensions[count] = -onScreen;
			}
			else{
				ShiftArray(xExtensions);
				xExtensions[numExtensions-1] = -onScreen;
			}

		}
		xExtension = PerfomKalmanTest (xExtensions);
		if(count < numExtensions-1)
			count++;
	}

	public float GetXExtension(){
		return xExtension;
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
