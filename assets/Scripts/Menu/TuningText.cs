using UnityEngine;
using System.Collections;

public class TuningText : MonoBehaviour {

	public GameObject tuningController;

	void OnMouseDown(){
		if(gameObject.name.Equals("Save Text")){
			tuningController.GetComponent<TuningController>().SaveStats();
		}

		if(gameObject.name.Equals("Update Text")){
			tuningController.GetComponent<TuningController>().Redo();
		}
		if(gameObject.name.Equals("Menu Text")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}

}
