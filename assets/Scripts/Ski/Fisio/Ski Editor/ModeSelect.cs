using UnityEngine;
using System.Collections;

public class ModeSelect : MonoBehaviour {

	static string center_texture = "Textures/Ski/curva";
	static string left_texture = "Textures/Ski/curva sinistra";
	static string right_texture = "Textures/Ski/curva destra";
	static string empty_texture = "Textures/Ski/vuoto";

	public GameObject controller;
	public int sector;

	int mode = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Increase(){
		mode = (mode + 1) % 4;
		switch (mode){
		case 0:
			renderer.material.mainTexture = (Texture)Resources.Load(empty_texture);
			controller.GetComponent<SkiEditorController>().SetMode(mode, sector);
			break;
		case 1:
			renderer.material.mainTexture = (Texture)Resources.Load(center_texture);
			controller.GetComponent<SkiEditorController>().SetMode(mode, sector);
			break;
		case 2:
			renderer.material.mainTexture = (Texture)Resources.Load(left_texture);
			controller.GetComponent<SkiEditorController>().SetMode(mode, sector);
			break;
		case 3:
			renderer.material.mainTexture = (Texture)Resources.Load(right_texture);
			controller.GetComponent<SkiEditorController>().SetMode(mode, sector);
			break;
		}
	}
	
	public void Decrease(){
		if(mode == 0)
			mode = 3;
		else
			mode = (mode - 1) % 4;
		switch (mode){
		case 0:
			renderer.material.mainTexture = (Texture)Resources.Load(empty_texture);
			controller.GetComponent<SkiEditorController>().SetMode(mode, sector);
			break;
		case 1:
			renderer.material.mainTexture = (Texture)Resources.Load(center_texture);
			controller.GetComponent<SkiEditorController>().SetMode(mode, sector);
			break;
		case 2:
			renderer.material.mainTexture = (Texture)Resources.Load(left_texture);
			controller.GetComponent<SkiEditorController>().SetMode(mode, sector);
			break;
		case 3:
			renderer.material.mainTexture = (Texture)Resources.Load(right_texture);
			controller.GetComponent<SkiEditorController>().SetMode(mode, sector);
			break;
		}
	}
}
