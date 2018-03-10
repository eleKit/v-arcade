using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text;

public class XMLGameSaveLoad : MonoBehaviour {

	// This is our local private members 
	Rect _Save, _Load, _SaveMSG, _LoadMSG;
	bool _ShouldSave, _ShouldLoad, _SwitchSave, _SwitchLoad;
	string _FileLocation, _FileName;
	
	Vector3 VPosition;
	
	List<XMLSaveStructure.GameItems> _GameItems;
	public GameObject[] bodies;
	List<List<SaveInfos.GameObjectInfos>> objects;
	string _data = string.Empty;
	string _documentsPath = string.Empty;
	
	void Awake()
	{
		_GameItems = new List<XMLSaveStructure.GameItems>();
	}
	
	// When the EGO is instansiated the Start will trigger 
	// so we setup our initial values for our local members 
	void Start()
	{
		// We setup our rectangles for our messages 
		_Save = new Rect(10, 80, 100, 20);
		_Load = new Rect(10, 100, 100, 20);
		_SaveMSG = new Rect(10, 120, 400, 40);
		_LoadMSG = new Rect(10, 140, 400, 40);
		
		// Where we want to save and load to and from 
		_documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
		_FileLocation = _documentsPath + "/Fisio SaveData";
		
		if(!System.IO.Directory.Exists(_FileLocation))
			System.IO.Directory.CreateDirectory(_FileLocation);
		_FileName = "Test.xml";
	}
	
	void Update() { }
	
	bool isSaving = false;
	bool isLoading = false;
	
	/* The following metods came from the referenced URL */
	string UTF8ByteArrayToString(byte[] characters)
	{
		UTF8Encoding encoding = new UTF8Encoding();
		string constructedString = encoding.GetString(characters);
		return (constructedString);
	}
	
	byte[] StringToUTF8ByteArray(string pXmlString)
	{
		UTF8Encoding encoding = new UTF8Encoding();
		byte[] byteArray = encoding.GetBytes(pXmlString);
		return byteArray;
	}
	
	// Here we serialize our UserData object of myData 
	string SerializeObject(object pObject)
	{
		string XmlizedString = null;
		MemoryStream memoryStream = new MemoryStream();
		XmlSerializer xs = new XmlSerializer(typeof(List<XMLSaveStructure.GameItems>));
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
		xs.Serialize(xmlTextWriter, pObject);
		memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
		XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
		return XmlizedString;
	}
	
	// Here we deserialize it back into its original form 
	object DeserializeObject(string pXmlizedString)
	{
		XmlSerializer xs = new XmlSerializer(typeof(List<XMLSaveStructure.GameItems>));
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
		return xs.Deserialize(memoryStream);
	}
	
	// Finally our save and load methods for the file itself 
	void CreateXML()
	{
		StreamWriter writer;
		//FileInfo t = new FileInfo(_FileLocation + "/" + _FileName);
		FileInfo t = new FileInfo(_FileLocation);
		if (!t.Exists)
		{
			writer = t.CreateText();
		}
		else
		{
			//t.Delete();
			writer = t.CreateText();
		}
		writer.Write(_data);
		writer.Close();
		Debug.Log("File written.");
	}
	
	void LoadXML()
	{
		if (File.Exists(_FileLocation + "/" + _FileName))
		{
			StreamReader r = File.OpenText(_FileLocation + "/" + _FileName);
			string _info = r.ReadToEnd();
			r.Close();
			if(_data.ToString() != "")
			{
				// notice how I use a reference to type (UserData) here, you need this
				// so that the returned object is converted into the correct type
				_GameItems = (List<XMLSaveStructure.GameItems>)DeserializeObject(_info);
				for(int i = 0; i < _GameItems.Count; i++)
				{
					VPosition = new Vector3(_GameItems[i].posx, _GameItems[i].posy, _GameItems[i].posz);
					bodies[i].transform.position=VPosition;
				}
				Debug.Log("File Read with item count: " + _GameItems.Count);
			}
		}
		else
		{
			Debug.Log("Files does not exist: " + _FileLocation + "/" + _FileName);
		}
	}

	public void Save(){
		if (!isSaving)
		{
			try
			{
				isSaving = true;
				GUI.Label(_SaveMSG, "Saving to: " + _FileLocation);
				SaveInfos.GameObjectInfos[] goi;
				objects = SaveInfos.gameObjects;
				_GameItems = new List<XMLSaveStructure.GameItems>();
				XMLSaveStructure.GameItems itm;
				for(int i = 0; i < objects.Count; i++){
					goi = objects[i].ToArray();
					for(int j = 0; j < goi.Length; j++){
						itm = new XMLSaveStructure.GameItems();
						itm.ID = goi[j].gID;
						itm.Name = goi[j].gName;
						itm.posx = goi[j].posX;
						itm.posy = goi[j].posY;
						itm.posz = goi[j].posZ;
						itm.angx = goi[j].angX;
						itm.angy = goi[j].angY;
						itm.angz = goi[j].angZ;
						itm.frame = (int) Mathf.Floor (i/2);
						_GameItems.Add(itm);
					}
				}
				
				// Time to creat our XML! 
				_data = SerializeObject(_GameItems);
				
				CreateXML();
				Debug.Log("Data Saved");
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
			}
			finally
			{
				isSaving = false;
			}
		}
	}
	//Per il momento non viene usato, poiché non devo caricare nessun file XML
	public void Load(){
		if (!isLoading)
		{
			GetComponent<LoadData> ().LoadXML ();
			try
			{
				isLoading = true;
				GUI.Label(_LoadMSG, "Loading from: " + _FileLocation);
				LoadXML();
				Debug.Log("Data loaded");
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
			}
			finally
			{
				isLoading = false;
			}
			
		}
	}

	public void SetFileLocation(string name){
		_FileLocation = name;
		//GetComponent<LoadData> ().SetDoc (_FileLocation);
	}
	
	public string GetFilePath(){
		return _FileLocation;
	}
}
