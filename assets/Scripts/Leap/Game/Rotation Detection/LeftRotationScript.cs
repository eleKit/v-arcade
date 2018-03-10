using UnityEngine;
using System.Collections;
//Questo script va attaccato al gameobject HandController
public class LeftRotationScript : MonoBehaviour {

	float xExtension = 0;
	float yExtension = 0;
	
	float[] xExtensions, yExtensions;
	int count = 0;
	
	static int numExtensions = 20;
	
	// Use this for initialization
	void Start () {
		xExtensions = new float[numExtensions];
		yExtensions = new float[numExtensions];
	}
	
	// Restituisce le estensioni verticali e orizzontali del polso, senza applicare il filtro di kalman,
	// ma facendo la media delle ultime numExtensions estensioni
	void Update () {
		Vector3 rot = gameObject.GetComponent<HandController> ().leftPalmRotation;
		float xAngle = rot.x;
		float yAngle = rot.y;
		float onScreenx = 0f;
		float onScreeny = 0f;
		
		if (xAngle > 0 && xAngle <= 180){
			onScreenx = Mathf.Round(xAngle*100f)/100f;
			
			if(count < numExtensions-1){
				xExtensions[count] = onScreenx;
			}
			else{
				ShiftArray(xExtensions);
				xExtensions[numExtensions-1] = onScreenx;
			}
			xExtension = onScreenx;
		}
		else if (xAngle > 180 && xAngle < 360){
			onScreenx = Mathf.Round((360 - xAngle)*100f)/100f;
			
			if(count < numExtensions-1){
				xExtensions[count] = -onScreenx;
			}
			else{
				ShiftArray(xExtensions);
				xExtensions[numExtensions-1] = -onScreenx;
			}

			xExtension = -onScreenx;
		}


//		if(xExtensions[numExtensions-1] != 0 && (xExtensions[numExtensions-1] != xExtensions[numExtensions/2]))
//			xExtension = MeanArray(xExtensions);
//		else
//			xExtension = 0f;
		
		
		
		if (yAngle > 0 && yAngle <= 180){
			onScreeny = Mathf.Round(yAngle*100f)/100f;
			
			if(count < numExtensions-1){
				yExtensions[count] = onScreeny;
			}
			else{
				ShiftArray(yExtensions);
				yExtensions[numExtensions-1] = onScreeny;
			}
			yExtension = onScreeny;
		}
		else if (yAngle > 180 && yAngle < 360){
			onScreeny = Mathf.Round((360 - yAngle)*100f)/100f;
			
			if(count < numExtensions-1){
				yExtensions[count] = -onScreeny;
			}
			else{
				ShiftArray(yExtensions);
				yExtensions[numExtensions-1] = -onScreeny;
			}
			yExtension = -onScreeny;
		}
		
//		if(yExtensions[numExtensions-1] != 0 && (yExtensions[numExtensions-1] != yExtensions[numExtensions/2]))
//			yExtension = MeanArray(yExtensions);
//		else
//			yExtension = 0f;
		
		//Debug.Log (xExtension);
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
	
	public float GetXExtension(){
		return xExtension;
	}
	
	public float GetYExtension(){
		return yExtension;
	}
	
	public Vector3 GetLeftRotation(){
		return gameObject.GetComponent<HandController> ().leftPalmRotation;
	}

}
