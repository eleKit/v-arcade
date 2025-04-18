﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TreeGenerator : MonoBehaviour {

	int treeCount;
	int flagCount;
	int treeIndex;
	int flagIndex;
	List<GameObject> trees = new List<GameObject>();
	List<GameObject> flags = new List<GameObject>();
	string flagPrefabPath = "Prefabs/Ski/Flags";
	public GameObject treeParent, flagParent;

	// Use this for initialization
	void Start () {
		flagParent.SetActive (false);
		treeParent.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void EmptyPieceOfTrack(){
		treeParent.SetActive (false);
		flagParent.SetActive (false);
	}

	public void CreatePieceOfTrack(bool step){
		if(step){
			flagParent.SetActive(false);
			bool twoTreesOnRow = (UnityEngine.Random.value > 0.5f); ; ; ; ;
			treeCount = treeParent.transform.childCount;
			for(int i = 0; i < treeCount; i++){
				trees.Add (treeParent.transform.GetChild(i).gameObject);
				treeParent.transform.GetChild(i).gameObject.SetActive(false);
			}
			treeIndex = (int) Mathf.Round(UnityEngine.Random.Range(0,treeCount));
			trees [treeIndex].SetActive (true);
			if(twoTreesOnRow){
				trees.RemoveAt(treeIndex);
				int newIndex = treeIndex = (int) Mathf.Round(UnityEngine.Random.Range(0,treeCount-1));
				trees [newIndex].SetActive (true);
			}
		}
		else{
			treeParent.SetActive(false);
			flagCount = flagParent.transform.childCount;
			for(int i = 0; i < flagCount; i++){
				flags.Add (flagParent.transform.GetChild(i).gameObject);
				flagParent.transform.GetChild(i).gameObject.SetActive(false);
			}
			flagIndex = (int) Mathf.Round(UnityEngine.Random.Range(0,flagCount));
			flags [flagIndex].SetActive (true);
		}
	}

	public void CreateFlags(float pos){
		flagParent.SetActive (false);
		treeParent.SetActive (false);
		GameObject go = (GameObject)Instantiate(Resources.Load(flagPrefabPath), transform.position, Quaternion.identity);
		go.transform.parent = gameObject.transform;
		go.transform.localPosition = new Vector3 (pos,0f,0f);
	}
}
