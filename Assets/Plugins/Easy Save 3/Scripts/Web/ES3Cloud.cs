﻿#if !DISABLE_WEB
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;
using ES3Internal;

public class ES3Cloud : ES3WebClass
{
	/// <summary>Constructs an new ES3Cloud object with the given URL to an ES3.php file.</summary>
	/// <param name="url">The URL of the ES3.php file on your server you want to use.</param>
	public ES3Cloud(string url, string apiKey) : base(url, apiKey)
	{
	}

	#region Downloaded Data Handling

	/// <summary>The encoding to use when encoding and decoding data as strings.</summary>
	public System.Text.Encoding encoding = System.Text.Encoding.UTF8;


	private byte[] _data = null;
	/// <summary>Any downloaded data, if applicable. This may also contain an error message, so you should check the 'ifError' variable before reading data.</summary>
	public byte[] data
	{
		get{ return _data; }
	}

	/// <summary>The downloaded data as text, decoded using the encoding specified by the 'encoding' variable.</summary>
	public string text
	{
		get
		{
			if(data == null)
				return null;
			return encoding.GetString(data);
		}
	}

	/// <summary>An array of filenames downloaded from the server. This must only be accessed after calling the 'DownloadFilenames' routine.</summary>
	public string[] filenames
	{
		get
		{
			if(data == null || data.Length == 0)
				return new string[0];
			return text.Split(';');
		}
		
	}

	/// <summary>A UTC DateTime object representing the date and time a file on the server was last updated. This should only be called after calling the 'DownloadTimestamp' routine.</summary>
	public DateTime timestamp
	{
		get
		{
			if(data == null || data.Length == 0)
				return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

			double timestamp;
			if(!double.TryParse(text, out timestamp))
				throw new FormatException("Could not convert downloaded data to a timestamp. Data downloaded was: " + text);

			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
		}
	}

	#endregion

	#region Sync

	/// <summary>Synchronises a local file with a file on the server. If the file on the server is newer than the local copy, the local file will be overwritten by the file on the server. Otherwise, the file on the server will be overwritten.</summary>
	/// <param name="filePath">The relative or absolute path of the local file we want to synchronise.</param>
	public IEnumerator Sync(string filePath)
	{
		return Sync(new ES3Settings(filePath), "", "");
	}

	/// <summary>Synchronises a local file with a file on the server. If the file on the server is newer than the local copy, the local file will be overwritten by the file on the server. Otherwise, the file on the server will be overwritten.</summary>
	/// <param name="filePath">The relative or absolute path of the local file we want to synchronise.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	public IEnumerator Sync(string filePath, string user)
	{
		return Sync(new ES3Settings(filePath), user, "");
	}

	/// <summary>Synchronises a local file with a file on the server. If the file on the server is newer than the local copy, the local file will be overwritten by the file on the server. Otherwise, the file on the server will be overwritten.</summary>
	/// <param name="filePath">The relative or absolute path of the local file we want to synchronise.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	public IEnumerator Sync(string filePath, string user, string password)
	{
		return Sync(new ES3Settings(filePath), user, password);
	}

	/// <summary>Synchronises a local file with a file on the server. If the file on the server is newer than the local copy, the local file will be overwritten by the file on the server. Otherwise, the file on the server will be overwritten.</summary>
	/// <param name="filePath">The relative or absolute path of the local file we want to synchronise.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator Sync(string filePath, ES3Settings settings)
	{
		return Sync(new ES3Settings(filePath, settings), "", "");
	}

	/// <summary>Synchronises a local file with a file on the server. If the file on the server is newer than the local copy, the local file will be overwritten by the file on the server. Otherwise, the file on the server will be overwritten.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to use.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator Sync(string filePath, string user, ES3Settings settings)
	{
		return Sync(new ES3Settings(filePath, settings), user, "");
	}

	/// <summary>Synchronises a local file with a file on the server. If the file on the server is newer than the local copy, the local file will be overwritten by the file on the server. Otherwise, the file on the server will be overwritten.</summary>
	/// <param name="filePath">The relative or absolute path of the local file we want to synchronise.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator Sync(string filePath, string user, string password, ES3Settings settings)
	{
		return Sync(new ES3Settings(filePath, settings), user, password);
	}

	/// <summary>Synchronises a local file with a file on the server. If the file on the server is newer than the local copy, the local file will be overwritten by the file on the server. Otherwise, the file on the server will be overwritten.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	private IEnumerator Sync(ES3Settings settings, string user, string password)
	{
		Reset();

		yield return DownloadFile(settings, user, password, GetFileTimestamp(settings));

		if(errorCode == 3)
		{
			// Clear the error.
			Reset();

			// File does not exist on server, or is older than locally stored data, so upload the local file to the server if it exists.
			if(ES3.FileExists(settings))
				yield return UploadFile(settings, user, password);
		}
			
		isDone = true;
	}

	#endregion

	#region UploadFile

	/// <summary>Uploads a local file to the server, overwriting any existing file.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to use.</param>
	public IEnumerator UploadFile(string filePath)
	{
		return UploadFile(new ES3Settings(filePath), "", "");
	}

	/// <summary>Uploads a local file to the server, overwriting any existing file.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to use.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	public IEnumerator UploadFile(string filePath, string user)
	{
		return UploadFile(new ES3Settings(filePath), user, "");
	}

	/// <summary>Uploads a local file to the server, overwriting any existing file.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to use.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	public IEnumerator UploadFile(string filePath, string user, string password)
	{
		return UploadFile(new ES3Settings(filePath), user, password);
	}

	/// <summary>Uploads a local file to the server, overwriting any existing file.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to use.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator UploadFile(string filePath, ES3Settings settings)
	{
		return UploadFile(new ES3Settings(filePath, settings), "", "");
	}

	/// <summary>Uploads a local file to the server, overwriting any existing file.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to use.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator UploadFile(string filePath, string user, ES3Settings settings)
	{
		return UploadFile(new ES3Settings(filePath, settings), user, "");
	}

	/// <summary>Uploads a local file to the server, overwriting any existing file.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to use.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator UploadFile(string filePath, string user, string password, ES3Settings settings)
	{
		return UploadFile(new ES3Settings(filePath, settings), user, password);
	}

	/// <summary>Uploads a local file to the server, overwriting any existing file.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	private IEnumerator UploadFile(ES3Settings settings, string user, string password)
	{
		Reset();

		var form = CreateWWWForm();
		form.AddField("apiKey",  apiKey);
		form.AddField("putFile", settings.path);
		form.AddField("timestamp", DateTimeToUnixTimestamp(ES3.GetTimestamp(settings)).ToString());
		form.AddField("user", GetUser(user, password));
		form.AddBinaryData("data", ES3.LoadRawBytes(settings), "data.dat", "multipart/form-data");

		using(var webRequest = UnityWebRequest.Post(url, form))
		{
			yield return SendWebRequest(webRequest);
			HandleError(webRequest, true);
		}

		isDone = true;
	}

	#endregion

	#region DownloadFile

	/// <summary>Downloads a file from the server and saves it locally, overwriting any existing local file. An error is returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to download.</param>
	public IEnumerator DownloadFile(string filePath)
	{
		return DownloadFile(new ES3Settings(filePath), "", "", 0);
	}

	/// <summary>Downloads a file from the server and saves it locally, overwriting any existing local file. An error is returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to download.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	public IEnumerator DownloadFile(string filePath, string user)
	{
		return DownloadFile(new ES3Settings(filePath), user, "", 0);
	}

	/// <summary>Downloads a file from the server and saves it locally, overwriting any existing local file. An error is returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to download.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	public IEnumerator DownloadFile(string filePath, string user, string password)
	{
		return DownloadFile(new ES3Settings(filePath), user, password, 0);
	}

	/// <summary>Downloads a file from the server and saves it locally, overwriting any existing local file. An error is returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to download.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator DownloadFile(string filePath, ES3Settings settings)
	{
		return DownloadFile(new ES3Settings(filePath, settings), "", "", 0);
	}

	/// <summary>Downloads a file from the server and saves it locally, overwriting any existing local file. An error is returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to download.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator DownloadFile(string filePath, string user, ES3Settings settings)
	{
		return DownloadFile(new ES3Settings(filePath, settings), user, "", 0);
	}

	/// <summary>Downloads a file from the server and saves it locally, overwriting any existing local file. An error is returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to download.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator DownloadFile(string filePath, string user, string password, ES3Settings settings)
	{
		return DownloadFile(new ES3Settings(filePath, settings), user, password, 0);
	}

	private IEnumerator DownloadFile(ES3Settings settings, string user, string password, long timestamp)
	{
		Reset();

		var form = CreateWWWForm();
		form.AddField("apiKey",  apiKey);
		form.AddField("getFile", settings.path);
		form.AddField("user", GetUser(user, password));
		if(timestamp > 0)
			form.AddField("timestamp", timestamp.ToString());

		using(var webRequest = UnityWebRequest.Post(url, form))
		{
			yield return SendWebRequest(webRequest);

			if(!HandleError(webRequest, false))
			{
				if(webRequest.downloadedBytes > 0)
					ES3.SaveRaw(webRequest.downloadHandler.data, settings);
				else
				{
					error = string.Format("File {0} was not found on the server.", settings.path);
					errorCode = 3;
				}
			}
		}

		isDone = true;
	}

	#endregion

	#region DeleteFile

	/// <summary>Deletes a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	public IEnumerator DeleteFile(string filePath)
	{
		return DeleteFile(new ES3Settings(filePath), "", "");
	}

	/// <summary>Deletes a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	public IEnumerator DeleteFile(string filePath, string user)
	{
		return DeleteFile(new ES3Settings(filePath), user, "");
	}

	/// <summary>Deletes a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	public IEnumerator DeleteFile(string filePath, string user, string password)
	{
		return DeleteFile(new ES3Settings(filePath), user, password);
	}

	/// <summary>Deletes a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator DeleteFile(string filePath, ES3Settings settings)
	{
		return DeleteFile(new ES3Settings(filePath, settings), "", "");
	}

	/// <summary>Deletes a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator DeleteFile(string filePath, string user, ES3Settings settings)
	{
		return DeleteFile(new ES3Settings(filePath, settings), user, "");
	}

	/// <summary>Deletes a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator DeleteFile(string filePath, string user, string password, ES3Settings settings)
	{
		return DeleteFile(new ES3Settings(filePath, settings), user, password);
	}

	private IEnumerator DeleteFile(ES3Settings settings, string user, string password)
	{
		Reset();

		var form = CreateWWWForm();
		form.AddField("apiKey",  apiKey);
		form.AddField("deleteFile", settings.path);
		form.AddField("user", GetUser(user, password));

		using(var webRequest = UnityWebRequest.Post(url, form))
		{
			yield return SendWebRequest(webRequest);
			HandleError(webRequest, true);
		}

		isDone = true;
	}

	#endregion

	#region RenameFile

	/// <summary>Renames a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	public IEnumerator RenameFile(string filePath, string newFilePath)
	{
		return RenameFile(new ES3Settings(filePath), new ES3Settings(newFilePath), "", "");
	}

	/// <summary>Renames a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	public IEnumerator RenameFile(string filePath, string newFilePath, string user)
	{
		return RenameFile(new ES3Settings(filePath), new ES3Settings(newFilePath), user, "");
	}

	/// <summary>Renames a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	public IEnumerator RenameFile(string filePath, string newFilePath, string user, string password)
	{
		return RenameFile(new ES3Settings(filePath), new ES3Settings(newFilePath), user, password);
	}

	/// <summary>Renames a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator RenameFile(string filePath, string newFilePath, ES3Settings settings)
	{
		return RenameFile(new ES3Settings(filePath, settings), new ES3Settings(newFilePath, settings), "", "");
	}

	/// <summary>Renames a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator RenameFile(string filePath, string newFilePath, string user, ES3Settings settings)
	{
		return RenameFile(new ES3Settings(filePath, settings), new ES3Settings(newFilePath, settings), user, "");
	}

	/// <summary>Renames a file from the server. An error is *not* returned if the file does not exist.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to delete.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator RenameFile(string filePath, string newFilePath, string user, string password, ES3Settings settings)
	{
		return RenameFile(new ES3Settings(filePath, settings), new ES3Settings(newFilePath, settings), user, password);
	}

	private IEnumerator RenameFile(ES3Settings settings, ES3Settings newSettings, string user, string password)
	{
		Reset();

		var form = CreateWWWForm();
		form.AddField("apiKey",  apiKey);
		form.AddField("renameFile", settings.path);
		form.AddField("newFilename", newSettings.path);
		form.AddField("user", GetUser(user, password));

		using(var webRequest = UnityWebRequest.Post(url, form))
		{
			yield return SendWebRequest(webRequest);
			HandleError(webRequest, true);
		}

		isDone = true;
	}

	#endregion

	#region DownloadFilenames

	/// <summary>Downloads the names of all of the files on the server. Downloaded filenames are stored in the 'filenames' variable of the ES3Cloud object.</summary>
	public IEnumerator DownloadFilenames()
	{
		return DownloadFilenames("", "");
	}

	/// <summary>Downloads the names of all of the files on the server. Downloaded filenames are stored in the 'filenames' variable of the ES3Cloud object.</summary>
	/// <param name="user">The unique name of the user we want to find the filenames of.</param>
	public IEnumerator DownloadFilenames(string user)
	{
		return DownloadFilenames(user, "");
	}

	/// <summary>Downloads the names of all of the files on the server. Downloaded filenames are stored in the 'filenames' variable of the ES3Cloud object.</summary>
	/// <param name="user">The unique name of the user we want to find the filenames of.</param>
	/// <param name="password">The password of the user we want to find the filenames of.</param>
	public IEnumerator DownloadFilenames(string user, string password)
	{
		Reset();

		var form = CreateWWWForm();
		form.AddField("apiKey",  apiKey);
		form.AddField("getFilenames", "");
		form.AddField("user", GetUser(user, password));

		using(var webRequest = UnityWebRequest.Post(url, form))
		{
			yield return SendWebRequest(webRequest);
			if(!HandleError(webRequest, false))
				_data = webRequest.downloadHandler.data;
		}

		isDone = true;
	}

	#endregion

	#region DownloadTimestamp

	/// <summary>Downloads the timestamp representing when the server file was last updated. The downloaded timestamp is stored in the 'timestamp' variable of the ES3Cloud object.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to get the timestamp of.</param>
	public IEnumerator DownloadTimestamp(string filePath)
	{
		return DownloadTimestamp(new ES3Settings(filePath), "", "");
	}

	/// <summary>Downloads the timestamp representing when the server file was last updated. The downloaded timestamp is stored in the 'timestamp' variable of the ES3Cloud object.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to get the timestamp of.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	public IEnumerator DownloadTimestamp(string filePath, string user)
	{
		return DownloadTimestamp(new ES3Settings(filePath), user, "");
	}

	/// <summary>Downloads the timestamp representing when the server file was last updated. The downloaded timestamp is stored in the 'timestamp' variable of the ES3Cloud object.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to get the timestamp of.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	public IEnumerator DownloadTimestamp(string filePath, string user, string password)
	{
		return DownloadTimestamp(new ES3Settings(filePath), user, password);
	}

	/// <summary>Downloads the timestamp representing when the server file was last updated. The downloaded timestamp is stored in the 'timestamp' variable of the ES3Cloud object.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to get the timestamp of.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator DownloadTimestamp(string filePath, ES3Settings settings)
	{
		return DownloadTimestamp(new ES3Settings(filePath, settings), "", "");
	}

	/// <summary>Downloads the timestamp representing when the server file was last updated. The downloaded timestamp is stored in the 'timestamp' variable of the ES3Cloud object.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to get the timestamp of.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator DownloadTimestamp(string filePath, string user, ES3Settings settings)
	{
		return DownloadTimestamp(new ES3Settings(filePath, settings), user, "");
	}

	/// <summary>Downloads the timestamp representing when the server file was last updated. The downloaded timestamp is stored in the 'timestamp' variable of the ES3Cloud object.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to get the timestamp of.</param>
	/// <param name="user">The unique name of the user this file belongs to, if the file isn't globally accessible.</param>
	/// <param name="password">The password of the user this file belongs to.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public IEnumerator DownloadTimestamp(string filePath, string user, string password, ES3Settings settings)
	{
		return DownloadTimestamp(new ES3Settings(filePath, settings), user, password);
	}

	private IEnumerator DownloadTimestamp(ES3Settings settings, string user, string password)
	{
		Reset();

		var form = CreateWWWForm();
		form.AddField("apiKey",  apiKey);
		form.AddField("getTimestamp", settings.path);
		form.AddField("user", GetUser(user, password));

		using(var webRequest = UnityWebRequest.Post(url, form))
		{
			yield return SendWebRequest(webRequest);
			if(!HandleError(webRequest, false))
				_data = webRequest.downloadHandler.data;
		}

		isDone = true;
	}

	#endregion

	#region Internal Methods

	private long DateTimeToUnixTimestamp(DateTime dt)
	{
		return Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(dt) - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
	}

	private long GetFileTimestamp(ES3Settings settings)
	{
		return DateTimeToUnixTimestamp(ES3.GetTimestamp(settings));
	}

	protected override void Reset()
	{
		_data = null;
		base.Reset();
	}

	#endregion
}

#endif