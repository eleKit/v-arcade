using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Leap;

[Serializable]
public class FrameSequence : ISerializationCallbackReceiver
{
	// byte[] is not Serializable
	private List<byte[]> frames = new List<byte[]> ();

	// We convert bytes[] to a base64 representation to serialize it
	public List<string> serializableFrames;

	// UNIX timestamp
	public long timestamp;

	public string patientName;

	public void addFrame (Frame f)
	{
		frames.Add (f.Serialize);
	}

	public List<Frame> GetFrames ()
	{

		List<Frame> record = new List<Frame> ();

		foreach (byte[] frame in frames) {

			Frame reconstructedFrame = new Frame ();
			reconstructedFrame.Deserialize (frame);
			record.Add (reconstructedFrame);
		}

		return record;
	}

	public void OnBeforeSerialize ()
	{
		serializableFrames = new List<string> ();

		foreach (byte[] frame in frames) {
			string encodedText = Convert.ToBase64String (frame);
			serializableFrames.Add (encodedText);
		}
	}

	public void OnAfterDeserialize ()
	{
		//Unity has just written new data into the serializableFrames field.
		frames = new List<byte[]> ();

		foreach (string serFrame in serializableFrames) {
			frames.Add (Convert.FromBase64String (serFrame));
		}
	}

}
