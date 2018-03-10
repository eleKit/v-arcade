using UnityEngine;
using System.Collections;

public class ChangeValue : MonoBehaviour {

	public GameObject controller, value;
	public int sector;

	int val;
	bool diff;

	// Use this for initialization
	void Start () {
		val = int.Parse (value.GetComponent<TextMesh> ().text);
		if(gameObject.name.Equals("Difficoltà"))
			diff =true;
		else
			diff = false;
	}


	public void Increase(){
		if(diff){
			val = (val + 1) % 4;
			if(val== 0)
				val = 1;
			value.GetComponent<TextMesh> ().text = val.ToString();
			controller.GetComponent<SkiEditorController>().SetDifficulty(val, sector);
		}
		else{
			val += 5;
			value.GetComponent<TextMesh> ().text = val.ToString();
			controller.GetComponent<SkiEditorController>().IncreaseObstacles(val, sector);
		}
	}

	public void Decrease(){
		if(diff){
			if(val == 1)
				val = 3;
			else
				val = (val - 1) % 4;
			value.GetComponent<TextMesh> ().text = val.ToString();
			controller.GetComponent<SkiEditorController>().SetDifficulty(val, sector);
		}
		else{
			if(val > 10){
				val -= 5;
				value.GetComponent<TextMesh> ().text = val.ToString();
				controller.GetComponent<SkiEditorController>().DecreaseObstacles(val, sector);
			}
		}
	}
}
