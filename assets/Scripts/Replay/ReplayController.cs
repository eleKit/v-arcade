using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ReplayController : MonoBehaviour {

	List<string> availableUsers = new List<string>();

	string leftHandPrefab = "Prefabs/Leap/V2/HandGraphics/LeftSkeletalHand";
	string rightHandPrefab = "Prefabs/Leap/V2/HandGraphics/RightSkeletalHand";

	
	public GameObject saveManager, handController;
	GameObject leftHandGo;
	GameObject rightHandGo;
	int frameCount = 0;
	string game;

	void Awake(){

	}

	// Use this for initialization
	void Start () {
		game = saveManager.GetComponent<ReplaySave> ().GetGame ();
		SaveInfos.replay = true;
		LoadAvailableUsers ();
	}
	
	// Update is called once per frame
	void Update () {
		game = saveManager.GetComponent<ReplaySave> ().GetGame ();
	}

	void LoadAvailableUsers(){
		string p = Application.persistentDataPath + "/" + game;
		DirectoryInfo di = new DirectoryInfo(@p);
		DirectoryInfo[] subdirs = di.GetDirectories();
		foreach(DirectoryInfo dir in subdirs){
			availableUsers.Add (dir.Name);
		}
	}

	public List<string> GetAvailableUsers(){
		return availableUsers;
	}

	public List<string> GetAvailablePaths(string user){
		List<string> paths = new List<string>();
		string path = Application.persistentDataPath + "/" + game + "/" + user;
		DirectoryInfo di = new DirectoryInfo(@path);
		DirectoryInfo[] subdirs = di.GetDirectories();
		foreach(DirectoryInfo dir in subdirs){
			paths.Add (dir.Name);
		}
		return paths;
	}

	public List<string> GetAvailableRuns(string user, string path, string mode){
		List<string> runs = new List<string>();
		string dirP = Application.persistentDataPath + "/" + game + "/" + user + "/" + path + "/" + mode;
		DirectoryInfo di = new DirectoryInfo(@dirP);
		DirectoryInfo[] subdirs = di.GetDirectories();
		foreach(DirectoryInfo dir in subdirs){
			runs.Add (dir.Name);
		}
		return runs;
	}

	public void LoadHands(string user, string path, string mode, string run){
		string dirP = Application.persistentDataPath + "/" + game + "/" + user + "/" + path + "/" + mode + "/" + run;
		string leftP = dirP + "/Left.dat";
		string rightP = dirP + "/Right.dat";
		string infoP = dirP + "/Infos.dat";
		Debug.Log (leftP);
		saveManager.GetComponent<ReplaySave> ().LoadHands (leftP, rightP);
		saveManager.GetComponent<ReplaySave> ().LoadGameInfos (infoP);
		if(SaveInfos.plane)
			SendMessage ("SetStartAngles", PlayerSaveData.playerData.GetStartAngles());
		Time.timeScale = 1f;
		Debug.Log (SaveInfos.rightHandObjects.Count);
		if(SaveInfos.music)
			InvokeRepeating ("Replay", 0f, 0.1f);
		else
			InvokeRepeating ("Replay", 0f, 0.2f);
	}

	public void Replay(){
		string leftName = "LeftSkeletalHand(Clone)";
		string rightName = "RightSkeletalHand(Clone)";


		List<SaveInfos.GameObjectInfos> leftFrame = SaveInfos.leftHandObjects [frameCount];
		List<SaveInfos.GameObjectInfos> rightFrame = SaveInfos.rightHandObjects [frameCount];
		SaveInfos.GameObjectInfos leftHand = new SaveInfos.GameObjectInfos();
		SaveInfos.GameObjectInfos rightHand = new SaveInfos.GameObjectInfos();
		if(leftFrame.Exists(x => x.gName.Equals(leftName)))
			leftHand = leftFrame.Find(x => x.gName.Equals(leftName));
		if(rightFrame.Exists(x => x.gName.Equals(rightName)))
			rightHand = rightFrame.Find(x => x.gName.Equals(rightName));
		Vector3 startLeftPos;
		Vector3 startLeftAngles;
		Vector3 startRightPos;
		Vector3 startRightAngles;


		if(leftHand != null){
			startLeftPos = new Vector3(leftHand.posX, leftHand.posY, leftHand.posZ);
			startLeftAngles = new Vector3(leftHand.angX, leftHand.angY, leftHand.angZ);
			if(leftHandGo == null)
				leftHandGo = (GameObject) Instantiate(Resources.Load(leftHandPrefab), startLeftPos, Quaternion.Euler(startLeftAngles));
			foreach (Transform child in leftHandGo.transform)
			{
				if(leftFrame.Exists(x => x.gName.Equals(child.gameObject.name))){
					float posx = leftFrame.Find(x => x.gName.Equals(child.gameObject.name)).posX;
					float posy = leftFrame.Find(x => x.gName.Equals(child.gameObject.name)).posY;
					float posz = leftFrame.Find(x => x.gName.Equals(child.gameObject.name)).posZ;
					float angx = leftFrame.Find(x => x.gName.Equals(child.gameObject.name)).angX;
					float angy = leftFrame.Find(x => x.gName.Equals(child.gameObject.name)).angY;
					float angz = leftFrame.Find(x => x.gName.Equals(child.gameObject.name)).angZ;
					Vector3 pos = new Vector3(posx, posy, posz);
					Vector3 angs = new Vector3(angx, angy, angz);
					child.position = pos;
					child.eulerAngles = angs;
				}
				foreach(Transform ch in child){
					if(leftFrame.Exists(x => x.gName.Equals(ch.gameObject.name))){
						float posx = leftFrame.Find(x => x.gName.Equals(ch.gameObject.name)).posX;
						float posy = leftFrame.Find(x => x.gName.Equals(ch.gameObject.name)).posY;
						float posz = leftFrame.Find(x => x.gName.Equals(ch.gameObject.name)).posZ;
						float angx = leftFrame.Find(x => x.gName.Equals(ch.gameObject.name)).angX;
						float angy = leftFrame.Find(x => x.gName.Equals(ch.gameObject.name)).angY;
						float angz = leftFrame.Find(x => x.gName.Equals(ch.gameObject.name)).angZ;
						Vector3 pos = new Vector3(posx, posy, posz);
						Vector3 angs = new Vector3(angx, angy, angz);
						ch.position = pos;
						ch.eulerAngles = angs;
					}
				}
			}
			handController.GetComponent<MyHandController>().SetLeftPalmRotation(leftHandGo.transform.Find("lpalm").eulerAngles);
			handController.GetComponent<MyHandController>().SetLeftHandVisible(true);
		}
		if(rightHand != null){
			startRightPos = new Vector3(rightHand.posX, rightHand.posY, rightHand.posZ);
			startRightAngles = new Vector3(rightHand.angX, rightHand.angY, rightHand.angZ);
			if(rightHandGo == null)
				rightHandGo = (GameObject) Instantiate(Resources.Load(rightHandPrefab), startRightPos, Quaternion.Euler(startRightAngles));
			foreach (Transform child in rightHandGo.transform)
			{
				if(rightFrame.Exists(x => x.gName.Equals(child.gameObject.name))){
					float posx = rightFrame.Find(x => x.gName.Equals(child.gameObject.name)).posX;
					float posy = rightFrame.Find(x => x.gName.Equals(child.gameObject.name)).posY;
					float posz = rightFrame.Find(x => x.gName.Equals(child.gameObject.name)).posZ;
					float angx = rightFrame.Find(x => x.gName.Equals(child.gameObject.name)).angX;
					float angy = rightFrame.Find(x => x.gName.Equals(child.gameObject.name)).angY;
					float angz = rightFrame.Find(x => x.gName.Equals(child.gameObject.name)).angZ;
					Vector3 pos = new Vector3(posx, posy, posz);
					Vector3 angs = new Vector3(angx, angy, angz);
					child.position = pos;
					child.eulerAngles = angs;
				}
				foreach(Transform ch in child){
					if(rightFrame.Exists(x => x.gName.Equals(ch.gameObject.name))){
						float posx = rightFrame.Find(x => x.gName.Equals(ch.gameObject.name)).posX;
						float posy = rightFrame.Find(x => x.gName.Equals(ch.gameObject.name)).posY;
						float posz = rightFrame.Find(x => x.gName.Equals(ch.gameObject.name)).posZ;
						float angx = rightFrame.Find(x => x.gName.Equals(ch.gameObject.name)).angX;
						float angy = rightFrame.Find(x => x.gName.Equals(ch.gameObject.name)).angY;
						float angz = rightFrame.Find(x => x.gName.Equals(ch.gameObject.name)).angZ;
						Vector3 pos = new Vector3(posx, posy, posz);
						Vector3 angs = new Vector3(angx, angy, angz);
						ch.position = pos;
						ch.eulerAngles = angs;
					}	
				}	
			}
			handController.GetComponent<MyHandController>().SetRightPalmRotation(rightHandGo.transform.Find("rpalm").eulerAngles);
			handController.GetComponent<MyHandController>().SetRightHandVisible(true);
		}
		frameCount++;
		if(SaveInfos.rightHandObjects.Count <= frameCount)
			CancelInvoke();
	}
	
}
