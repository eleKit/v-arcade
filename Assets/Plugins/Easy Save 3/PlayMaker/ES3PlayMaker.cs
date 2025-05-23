﻿#if PLAYMAKER_1_8_OR_NEWER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

public class FsmES3File : ScriptableObject
{
	public ES3File file;
}

namespace ES3PlayMaker
{

	#region Base Classes

	public abstract class ActionBase : FsmStateAction
	{
		[Tooltip("This event is triggered if an error occurs.")]
		public FsmEvent errorEvent;
		[Tooltip("If an error occurs, the error message will be stored in this variable.")]
		public FsmString errorMessage;

		public abstract void Enter();
		public abstract void OnReset();

		public override void OnEnter()
		{
			try
			{
				Enter();
			}
			catch(System.Exception e)
			{
				HandleError(e.ToString());
			}
			Finish();
		}

		public override void Reset()
		{
			errorEvent = null;
			errorMessage = "";
			OnReset();
		}

		public void HandleError(string msg)
		{
			errorMessage.Value = msg;
			if(errorEvent != null)
				Fsm.Event(errorEvent);
			else
				LogError(msg);
		}
	}

	public abstract class SettingsAction : ActionBase
	{
		public FsmBool overrideDefaultSettings = false;

		[Tooltip("The path this ES3Settings object points to, if any.")]
		public FsmString path;
		[ObjectType(typeof(ES3.Location))]
		[Tooltip("The storage location where we wish to store data by default.")]
		public FsmEnum location;
		[ObjectType(typeof(ES3.EncryptionType))]
		[Tooltip("The type of encryption to use when encrypting data, if any.")]
		public FsmEnum encryptionType;
		[Tooltip("The password to use to encrypt the data if encryption is enabled.")]
		public FsmString encryptionPassword;
		[ObjectType(typeof(ES3.Directory))]
		[Tooltip("The default directory in which to store files when using the File save location, and the location which relative paths should be relative to.")]
		public FsmEnum directory;
		[ObjectType(typeof(ES3.Format))]
		[Tooltip("The format we should use when serializing and deserializing data.")]
		public FsmEnum format;
		[Tooltip("Any stream buffers will be set to this length in bytes.")]
		public FsmInt bufferSize;

		public override void Reset()
		{
			var settings = new ES3Settings();
			path = settings.path;
			location = settings.location;
			encryptionType = settings.encryptionType;
			encryptionPassword = settings.encryptionPassword;
			directory = settings.directory;
			format = settings.format;
			bufferSize = settings.bufferSize;
			overrideDefaultSettings = false;
			base.Reset();
		}

		public ES3Settings GetSettings()
		{
			var settings = new ES3Settings();
			if(overrideDefaultSettings.Value)
			{
				settings.path = path.Value;
				settings.location = (ES3.Location)location.Value;
				settings.encryptionType = (ES3.EncryptionType)encryptionType.Value;
				settings.encryptionPassword = encryptionPassword.Value;
				settings.directory = (ES3.Directory)directory.Value;
				settings.format = (ES3.Format)format.Value;
				settings.bufferSize = bufferSize.Value;
			}
			return settings;
		}
	}

	public abstract class ES3FileAction : ActionBase
	{
		[Tooltip("The ES3 File we are using, created using the Create ES3 File action.")]
		[ObjectType(typeof(FsmES3File))]
		[Title("ES3 File")]
		[RequiredField]
		public FsmObject fsmES3File;

		public ES3File es3File { get{ return ((FsmES3File)fsmES3File.Value).file; } }

		public override void Reset()
		{
			fsmES3File = null;
			base.Reset();
		}
	}

	public abstract class ES3FileSettingsAction : SettingsAction
	{
		[Tooltip("The ES3File variable we're using.")]
		[ObjectType(typeof(FsmES3File))]
		[Title("ES3 File")]
		[RequiredField]
		public FsmObject fsmES3File;

		public ES3File es3File { get{ return ((FsmES3File)fsmES3File.Value).file; } }

		public override void Reset()
		{
			fsmES3File = null;
			base.Reset();
		}
	}

	#endregion

	#region Save Actions

	[ActionCategory("Easy Save 3")]
	[Tooltip("Saves the value to a file with the given key.")]
	public class Save : SettingsAction
	{
		[Tooltip("The unique key we want to use to identity the data we are saving.")]
		public FsmString key;
		[Tooltip("The value we want to save.")]
		public FsmVar value;

		public override void OnReset()
		{
			key = "key";
			value = null;
		}

		public override void Enter()
		{
			value.UpdateValue();
			ES3.Save<object>(key.Value, value.GetValue(), GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Saves a byte array as a file, overwriting any existing files.")]
	public class SaveRaw : SettingsAction
	{
		[Tooltip("The string we want to save as a file.")]
		public FsmString str;
		[Tooltip("Whether to encode this string using Base-64 encoding. This will override any default encoding settings.")]
		public FsmBool useBase64Encoding;

		public override void OnReset()
		{
			str = "";
			useBase64Encoding = false;
		}

		public override void Enter()
		{
			if(useBase64Encoding.Value)
				ES3.SaveRaw(System.Convert.FromBase64String(str.Value), GetSettings());
			else
				ES3.SaveRaw(str.Value, GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("")]
	public class AppendRaw : SettingsAction
	{
		[Tooltip("The string we want to append to a file.")]
		public FsmString str;
		[Tooltip("Whether to encode this string using Base-64 encoding. This will override any default encoding settings.")]
		public FsmBool useBase64Encoding;

		public override void OnReset()
		{
			str = "";
			useBase64Encoding = false;
		}

		public override void Enter()
		{
			if(useBase64Encoding.Value)
				ES3.AppendRaw(System.Convert.FromBase64String(str.Value), GetSettings());
			else
				ES3.AppendRaw(str.Value, GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Saves a Texture2D as a PNG or a JPG, depending on the file extension of the supplied image path.")]
	public class SaveImage : SettingsAction
	{
		[Tooltip("The relative or absolute path of the PNG or JPG file we want to store our image to.")]
		public FsmString imagePath;
		[Tooltip("The Texture2D we want to save as an image.")]
		[ObjectType(typeof(Texture2D))]
		public FsmTexture texture2D;

		public override void OnReset()
		{
			imagePath = "image.png";
			texture2D = null;
		}

		public override void Enter()
		{
			ES3.SaveImage((Texture2D)texture2D.Value, imagePath.Value, GetSettings());
		}
	}

	#endregion

	#region Load Actions

	[ActionCategory("Easy Save 3")]
	[Tooltip("Loads a value from a file with the given key.")]
	public class Load : SettingsAction
	{
		[Tooltip("The unique key which identifies the data we're loading.")]
		public FsmString key;
		[Tooltip("The variable we want to use to store our loaded data.")]
		public FsmVar value;
		[Tooltip("Optional: A value to return if the key does not exist.")]
		public FsmVar defaultValue;

		public override void OnReset()
		{
			key = "key";
			value = null;
			defaultValue = null;
		}

		public override void Enter()
		{
			defaultValue.UpdateValue();
			if(defaultValue.GetValue() != null && !defaultValue.IsNone)
				value.SetValue( ES3.Load<object>(key.Value, defaultValue.GetValue(), GetSettings()) );
			else
				value.SetValue( ES3.Load<object>(key.Value, GetSettings()) );
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Loads a value from a file with the given key into an existing object, rather than creating a new instance.")]
	public class LoadInto : SettingsAction
	{
		[Tooltip("The unique key which identifies the data we're loading.")]
		public FsmString key;
		[Tooltip("The object we want to load our data into.")]
		[RequiredField]
		public FsmVar value;

		public override void OnReset()
		{
			key = "key";
			value = null;
		}

		public override void Enter()
		{
			value.UpdateValue();
			if(value.IsNone || value.GetValue() == null)
				HandleError("The 'Load Into' action requires an object to load the data into, but none was specified in the 'Value' field.");
			else
				ES3.LoadInto<object>(key.Value, value.GetValue(), GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Loads an entire file as a string.")]
	public class LoadRawString : SettingsAction
	{
		[Tooltip("The variable we want to store our loaded string in.")]
		public FsmString str;
		[Tooltip("Whether or not the data we're loading is Base-64 encoded. Usually this should be left unchecked.")]
		public FsmBool useBase64Encoding;

		public override void OnReset()
		{
			str = null;
			useBase64Encoding = false;
		}

		public override void Enter()
		{
			if(useBase64Encoding.Value)
				str.Value = System.Convert.ToBase64String( ES3.LoadRawBytes(GetSettings()) );
			else
				str.Value = ES3.LoadRawString(GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Loads a JPG or PNG image file as a Texture2D.")]
	public class LoadImage : SettingsAction
	{
		[Tooltip("The relative or absolute path of the JPG or PNG image file we want to load.")]
		public FsmString imagePath;
		[Tooltip("The variable we want to use to store our loaded texture.")]
		public FsmTexture texture2D;

		public override void OnReset()
		{
			imagePath = "image.png";
			texture2D = null;
		}

		public override void Enter()
		{
			texture2D.Value = ES3.LoadImage(imagePath.Value, GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Loads an audio file as an AudioClip.")]
	public class LoadAudio : SettingsAction
	{
		[Tooltip("The relative or absolute path of the audio file we want to load.")]
		public FsmString audioFilePath;
		[ObjectType(typeof(AudioClip))]
		[Tooltip("The variable we want to use to store our loaded AudioClip.")]
		public FsmObject audioClip;

		public override void OnReset()
		{
			audioFilePath = "audio.wav";
			audioClip = null;
		}

		public override void Enter()
		{
			audioClip.Value = ES3.LoadAudio(audioFilePath.Value, GetSettings());
		}
	}


	#endregion

	#region Exists Actions

	[ActionCategory("Easy Save 3")]
	[Tooltip("Checks whether a key exists in a file.")]
	public class KeyExists : SettingsAction
	{
		[Tooltip("The key we want to check the existence of.")]
		public FsmString key;
		[Tooltip("Whether the key exists. This is set after the action runs.")]
		public FsmBool exists;

		[Tooltip("This event is triggered if the key exists.")]
		public FsmEvent existsEvent;
		[Tooltip("This event is triggered if the key does not exist.")]
		public FsmEvent doesNotExistEvent;

		public override void OnReset()
		{
			key = "key";
			exists = false;
			existsEvent = null;
			doesNotExistEvent = null;
		}

		public override void Enter()
		{
			exists.Value = ES3.KeyExists(key.Value, GetSettings());

			if(exists.Value && existsEvent != null)
				Fsm.Event(existsEvent);
			else if(doesNotExistEvent != null)
				Fsm.Event(doesNotExistEvent);
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Checks whether a file exists in a directory.")]
	public class FileExists : SettingsAction
	{
		[Tooltip("The file we want to check the existence of")]
		public FsmString filePath;
		[Tooltip("Whether the file exists. This is set after the action runs.")]
		public FsmBool exists;

		[Tooltip("This event is triggered if the file exists.")]
		public FsmEvent existsEvent;
		[Tooltip("This event is triggered if the file does not exist.")]
		public FsmEvent doesNotExistEvent;

		public override void OnReset()
		{
			filePath = "SaveFile.es3";
			exists = false;
			existsEvent = null;
			doesNotExistEvent = null;
		}

		public override void Enter()
		{
			exists.Value = ES3.FileExists(filePath.Value, GetSettings());

			if(exists.Value && existsEvent != null)
				Fsm.Event(existsEvent);
			else if(doesNotExistEvent != null)
				Fsm.Event(doesNotExistEvent);
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Checks whether a directory exists in another directory")]
	public class DirectoryExists : SettingsAction
	{
		[Tooltip("The directory we want to check the existence of.")]
		public FsmString directoryPath;
		[Tooltip("Whether the directory exists. This is set after the action runs.")]
		public FsmBool exists;

		[Tooltip("This event is triggered if the directory exists.")]
		public FsmEvent existsEvent;
		[Tooltip("This event is triggered if the directory does not exist.")]
		public FsmEvent doesNotExistEvent;

		public override void OnReset()
		{
			directoryPath = "";
			exists = false;
			existsEvent = null;
			doesNotExistEvent = null;
		}

		public override void Enter()
		{
			exists.Value = ES3.DirectoryExists(directoryPath.Value, GetSettings());

			if(exists.Value && existsEvent != null)
				Fsm.Event(existsEvent);
			else if(doesNotExistEvent != null)
				Fsm.Event(doesNotExistEvent);
		}
	}

	#endregion

	#region Delete Actions

	[ActionCategory("Easy Save 3")]
	[Tooltip("Deletes a key from a file.")]
	public class DeleteKey : SettingsAction
	{
		[Tooltip("The key we want to delete.")]
		public FsmString key;

		public override void OnReset()
		{
			key = "key";
		}

		public override void Enter()
		{
			ES3.DeleteKey(key.Value, GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Deletes a file.")]
	public class DeleteFile : SettingsAction
	{
		[Tooltip("The relative or absolute path of the file we want to delete.")]
		public FsmString filePath;

		public override void OnReset()
		{
			filePath = "SaveFile.es3";
		}

		public override void Enter()
		{
			ES3.DeleteFile(filePath.Value, GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Deletes a directory and it's contents.")]
	public class DeleteDirectory : SettingsAction
	{
		[Tooltip("The relative or absolute path of the directory we want to delete.")]
		public FsmString directoryPath;

		public override void OnReset()
		{
			directoryPath = "";
		}

		public override void Enter()
		{
			ES3.DeleteDirectory(directoryPath.Value, GetSettings());
		}
	}

	#endregion

	#region Backup Actions

	[ActionCategory("Easy Save 3")]
	[Tooltip("Creates a backup of a file which can be restored using the Restore Backup action.")]
	public class CreateBackup : SettingsAction
	{
		[Tooltip("The relative or absolute path of the file we want to backup.")]
		public FsmString filePath;

		public override void OnReset()
		{
			filePath = "SaveFile.es3";
		}

		public override void Enter()
		{
			ES3.CreateBackup(filePath.Value, GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Restores a backup of a file created using the Create Backup action.")]
	public class RestoreBackup : SettingsAction
	{
		[Tooltip("The relative or absolute path of the file we want to restore the backup of.")]
		public FsmString filePath;
		[Tooltip("True if a backup was restored, or False if no backup could be found.")]
		public FsmBool backupWasRestored;

		public override void OnReset()
		{
			filePath = "SaveFile.es3";
			backupWasRestored = false;
		}

		public override void Enter()
		{
			backupWasRestored.Value = ES3.RestoreBackup(filePath.Value, GetSettings());
		}
	}

	#endregion

	#region Key, File and Directory Methods

	[ActionCategory("Easy Save 3")]
	[Tooltip("Renames a file.")]
	public class RenameFile : SettingsAction
	{
		[Tooltip("The relative or absolute path of the file we want to rename from.")]
		public FsmString oldFilePath;
		[Tooltip("The relative or absolute path of the file we want to rename to.")]
		public FsmString newFilePath;

		public override void OnReset()
		{
			oldFilePath = "SaveFile.es3";
			newFilePath = "";
		}

		public override void Enter()
		{
			ES3.RenameFile(oldFilePath.Value, newFilePath.Value, GetSettings(), GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Copies a file.")]
	public class CopyFile : SettingsAction
	{
		[Tooltip("The relative or absolute path of the file we want to copy.")]
		public FsmString oldFilePath;
		[Tooltip("The relative or absolute path of the file we want to create.")]
		public FsmString newFilePath;

		public override void OnReset()
		{
			oldFilePath = "SaveFile.es3";
			newFilePath = "";
		}

		public override void Enter()
		{
			ES3.CopyFile(oldFilePath.Value, newFilePath.Value, GetSettings(), GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Gets an array of key names from a file.")]
	public class GetKeys : SettingsAction
	{
		[Tooltip("The relative or absolute path of the file we want to get the keys from.")]
		public FsmString filePath;
		[Tooltip("The string array variable we want to load our key names into.")]
		[ArrayEditor(VariableType.String)]
		public FsmArray keys;

		public override void OnReset()
		{
			filePath = "SaveFile.es3";
			keys = null;
		}

		public override void Enter()
		{
			keys.Values = ES3.GetKeys(filePath.Value, GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Gets how many keys are in a file.")]
	public class GetKeyCount : SettingsAction
	{
		[Tooltip("The relative or absolute path of the file we want to count the keys of.")]
		public FsmString filePath;
		[Tooltip("The int variable we want to load our count into.")]
		public FsmInt keyCount;

		public override void OnReset()
		{
			filePath = "SaveFile.es3";
			keyCount = null;
		}

		public override void Enter()
		{
			keyCount.Value = ES3.GetKeys(filePath.Value, GetSettings()).Length;
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Gets the names of the files in a given directory.")]
	public class GetFiles : SettingsAction
	{
		[Tooltip("The relative or absolute path of the directory we want to get the file names from.")]
		public FsmString directoryPath;
		[Tooltip("The string array variable we want to load our file names into.")]
		[ArrayEditor(VariableType.String)]
		public FsmArray files;

		public override void OnReset()
		{
			directoryPath = "";
			files = null;
		}

		public override void Enter()
		{
			files.Values = ES3.GetFiles(directoryPath.Value, GetSettings());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Gets the names of any directories in a given directory.")]
	public class GetDirectories : SettingsAction
	{
		[Tooltip("The relative or absolute path of the directory we want to get the directory names from.")]
		public FsmString directoryPath;
		[Tooltip("The string array variable we want to load our directory names into.")]
		[ArrayEditor(VariableType.String)]
		public FsmArray directories;

		public override void OnReset()
		{
			directoryPath = "";
			directories = null;
		}

		public override void Enter()
		{
			directories.Values = ES3.GetDirectories(directoryPath.Value, GetSettings());
		}
	}

	#endregion

	#region ES3File Actions

	[ActionCategory("Easy Save 3")]
	[Tooltip("Creates a new ES3File, and optionally loads a file from storage into it.")]
	public class ES3FileCreate : ES3FileSettingsAction
	{
		[Tooltip("The relative or absolute path of the file this ES3File represents in storage.")]
		public FsmString filePath;
		[Tooltip("Whether we should sync this ES3File with the one in storage immediately after creating it.")]
		public FsmBool syncWithFile;

		public override void OnReset()
		{
			filePath = "SaveFile.es3";
			syncWithFile = true;
		}

		public override void Enter()
		{
			var file = ScriptableObject.CreateInstance<FsmES3File>();
			file.file = new ES3File(filePath.Value, GetSettings(), syncWithFile.Value);
			fsmES3File.Value = file;
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Synchronises this ES3File with a file in storage.")]
	public class ES3FileSync : ES3FileSettingsAction
	{
		[Tooltip("The relative or absolute path of the file we want to synchronise with in storage.")]
		public FsmString filePath;

		public override void OnReset()
		{
			filePath = "SaveFile.es3";
		}

		public override void Enter()
		{
			if(overrideDefaultSettings.Value)
				es3File.Sync(GetSettings());
			else
				es3File.Sync();
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Saves the value to the ES3File with the given key.")]
	public class ES3FileSave : ES3FileAction
	{
		[Tooltip("The unique key we want to use to identity the data we are saving.")]
		public FsmString key;
		[Tooltip("The value we want to save.")]
		public FsmVar value;

		public override void OnReset()
		{
			key = "key";
			value = null;
		}

		public override void Enter()
		{
			value.UpdateValue();
			es3File.Save<object>(key.Value, value.GetValue());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Loads a value with the given key from the ES3File")]
	public class ES3FileLoad : ES3FileAction
	{
		[Tooltip("The unique key which identifies the data we're loading.")]
		public FsmString key;
		[Tooltip("The variable we want to use to store our loaded data.")]
		public FsmVar value;
		[Tooltip("Optional: A value to return if the key does not exist.")]
		public FsmVar defaultValue;

		public override void OnReset()
		{
			key = "key";
			value = null;
			defaultValue = null;
		}

		public override void Enter()
		{
			defaultValue.UpdateValue();
			if(defaultValue.GetValue() != null && !defaultValue.IsNone)
				value.SetValue(es3File.Load<object>(key.Value, defaultValue.GetValue()));
			else
				value.SetValue(es3File.Load<object>(key.Value));
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Loads a value with the given key from the ES3File into an existing object")]
	public class ES3FileLoadInto : ES3FileAction
	{
		[Tooltip("The unique key which identifies the data we're loading.")]
		public FsmString key;
		[Tooltip("The variable we want to load our data into.")]
		public FsmVar value;

		public override void OnReset()
		{
			key = "key";
			value = null;
		}

		public override void Enter()
		{
			value.UpdateValue();
			es3File.LoadInto<object>(key.Value, value.GetValue());
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Loads the entire ES3 File as a string")]
	public class ES3FileLoadRawString : ES3FileAction
	{
		[Tooltip("The FsmArray variable we want to use to store our string representing the file.")]
		public FsmString str;
		[Tooltip("Whether or not the data we're loading is Base-64 encoded. Usually this should be left unchecked.")]
		public FsmBool useBase64Encoding;

		public override void OnReset()
		{
			str = null;
			useBase64Encoding = false;
		}

		public override void Enter()
		{
			if(useBase64Encoding.Value)
				str.Value = System.Convert.ToBase64String( es3File.LoadRawBytes() );
			else
				str.Value = es3File.LoadRawString();
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Deletes a key from an ES3 File.")]
	public class ES3FileDeleteKey : ES3FileAction
	{
		[Tooltip("The key we want to delete.")]
		public FsmString key;

		public override void OnReset()
		{
			key = "key";
		}

		public override void Enter()
		{
			es3File.DeleteKey(key.Value);
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Checks whether a key exists in an ES3File.")]
	public class ES3FileKeyExists : ES3FileAction
	{
		[Tooltip("The key we want to check the existence of.")]
		public FsmString key;
		[Tooltip("Whether the key exists. This is set after the action runs.")]
		public FsmBool exists;

		[Tooltip("This event is triggered if the key exists.")]
		public FsmEvent existsEvent;
		[Tooltip("This event is triggered if the key does not exist.")]
		public FsmEvent doesNotExistEvent;

		public override void OnReset()
		{
			key = "key";
			exists = false;
			existsEvent = null;
			doesNotExistEvent = null;
		}

		public override void Enter()
		{
			exists.Value = es3File.KeyExists(key.Value);

			if(exists.Value && existsEvent != null)
				Fsm.Event(existsEvent);
			else if(doesNotExistEvent != null)
				Fsm.Event(doesNotExistEvent);
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Gets an array of key names from an ES3File.")]
	public class ES3FileGetKeys : ES3FileAction
	{
		[Tooltip("The string array variable we want to load our key names into.")]
		[ArrayEditor(VariableType.String)]
		public FsmArray keys;

		public override void OnReset()
		{
			keys = null;
		}

		public override void Enter()
		{
			keys.Values = es3File.GetKeys();
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Clears all keys from an ES3File.")]
	public class ES3FileClear : ES3FileAction
	{
		public override void OnReset(){}

		public override void Enter()
		{
			es3File.Clear();
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Gets an array of key names from a file.")]
	public class ES3FileSize : ES3FileAction
	{
		[Tooltip("The variable we want to put the file size into.")]
		public FsmInt size;

		public override void OnReset()
		{
			size = 0;
		}

		public override void Enter()
		{
			size.Value = es3File.Size();
		}
	}

	#endregion

	#region ES3Cloud Actions

	#if !DISABLE_WEB
	public abstract class ES3CloudAction : SettingsAction
	{
		[Tooltip("The URL to the ES3Cloud.php file on your server.")]
		[RequiredField]
		public FsmString url;
		[Tooltip("The API key generated when installing ES3 Cloud on your server.")]
		[RequiredField]
		public FsmString apiKey;

		[Tooltip("An error code if an error occurred.")]
		public FsmInt errorCode;

		protected ES3Cloud cloud = null;

		public override void OnReset()
		{
			url = "http://www.myserver.com/ES3Cloud.php";
			errorCode = 0;
			cloud = null;
		}

		public override void OnEnter()
		{
			try
			{
				CreateES3Cloud();
				Enter();
			}
			catch(System.Exception e)
			{
				HandleError(e.ToString());
			}
		}

		public override void OnUpdate()
		{
			if(cloud.isError)
			{
				errorCode = (int)cloud.errorCode;
				errorMessage = cloud.error;
				Log("Error occurred when trying to perform operation with ES3Cloud: [Error "+cloud.errorCode+"] " + cloud.error);
				Fsm.Event(errorEvent);
			}
			else if(cloud.isDone)
			{
				Finish();
			}
		}

		protected void CreateES3Cloud()
		{
			cloud = new ES3Cloud(url.Value, apiKey.Value);
		}
	}

	public abstract class ES3CloudUserAction : ES3CloudAction
	{
		public FsmString user;
		public FsmString password;

		public override void OnReset()
		{
			base.OnReset();
			user = "";
			password = "";
		}

		public override void Enter()
		{
			var settings = GetSettings();
			StartCoroutine(cloud.Sync(path.Value, user.Value, password.Value, settings));
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Synchronises a file in storage with the server.")]
	public class ES3CloudSync : ES3CloudUserAction
	{
		public override void Enter()
		{
			base.Enter();
			var settings = GetSettings();
			StartCoroutine(cloud.Sync(path.Value, settings));
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Uploads a file in storage to the server, overwriting any existing files.")]
	public class ES3CloudUploadFile : ES3CloudUserAction
	{
		public override void Enter()
		{
			base.Enter();
			var settings = GetSettings();
			StartCoroutine(cloud.UploadFile(path.Value, user.Value, password.Value, settings));
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Downloads a file from the server, overwriting any existing files, or returning error code 3 if no file exists on the server.")]
	public class ES3CloudDownloadFile : ES3CloudUserAction
	{
		public override void Enter()
		{
			base.Enter();
			var settings = GetSettings();
			StartCoroutine(cloud.DownloadFile(path.Value, user.Value, password.Value, settings));
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Downloads a file from the server, overwriting any existing files, or returning error code 3 if no file exists on the server.")]
	public class ES3CloudDeleteFile : ES3CloudUserAction
	{
		public override void Enter()
		{
			base.Enter();
			var settings = GetSettings();
			StartCoroutine(cloud.DeleteFile(path.Value, user.Value, password.Value, settings));
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Renames a file on the server, overwriting any existing files, or returning error code 3 if no file exists on the server.")]
	public class ES3CloudRenameFile : ES3CloudUserAction
	{
		[Tooltip("The name we want to rename the file to.")]
		public FsmString newFilename;

		public override void Enter()
		{
			base.Enter();
			var settings = GetSettings();
			StartCoroutine(cloud.RenameFile(path.Value, newFilename.Value, user.Value, password.Value, settings));
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Downloads the names of all of the files on the server for the given user.")]
	public class ES3CloudDownloadFilenames : ES3CloudUserAction
	{
		[Tooltip("The string array variable we want to load our file names into.")]
		[ArrayEditor(VariableType.String)]
		public FsmArray filenames;

		public override void OnReset()
		{
			filenames = null;
		}

		public override void Enter()
		{
			base.Enter();
			StartCoroutine(cloud.DownloadFilenames(user.Value, password.Value));
		}

		public override void OnUpdate()
		{
			if(cloud != null && cloud.isDone)
				filenames.Values = cloud.filenames;
			base.OnUpdate();
		}
	}

	[ActionCategory("Easy Save 3")]
	[Tooltip("Determines when a file was last updated.")]
	public class ES3CloudDownloadTimestamp : ES3CloudUserAction
	{
		[Tooltip("The Date and time the file was last updated, as a string formatted as yyyy-MM-ddTHH:mm:ss.")]
		public FsmString timestamp;

		public override void OnReset()
		{
			timestamp = "";
		}

		public override void Enter()
		{
			base.Enter();
			StartCoroutine(cloud.DownloadFilenames(user.Value, password.Value));
		}

		public override void OnUpdate()
		{
			if(cloud != null && cloud.isDone)
				timestamp = cloud.timestamp.ToString("s");
			base.OnUpdate();
		}
	}

	#endif
	#endregion
}


#endif
