using UnityEngine;
using System.Collections;
using System.Xml.Linq;

public class LoadData : MonoBehaviour {

	string xmlPath = string.Empty;
	XDocument doc;
	ArrayList names,xpos,ypos,zpos,xangles,yangles,zangles;
	string targetPrefabPath = "Prefabs/Plane/concretePipe";

	// Use this for initialization
	void Start () {
		names = new ArrayList ();
		xpos = new ArrayList ();
		ypos = new ArrayList ();
		zpos = new ArrayList ();
		xangles = new ArrayList ();
		yangles = new ArrayList ();
		zangles = new ArrayList ();
	}
	
	// Update is called once per frame
	void Update () {
		if(xmlPath != ""){
		//	doc = XDocument.Load(xmlPath);
		}

	}

	public void SetXmlPath(string path){
		xmlPath = path;
	}

	public void SetDoc (string path){
		xmlPath = path;
		//doc = XDocument.Load(xmlPath);
	}
	//Per il momento non viene usato, poiché non devo caricare nessun file XML
	public void LoadXML(){
		var nameList = doc.Descendants( "Name" );
		var posXList = doc.Descendants( "posx" );
		var posYList = doc.Descendants( "posy" );
		var posZList = doc.Descendants( "posz" );
		var angleXList = doc.Descendants( "anglex" );
		var angleYList = doc.Descendants( "angley" );
		var angleZList = doc.Descendants( "anglez" );

		foreach ( var name in nameList )
		{
			string temp = name.Value;
			names.Add(temp);
		}

		foreach ( var posx in posXList )
		{
			float temp = float.Parse(posx.Value);
			xpos.Add(temp);
		}

		foreach ( var posy in posYList )
		{
			float temp = float.Parse(posy.Value);
			ypos.Add(temp);
		}

		foreach ( var posz in posZList )
		{
			float temp = float.Parse(posz.Value);
			zpos.Add(temp);
		}

		foreach ( var anglex in angleXList )
		{
			float temp = float.Parse(anglex.Value);
			xangles.Add(temp);
		}

		foreach ( var angley in angleYList )
		{
			float temp = float.Parse(angley.Value);
			yangles.Add(temp);
		}

		foreach ( var anglez in angleZList )
		{
			float temp = float.Parse(anglez.Value);
			zangles.Add(temp);
		}

		foreach(string name in names){
			int index = names.IndexOf(name);
			Vector3 pos = new Vector3((float) xpos[index],(float) ypos[index],(float) zpos[index]);
			float xangle = (float) xangles[index];
			float yangle = (float) yangles[index];
			float zangle = (float) zangles[index];
			GameObject go = (GameObject)Instantiate(Resources.Load(targetPrefabPath), pos, Quaternion.Euler(xangle, yangle, zangle));
			go.name = name;
		}
	}
}
