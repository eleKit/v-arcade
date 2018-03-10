using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable()]
public class XMLSaveStructure : ISerializable{

	[Serializable()]
	public class GameItems : ISerializable
	{
		public string ID;
		public string Name;
		public int frame;
		public float posx;
		public float posy;
		public float posz;
		public float angx;
		public float angy;
		public float angz;
		
		public GameItems()
		{
			ID = string.Empty;
			Name = string.Empty;
			posx = 0;
			posy = 0;
			posz = 0;
			angx = 0;
			angy = 0;
			angz = 0;
			frame = 0;
		}
		
		// Deserialization
		public GameItems(SerializationInfo info, StreamingContext ctxt)
		{
			ID = (String)info.GetValue("ID", typeof(string));
			Name = (String)info.GetValue("Name", typeof(string));
			posx = (float)info.GetValue("posx", typeof(float));
			posy = (float)info.GetValue("posy", typeof(float));
			posz = (float)info.GetValue("posz", typeof(float));
			angx = (float)info.GetValue("angx", typeof(float));
			angy = (float)info.GetValue("angy", typeof(float));
			angz = (float)info.GetValue("angz", typeof(float));
			frame = (int)info.GetValue("frame", typeof(int));
		}
		
		// Serialization
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("ID", ID);
			info.AddValue("Name", Name);
			info.AddValue("posx", posx);
			info.AddValue("posy", posy);
			info.AddValue("posz", posz);
			info.AddValue("angx", angx);
			info.AddValue("angy", angy);
			info.AddValue("angz", angz);
			info.AddValue("frame", frame);
		}
	}
	
	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
	}
}
