﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;

public static class ES3
{
	public enum Location 		{ File, PlayerPrefs, Memory, Resources };
	public enum Directory		{ PersistentDataPath, DataPath }
	public enum EncryptionType 	{ None, AES };
	public enum Format 			{ JSON };
	public enum ReferenceMode	{ ByRef, ByValue, ByRefAndValue};

	public static string backupFileSuffix = ".bac";
	public static string temporaryFileSuffix = ".tmp";

	#region ES3.Save

	/// <summary>Saves the value to the default file with the given key.</summary>
	/// <param name="key">The key we want to use to identify our value in the file.</param>
	/// <param name="value">The value we want to save.</param>
	public static void Save<T>(string key, object value)
	{
		using(var writer = ES3Writer.Create(new ES3Settings()))
		{
			writer.Write<T>(key, value);
			writer.Save();
		}
	}

	/// <summary>Saves the value to a file with the given key.</summary>
	/// <param name="key">The key we want to use to identify our value in the file.</param>
	/// <param name="value">The value we want to save.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void Save<T>(string key, object value, ES3Settings settings)
	{
		using(var writer = ES3Writer.Create(settings))
		{
			writer.Write<T>(key, value);
			writer.Save();
		}
	}

	/// <summary>Saves the value to a file with the given key.</summary>
	/// <param name="key">The key we want to use to identify our value in the file.</param>
	/// <param name="value">The value we want to save.</param>
	/// <param name="filepath">The relative or absolute path of the file we want to store our value to.</param>
	public static void Save<T>(string key, object value, string filePath)
	{
		Save<T>(key, value, new ES3Settings(filePath));
	}

	/// <summary>Saves the value to a file with the given key.</summary>
	/// <param name="key">The key we want to use to identify our value in the file.</param>
	/// <param name="value">The value we want to save.</param>
	/// <param name="filepath">The relative or absolute path of the file we want to store our value to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void Save<T>(string key, object value, string filePath, ES3Settings settings)
	{
		Save<T>(key, value, new ES3Settings(filePath, settings));
	}


	/// <summary>Creates or overwrites a file with the specified raw bytes.</summary>
	/// <param name="bytes">The bytes we want to store.</param>
	public static void SaveRaw(byte[] bytes)
	{
		SaveRaw(bytes, new ES3Settings());
	}

	/// <summary>Creates or overwrites a file with the specified raw bytes.</summary>
	/// <param name="bytes">The bytes we want to store.</param>
	/// <param name="filepath">The relative or absolute path of the file we want to store our bytes to.</param>
	public static void SaveRaw(byte[] bytes, string filePath)
	{
		SaveRaw(bytes, new ES3Settings(filePath));
	}

	/// <summary>Creates or overwrites a file with the specified raw bytes.</summary>
	/// <param name="bytes">The bytes we want to store.</param>
	/// <param name="filepath">The relative or absolute path of the file we want to store our bytes to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void SaveRaw(byte[] bytes, string filePath, ES3Settings settings)
	{
		SaveRaw(bytes, new ES3Settings(filePath, settings));
	}

	/// <summary>Creates or overwrites a file with the specified raw bytes.</summary>
	/// <param name="bytes">The bytes we want to store.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void SaveRaw(byte[] bytes, ES3Settings settings)
	{
		using (var stream = ES3Stream.CreateStream(settings, ES3FileMode.Write))
		{
			stream.Write(bytes, 0, bytes.Length);
		}
		ES3IO.CommitBackup(settings);
	}

	/// <summary>Creates or overwrites the default file with the specified raw bytes.</summary>
	/// <param name="str">The string we want to store.</param>
	public static void SaveRaw(string str)
	{
		SaveRaw(str, new ES3Settings());
	}

	/// <summary>Creates or overwrites the default file with the specified raw bytes.</summary>
	/// <param name="str">The string we want to store.</param>
	/// <param name="filepath">The relative or absolute path of the file we want to store our bytes to.</param>
	public static void SaveRaw(string str, string filePath)
	{
		SaveRaw(str, new ES3Settings(filePath));
	}

	/// <summary>Creates or overwrites a file with the specified raw bytes.</summary>
	/// <param name="str">The string we want to store.</param>
	/// <param name="filepath">The relative or absolute path of the file we want to store our bytes to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void SaveRaw(string str, string filePath, ES3Settings settings)
	{
		SaveRaw(str, new ES3Settings(filePath, settings));
	}

	/// <summary>Creates or overwrites a file with the specified raw bytes.</summary>
	/// <param name="str">The string we want to store.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void SaveRaw(string str, ES3Settings settings)
	{
		var bytes = settings.encoding.GetBytes(str);
		SaveRaw(bytes, settings);
	}

	/// <summary>Creates or appends the specified bytes to a file.</summary>
	/// <param name="bytes">The bytes we want to append.</param>
	public static void AppendRaw(byte[] bytes)
	{
		AppendRaw(bytes, new ES3Settings());
	}

	/// <summary>Creates or appends the specified bytes to a file.</summary>
	/// <param name="bytes">The bytes we want to append.</param>
	/// <param name="filepath">The relative or absolute path of the file we want to append our bytes to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void AppendRaw(byte[] bytes, string filePath, ES3Settings settings)
	{
		AppendRaw(bytes, new ES3Settings(filePath, settings));
	}

	/// <summary>Creates or appends the specified bytes to a file.</summary>
	/// <param name="bytes">The bytes we want to append.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void AppendRaw(byte[] bytes, ES3Settings settings)
	{
		ES3Settings newSettings = new ES3Settings(settings.path, settings);
		newSettings.encryptionType = EncryptionType.None;

		using(var stream = ES3Stream.CreateStream(newSettings, ES3FileMode.Append))
			stream.Write(bytes, 0, bytes.Length);
	}

	/// <summary>Creates or appends the specified bytes to the default file.</summary>
	/// <param name="bytes">The bytes we want to append.</param>
	public static void AppendRaw(string str)
	{
		AppendRaw(str, new ES3Settings());
	}

	/// <summary>Creates or appends the specified bytes to a file.</summary>
	/// <param name="bytes">The bytes we want to append.</param>
	/// <param name="filepath">The relative or absolute path of the file we want to append our bytes to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void AppendRaw(string str, string filePath, ES3Settings settings)
	{
		AppendRaw(str, new ES3Settings(filePath, settings));
	}

	/// <summary>Creates or appends the specified bytes to a file.</summary>
	/// <param name="bytes">The bytes we want to append.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void AppendRaw(string str, ES3Settings settings)
	{
		var bytes = settings.encoding.GetBytes(str);
		ES3Settings newSettings = new ES3Settings(settings.path, settings);
		newSettings.encryptionType = EncryptionType.None;

		using(var stream = ES3Stream.CreateStream(newSettings, ES3FileMode.Append))
			stream.Write(bytes, 0, bytes.Length);
	}

	/// <summary>Saves a Texture2D as a PNG or JPG, depending on the file extension used for the filePath.</summary>
	/// <param name="texture">The Texture2D we want to save as a JPG or PNG.</param>
	/// <param name="filePath">The relative or absolute path of the PNG or JPG file we want to create.</param>
	public static void SaveImage(Texture2D texture, string imagePath)
	{
		SaveImage(texture, new ES3Settings(imagePath));
	}

	/// <summary>Saves a Texture2D as a PNG or JPG, depending on the file extension used for the filePath.</summary>
	/// <param name="texture">The Texture2D we want to save as a JPG or PNG.</param>
	/// <param name="filePath">The relative or absolute path of the PNG or JPG file we want to create.</param>
	public static void SaveImage(Texture2D texture, string imagePath, ES3Settings settings)
	{
		SaveImage(texture, new ES3Settings(imagePath, settings));
	}

	/// <summary>Saves a Texture2D as a PNG or JPG, depending on the file extension used for the filePath.</summary>
	/// <param name="texture">The Texture2D we want to save as a JPG or PNG.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void SaveImage(Texture2D texture, ES3Settings settings)
	{
		// Get the file extension to determine what format we want to save the image as.
		string extension = ES3IO.GetExtension(settings.path).ToLower();
		if(string.IsNullOrEmpty(extension))
			throw new System.ArgumentException("File path must have a file extension when using ES3.SaveImage.");
		byte[] bytes;
		if(extension == ".jpg" || extension == ".jpeg")
			bytes = texture.EncodeToJPG();
		else if(extension == ".png")
			bytes = texture.EncodeToPNG();
		else
			throw new System.ArgumentException("File path must have extension of .png, .jpg or .jpeg when using ES3.SaveImage.");

		ES3.SaveRaw(bytes, settings);
	}

	#endregion

	#region ES3.Load<T>

	/* Standard load methods */

	/// <summary>Loads the value from a file with the given key.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	public static T Load<T>(string key)
	{
		return Load<T>(key, new ES3Settings());
	}

	/// <summary>Loads the value from a file with the given key.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
	public static T Load<T>(string key, string filePath)
	{
		return Load<T>(key, new ES3Settings(filePath));
	}

	/// <summary>Loads the value from a file with the given key.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static T Load<T>(string key, string filePath, ES3Settings settings)
	{
		return Load<T>(key, new ES3Settings(filePath, settings));
	}

	/// <summary>Loads the value from a file with the given key.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static T Load<T>(string key, ES3Settings settings)
	{
		using(var reader = ES3Reader.Create(settings))
		{
			if(reader == null)
				throw new System.IO.FileNotFoundException("File \""+settings.FullPath + "\" could not be found.");
			return reader.Read<T>(key);
		}
	}

	/// <summary>Loads the value from a file with the given key.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
	public static T Load<T>(string key, T defaultValue)
	{
		return Load<T>(key, defaultValue, new ES3Settings());
	}

	/// <summary>Loads the value from a file with the given key.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
	/// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
	public static T Load<T>(string key, string filePath, T defaultValue)
	{
		return Load<T>(key, defaultValue, new ES3Settings(filePath));
	}

	/// <summary>Loads the value from a file with the given key.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
	/// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static T Load<T>(string key, string filePath, T defaultValue, ES3Settings settings)
	{
		return Load<T>(key, defaultValue, new ES3Settings(filePath, settings));
	}

	/// <summary>Loads the value from a file with the given key.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static T Load<T>(string key, T defaultValue, ES3Settings settings)
	{
		using(var reader = ES3Reader.Create(settings))
		{
			if(reader == null)
				return defaultValue;
			return reader.Read<T>(key, defaultValue);
		}
	}

	/* Self-assigning load methods */

	/// <summary>Loads the value from a file with the given key into an existing object, rather than creating a new instance.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="obj">The object we want to load the value into.</param>
	public static void LoadInto<T>(string key, T obj) where T : class
	{
		LoadInto<T>(key, obj, new ES3Settings());
	}

	/// <summary>Loads the value from a file with the given key into an existing object, rather than creating a new instance.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
	/// <param name="obj">The object we want to load the value into.</param>
	public static void LoadInto<T>(string key, string filePath, T obj) where T : class
	{
		LoadInto<T>(key, obj, new ES3Settings(filePath));
	}

	/// <summary>Loads the value from a file with the given key into an existing object, rather than creating a new instance.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
	/// <param name="obj">The object we want to load the value into.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void LoadInto<T>(string key, string filePath, T obj, ES3Settings settings)  where T : class
	{
		LoadInto<T>(key, obj, new ES3Settings(filePath, settings));
	}

	/// <summary>Loads the value from a file with the given key into an existing object, rather than creating a new instance.</summary>
	/// <param name="key">The key which identifies the value we want to load.</param>
	/// <param name="obj">The object we want to load the value into.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void LoadInto<T>(string key, T obj, ES3Settings settings) where T : class
	{
		if(settings == null) settings = new ES3Settings();
		using(var reader = ES3Reader.Create(settings))
		{
			if(reader == null)
				throw new System.IO.FileNotFoundException("File \""+settings.FullPath + "\" could not be found.");
			reader.ReadInto<T>(key, obj);
		}
	}

	#endregion

	#region Other ES3.Load Methods

	/// <summary>Loads the default file as a byte array.</summary>
	public static byte[] LoadRawBytes()
	{
		return LoadRawBytes(new ES3Settings());
	}

	/// <summary>Loads a file as a byte array.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to load as a byte array.</param>
	public static byte[] LoadRawBytes(string filePath)
	{
		return LoadRawBytes(new ES3Settings(filePath));
	}

	/// <summary>Loads a file as a byte array.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to load as a byte array.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static byte[] LoadRawBytes(string filePath, ES3Settings settings)
	{
		return LoadRawBytes(new ES3Settings(filePath, settings));
	}

	/// <summary>Loads the default file as a byte array.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static byte[] LoadRawBytes(ES3Settings settings)
	{
		using (var stream = ES3Stream.CreateStream(settings, ES3FileMode.Read))
		{
			var bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			return bytes;
		}

		/*if(settings.location == Location.File)
			return ES3IO.ReadAllBytes(settings.FullPath);
		else if(settings.location == Location.PlayerPrefs)
			return System.Convert.FromBase64String(PlayerPrefs.GetString(settings.FullPath));
		else if(settings.location == Location.Resources)
		{
			var textAsset = Resources.Load<TextAsset>(settings.FullPath);
			return textAsset.bytes;
		}
		return null;*/
	}

	/// <summary>Loads the default file as a byte array.</summary>
	public static string LoadRawString()
	{
		return LoadRawString(new ES3Settings());
	}

	/// <summary>Loads a file as a byte array.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to load as a byte array.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static string LoadRawString(string filePath)
	{
		return LoadRawString(new ES3Settings(filePath));
	}

	/// <summary>Loads a file as a byte array.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to load as a byte array.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static string LoadRawString(string filePath, ES3Settings settings)
	{
		return LoadRawString(new ES3Settings(filePath, settings));
	}

	/// <summary>Loads the default file as a byte array.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static string LoadRawString(ES3Settings settings)
	{
		var bytes = ES3.LoadRawBytes(settings);
		return settings.encoding.GetString(bytes, 0, bytes.Length);
	}

	/// <summary>Loads a PNG or JPG as a Texture2D.</summary>
	/// <param name="imagePath">The relative or absolute path of the PNG or JPG file we want to load as a Texture2D.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static Texture2D LoadImage(string imagePath)
	{
		return LoadImage(new ES3Settings(imagePath));
	}

	/// <summary>Loads a PNG or JPG as a Texture2D.</summary>
	/// <param name="imagePath">The relative or absolute path of the PNG or JPG file we want to load as a Texture2D.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static Texture2D LoadImage(string imagePath, ES3Settings settings)
	{
		return LoadImage(new ES3Settings(imagePath, settings));
	}

	/// <summary>Loads a PNG or JPG as a Texture2D.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static Texture2D LoadImage(ES3Settings settings)
	{
		byte[] bytes = ES3.LoadRawBytes(settings);
		return LoadImage(bytes);
	}

	/// <summary>Loads a PNG or JPG as a Texture2D.</summary>
	/// <param name="bytes">The raw bytes of the PNG or JPG.</param>
	public static Texture2D LoadImage(byte[] bytes)
	{
		var texture = new Texture2D(1,1);
		texture.LoadImage(bytes);
		return texture;
	}

	/// <summary>Loads an audio file as an AudioClip. Note that MP3 files are not supported on standalone platforms and Ogg Vorbis files are not supported on mobile platforms.</summary>
	/// <param name="imagePath">The relative or absolute path of the audio file we want to load as an AudioClip.</param>
	public static AudioClip LoadAudio(string audioFilePath)
	{
		return LoadAudio(audioFilePath, new ES3Settings());
	}

	/// <summary>Loads an audio file as an AudioClip. Note that MP3 files are not supported on standalone platforms and Ogg Vorbis files are not supported on mobile platforms.</summary>
	/// <param name="imagePath">The relative or absolute path of the audio file we want to load as an AudioClip.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static AudioClip LoadAudio(string audioFilePath, ES3Settings settings)
	{
		if(Application.platform == RuntimePlatform.WebGLPlayer)
			Debug.LogError("You cannot use ES3.LoadAudio with Unity Web Player");

		string extension = ES3IO.GetExtension(audioFilePath).ToLower();

		if(extension == ".mp3" && (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer))
			throw new System.InvalidOperationException("You can only load Ogg, WAV, XM, IT, MOD or S3M on Unity Standalone");

		if(extension == ".ogg" && (		Application.platform == RuntimePlatform.IPhonePlayer 
									|| 	Application.platform == RuntimePlatform.Android 
									|| 	Application.platform == RuntimePlatform.WSAPlayerARM))
			throw new System.InvalidOperationException("You can only load MP3, WAV, XM, IT, MOD or S3M on Unity Standalone");

		var newSettings = new ES3Settings(audioFilePath, settings);

		WWW www = new WWW("file://"+newSettings.FullPath);
		while(!www.isDone)
		{
			// Wait for it to load.
		}

		if(!string.IsNullOrEmpty(www.error))
			throw new System.Exception(www.error);

		#if UNITY_2017_3_OR_NEWER
		return www.GetAudioClip(true);
		#elif UNITY_5_6_OR_NEWER
		return WWWAudioExtensions.GetAudioClip(www);
		#else
		return www.audioClip;
		#endif
	}

	#endregion

	#region Other ES3 Methods

	/// <summary>Deletes the file at the given path using the default settings.</summary>
	/// <param name="filePath">The relative or absolute path of the file we wish to delete.</param>
	public static void DeleteFile(string filePath)
	{
		DeleteFile(new ES3Settings(filePath));
	}

	/// <summary>Deletes the file at the given path using the settings provided.</summary>
	/// <param name="filePath">The relative or absolute path of the file we wish to delete.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void DeleteFile(string filePath, ES3Settings settings)
	{
		DeleteFile(new ES3Settings(filePath, settings));
	}

	/// <summary>Deletes the file specified by the ES3Settings object provided as a parameter.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void DeleteFile(ES3Settings settings)
	{
		if(settings.location == Location.File)
			ES3IO.DeleteFile(settings.FullPath);
		else if(settings.location == Location.PlayerPrefs)
			PlayerPrefs.DeleteKey(settings.FullPath);
		else if(settings.location == Location.Resources)
			throw new System.NotSupportedException("Deleting files from Resources is not supported.");
	}

	/// <summary>Copies a file from one path to another.</summary>
	/// <param name="oldFilePath">The relative or absolute path of the file we want to copy.</param>
	/// <param name="newFilePath">The relative or absolute path of the copy we want to create.</param>
	public static void CopyFile(string oldFilePath, string newFilePath)
	{
		CopyFile(new ES3Settings(oldFilePath), new ES3Settings(newFilePath));
	}

	/// <summary>Copies a file from one location to another, using the ES3Settings provided to override any default settings.</summary>
	/// <param name="oldFilePath">The relative or absolute path of the file we want to copy.</param>
	/// <param name="newFilePath">The relative or absolute path of the copy we want to create.</param>
	/// <param name="oldSettings">The settings we want to use when copying the old file.</param>
	/// <param name="newSettings">The settings we want to use when creating the new file.</param>
	public static void CopyFile(string oldFilePath, string newFilePath, ES3Settings oldSettings, ES3Settings newSettings)
	{
		CopyFile(new ES3Settings(oldFilePath, oldSettings), new ES3Settings(newFilePath, newSettings));
	}

	/// <summary>Copies a file from one location to another, using the ES3Settings provided to determine the locations.</summary>
	/// <param name="oldSettings">The settings we want to use when copying the old file.</param>
	/// <param name="newSettings">The settings we want to use when creating the new file.</param>
	public static void CopyFile(ES3Settings oldSettings, ES3Settings newSettings)
	{
		if(oldSettings.location == Location.File)
		{
			if(ES3IO.FileExists(oldSettings.FullPath))
			{
				ES3IO.DeleteFile(newSettings.FullPath);
				ES3IO.CopyFile(oldSettings.FullPath, newSettings.FullPath);
			}
		}
		else if(oldSettings.location == Location.PlayerPrefs)
		{
			PlayerPrefs.SetString(newSettings.FullPath, PlayerPrefs.GetString(oldSettings.FullPath));
		}
		else if(oldSettings.location == Location.Resources)
			throw new System.NotSupportedException("Modifying files from Resources is not allowed.");
	}

	/// <summary>Renames a file.</summary>
	/// <param name="oldFilePath">The relative or absolute path of the file we want to rename.</param>
	/// <param name="newFilePath">The relative or absolute path we want to rename the file to.</param>
	public static void RenameFile(string oldFilePath, string newFilePath)
	{
		RenameFile(new ES3Settings(oldFilePath), new ES3Settings(newFilePath));
	}

	/// <summary>Renames a file.</summary>
	/// <param name="oldFilePath">The relative or absolute path of the file we want to rename.</param>
	/// <param name="newFilePath">The relative or absolute path we want to rename the file to.</param>
	/// <param name="oldSettings">The settings for the file we want to rename.</param>
	/// <param name="newSettings">The settings for the file we want our source file to be renamed to.</param>
	public static void RenameFile(string oldFilePath, string newFilePath, ES3Settings oldSettings, ES3Settings newSettings)
	{
		RenameFile(new ES3Settings(oldFilePath, oldSettings), new ES3Settings(newFilePath, newSettings));
	}

	/// <summary>Renames a file.</summary>
	/// <param name="oldSettings">The settings for the file we want to rename.</param>
	/// <param name="newSettings">The settings for the file we want our source file to be renamed to.</param>
	public static void RenameFile(ES3Settings oldSettings, ES3Settings newSettings)
	{
		if(oldSettings.location == Location.File)
		{
			if(ES3IO.FileExists(oldSettings.FullPath))
			{
				ES3IO.DeleteFile(newSettings.FullPath);
				ES3IO.MoveFile(oldSettings.FullPath, newSettings.FullPath);
			}
		}
		else if(oldSettings.location == Location.PlayerPrefs)
		{
			PlayerPrefs.SetString(newSettings.FullPath, PlayerPrefs.GetString(oldSettings.FullPath));
			PlayerPrefs.DeleteKey(oldSettings.FullPath);
		}
		else if(oldSettings.location == Location.Resources)
			throw new System.NotSupportedException("Modifying files from Resources is not allowed.");
	}

	/// <summary>Deletes the directory at the given path using the settings provided.</summary>
	/// <param name="directoryPath">The relative or absolute path of the folder we wish to delete.</param>
	public static void DeleteDirectory(string directoryPath)
	{
		DeleteDirectory(new ES3Settings(directoryPath));
	}

	/// <summary>Deletes the directory at the given path using the settings provided.</summary>
	/// <param name="directoryPath">The relative or absolute path of the folder we wish to delete.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void DeleteDirectory(string directoryPath, ES3Settings settings)
	{
		DeleteDirectory(new ES3Settings(directoryPath, settings));
	}

	/// <summary>Deletes the directory at the given path using the settings provided.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void DeleteDirectory(ES3Settings settings)
	{
		if(settings.location == Location.File)
			ES3IO.DeleteDirectory(settings.FullPath);
		else if(settings.location == Location.PlayerPrefs)
			throw new System.NotSupportedException("Deleting Directories using PlayerPrefs is not supported.");
		else if(settings.location == Location.Resources)
			throw new System.NotSupportedException("Deleting directories from Resources is not allowed.");
	}

	/// <summary>Deletes a key in the default file.</summary>
	/// <param name="key">The key we want to delete.</param>
	public static void DeleteKey(string key)
	{
		DeleteKey(key, new ES3Settings());
	}

	public static void DeleteKey(string key, string filePath)
	{
		DeleteKey(key, new ES3Settings(filePath));
	}

	/// <summary>Deletes a key in the file specified.</summary>
	/// <param name="key">The key we want to delete.</param>
	/// <param name="key">The relative or absolute path of the file we want to delete the key from.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void DeleteKey(string key, string filePath, ES3Settings settings)
	{
		DeleteKey(key, new ES3Settings(filePath, settings));
	}

	/// <summary>Deletes a key in the file specified by the ES3Settings object.</summary>
	/// <param name="key">The key we want to delete.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void DeleteKey(string key, ES3Settings settings)
	{
		if(settings.location == Location.Resources)
			throw new System.NotSupportedException("Modifying files in Resources is not allowed.");

		if(ES3.FileExists(settings))
		{	
			using (var writer = ES3Writer.Create(settings))
			{
				writer.MarkKeyForDeletion(key);
				writer.Save();
			}
		}
	}

	/// <summary>Checks whether a key exists in the default file.</summary>
	/// <param name="key">The key we want to check the existence of.</param>
	/// <returns>True if the key exists, otherwise False.</returns>
	public static bool KeyExists(string key)
	{
		return KeyExists(key, new ES3Settings());
	}

	/// <summary>Checks whether a key exists in the specified file.</summary>
	/// <param name="key">The key we want to check the existence of.</param>
	/// <param name="filePath">The relative or absolute path of the file we want to search.</param>
	/// <returns>True if the key exists, otherwise False.</returns>
	public static bool KeyExists(string key, string filePath)
	{
		return KeyExists(key, new ES3Settings(filePath));
	}

	/// <summary>Checks whether a key exists in the default file.</summary>
	/// <param name="key">The key we want to check the existence of.</param>
	/// <param name="filePath">The relative or absolute path of the file we want to search.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <returns>True if the key exists, otherwise False.</returns>
	public static bool KeyExists(string key, string filePath, ES3Settings settings)
	{
		return KeyExists(key, new ES3Settings(filePath, settings));
	}

	/// <summary>Checks whether a key exists in a file.</summary>
	/// <param name="key">The key we want to check the existence of.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <returns>True if the file exists, otherwise False.</returns>
	public static bool KeyExists(string key, ES3Settings settings)
	{
		using(var reader = ES3Reader.Create(settings))
		{
			if(reader == null)
				return false;
			return reader.Goto(key);
		}
	}

	/// <summary>Checks whether a file exists.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to check the existence of.</param>
	/// <returns>True if the file exists, otherwise False.</returns>
	public static bool FileExists(string filePath)
	{
		return FileExists(new ES3Settings(filePath));
	}

	/// <summary>Checks whether a file exists.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to check the existence of.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <returns>True if the file exists, otherwise False.</returns>
	public static bool FileExists(string filePath, ES3Settings settings)
	{
		return FileExists(new ES3Settings(filePath, settings));
	}

	/// <summary>Checks whether a file exists.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <returns>True if the file exists, otherwise False.</returns>
	public static bool FileExists(ES3Settings settings)
	{
		if(settings.location == Location.File)
			return ES3IO.FileExists(settings.FullPath);
		else if(settings.location == Location.PlayerPrefs)
			return PlayerPrefs.HasKey(settings.FullPath);
		else if(settings.location == Location.Resources)
			return Resources.Load(settings.FullPath) != null;
		return false;
	}

	/// <summary>Checks whether a file exists.</summary>
	/// <param name="folderPath">The relative or absolute path of the folder we want to check the existence of.</param>
	/// <returns>True if the folder exists, otherwise False.</returns>
	public static bool DirectoryExists(string folderPath)
	{
		return DirectoryExists(new ES3Settings(folderPath));
	}

	/// <summary>Checks whether a file exists.</summary>
	/// <param name="folderPath">The relative or absolute path of the folder we want to check the existence of.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <returns>True if the folder exists, otherwise False.</returns>

	public static bool DirectoryExists(string folderPath, ES3Settings settings)
	{
		return DirectoryExists(new ES3Settings(folderPath, settings));
	}

	/// <summary>Checks whether a folder exists.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <returns>True if the folder exists, otherwise False.</returns>
	public static bool DirectoryExists(ES3Settings settings)
	{
		if(settings.location == Location.File)
			return ES3IO.DirectoryExists(settings.FullPath);
		else if(settings.location == Location.PlayerPrefs)
			throw new System.NotSupportedException("Directories are not supported for the PlayerPrefs location.");
		else if(settings.location == Location.Resources)
			throw new System.NotSupportedException("Checking existence of folder in Resources not supported.");
		return false;
	}

	/// <summary>Gets an array of all of the key names in the default file.</summary>
	public static string[] GetKeys()
	{
		return GetKeys(new ES3Settings());
	}

	/// <summary>Gets an array of all of the key names in a file.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to get the key names from.</param>
	public static string[] GetKeys(string filePath)
	{
		return GetKeys(new ES3Settings(filePath));
	}

	/// <summary>Gets an array of all of the key names in a file.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to get the key names from.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static string[] GetKeys(string filePath, ES3Settings settings)
	{
		return GetKeys(new ES3Settings(filePath, settings));
	}

	/// <summary>Gets an array of all of the key names in a file.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static string[] GetKeys(ES3Settings settings)
	{
		var keys = new List<string>();
		using(var reader = ES3Reader.Create(settings))
		{
			foreach(string key in reader.Properties)
			{
				keys.Add(key);
				reader.Skip();
			}
		}
		return keys.ToArray();
	}

	/// <summary>Gets an array of all of the file names in a directory.</summary>
	public static string[] GetFiles()
	{
		return GetFiles(new ES3Settings());
	}

	/// <summary>Gets an array of all of the file names in a directory.</summary>
	/// <param name="directoryPath">The relative or absolute path of the directory we want to get the file names from.</param>
	public static string[] GetFiles(string directoryPath)
	{
		return GetFiles(new ES3Settings(directoryPath));
	}

	/// <summary>Gets an array of all of the file names in a directory.</summary>
	/// <param name="directoryPath">The relative or absolute path of the directory we want to get the file names from.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static string[] GetFiles(string directoryPath, ES3Settings settings)
	{
		return GetFiles(new ES3Settings(directoryPath, settings));
	}

	/// <summary>Gets an array of all of the file names in a directory.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static string[] GetFiles(ES3Settings settings)
	{
		if(settings.location != ES3.Location.File)
			throw new System.NotSupportedException("ES3.GetFiles can only be used when the location is set to File.");
		return ES3IO.GetFiles(settings.FullPath, false);
	}

	/// <summary>Gets an array of all of the sub-directory names in a directory.</summary>
	public static string[] GetDirectories()
	{
		return GetDirectories(new ES3Settings());
	}

	/// <summary>Gets an array of all of the sub-directory names in a directory.</summary>
	/// <param name="directoryPath">The relative or absolute path of the directory we want to get the sub-directory names from.</param>
	public static string[] GetDirectories(string directoryPath)
	{
		return GetDirectories(new ES3Settings(directoryPath));
	}

	/// <summary>Gets an array of all of the sub-directory names in a directory.</summary>
	/// <param name="directoryPath">The relative or absolute path of the directory we want to get the sub-directory names from.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static string[] GetDirectories(string directoryPath, ES3Settings settings)
	{
		return GetDirectories(new ES3Settings(directoryPath, settings));
	}

	/// <summary>Gets an array of all of the sub-directory names in a directory.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static string[] GetDirectories(ES3Settings settings)
	{
		if(settings.location != ES3.Location.File)
			throw new System.NotSupportedException("ES3.GetDirectories can only be used when the location is set to File.");	
		return ES3IO.GetDirectories(settings.FullPath, false);
	}

	/// <summary>Creates a backup of the default file .</summary>
	/// <remarks>A backup is created by copying the file and giving it a .bak extension. 
	/// If a backup already exists it will be overwritten, so you will need to ensure that the old backup will not be required before calling this method.</remarks>
	public static void CreateBackup()
	{
		CreateBackup(new ES3Settings());
	}

	/// <summary>Creates a backup of a file.</summary>
	/// <remarks>A backup is created by copying the file and giving it a .bak extension. 
	/// If a backup already exists it will be overwritten, so you will need to ensure that the old backup will not be required before calling this method.</remarks>
	/// <param name="filePath">The relative or absolute path of the file we wish to create a backup of.</param>
	public static void CreateBackup(string filePath)
	{
		CreateBackup(new ES3Settings(filePath));
	}

	/// <summary>Creates a backup of a file.</summary>
	/// <remarks>A backup is created by copying the file and giving it a .bak extension. 
	/// If a backup already exists it will be overwritten, so you will need to ensure that the old backup will not be required before calling this method.</remarks>
	/// <param name="filePath">The relative or absolute path of the file we wish to create a backup of.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void CreateBackup(string filePath, ES3Settings settings)
	{
		CreateBackup(new ES3Settings(filePath, settings));
	}

	/// <summary>Creates a backup of a file.</summary>
	/// <remarks>A backup is created by copying the file and giving it a .bak extension. 
	/// If a backup already exists it will be overwritten, so you will need to ensure that the old backup will not be required before calling this method.</remarks>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static void CreateBackup(ES3Settings settings)
	{
		var backupSettings = new ES3Settings(settings.path+backupFileSuffix, settings);
		ES3.CopyFile(backupSettings, settings);
	}

	/// <summary>Restores a backup of a file.</summary>
	/// <param name="filePath">The relative or absolute path of the file we wish to restore the backup of.</param>
	/// <returns>True if a backup was restored, or False if no backup could be found.</returns>
	public static bool RestoreBackup(string filePath)
	{
		return RestoreBackup(new ES3Settings(filePath));
	}

	/// <summary>Restores a backup of a file.</summary>
	/// <param name="filePath">The relative or absolute path of the file we wish to restore the backup of.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <returns>True if a backup was restored, or False if no backup could be found.</returns>
	public static bool RestoreBackup(string filePath, ES3Settings settings)
	{
		return RestoreBackup(new ES3Settings(filePath, settings));
	}

	/// <summary>Restores a backup of a file.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <returns>True if a backup was restored, or False if no backup could be found.</returns>
	public static bool RestoreBackup(ES3Settings settings)
	{
		var backupSettings = new ES3Settings(settings.path+backupFileSuffix, settings);

		if(!FileExists(backupSettings))
			return false;

		ES3.RenameFile(backupSettings, settings);

		return true;
	}

	public static DateTime GetTimestamp()
	{
		return GetTimestamp(new ES3Settings());
	}

	public static DateTime GetTimestamp(string filePath)
	{
		return GetTimestamp(new ES3Settings(filePath));
	}

	public static DateTime GetTimestamp(string filePath, ES3Settings settings)
	{
		return GetTimestamp(new ES3Settings(filePath, settings));
	}

	/// <summary>Gets the date and time the file was last updated, in the UTC timezone.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <returns>A DateTime object represeting the UTC date and time the file was last updated.</returns>
	public static DateTime GetTimestamp(ES3Settings settings)
	{
		if(settings.location == Location.File)
			return ES3IO.GetTimestamp(settings.FullPath);
		else if(settings.location == Location.PlayerPrefs)
			return new DateTime(long.Parse(PlayerPrefs.GetString("timestamp_"+settings.FullPath, "0")), DateTimeKind.Utc);
		else
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
	}

	/// <summary>Initialises Easy Save. This happens automatically when any ES3 methods are called, but is useful if you want to perform initialisation before calling an ES3 method.</summary>
	public static void Init()
	{
		ES3Settings.LoadDefaults();
		ES3TypeMgr.Init();
	}

	#endregion
}
