using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MatchDataExtractor : MonoBehaviour
{
	string directoryPath;

	string music_dataPath = "Assets/MusicTexts";

	void Start ()
	{
		directoryPath = Path.Combine (Application.persistentDataPath, "Paths");

	}

	void Update ()
	{
		
	}


	string FromFilenameToName (string name)
	{
		return name.Replace ("-", " ");

	}

	string FromNameToFilename (string name)
	{
		return name.Replace (" ", "-");

	}

	//extracts the Level Name string from the file containing the GameMatch element
	public string FromMatchDataToLevelName (string match_data_path)
	{

		string matchString = File.ReadAllText (match_data_path);

		GameMatch match_data = JsonUtility.FromJson<GameMatch> (matchString);

		return match_data.id_path;
		
	}

	/* set in the GlobalPlayedData instance the attributes of the yaw and pitch thresholds, 
	 * these thresholds are read from the file containing the GameMatch element
	 */
	public void FromMatchDataSetGlobalPlayerData (string match_data_path)
	{

		string matchString = File.ReadAllText (match_data_path);

		GameMatch match_data = JsonUtility.FromJson<GameMatch> (matchString);

		GlobalPlayerData.globalPlayerData.player_data.left_yaw_scale = match_data.left_yaw_scale;
		GlobalPlayerData.globalPlayerData.player_data.right_yaw_scale = match_data.right_yaw_scale;
		GlobalPlayerData.globalPlayerData.player_data.left_pitch_scale = match_data.left_pitch_scale;
		GlobalPlayerData.globalPlayerData.player_data.right_pitch_scale = match_data.right_pitch_scale;

	}

	//extracts the HandAngle enum value from the file containing the GameMatch element
	public GameMatch.HandAngle FromMatchDataToHandAngle (string match_data_path)
	{

		string matchString = File.ReadAllText (match_data_path);

		GameMatch match_data = JsonUtility.FromJson<GameMatch> (matchString);

		return match_data.handAngle;

	}

	//used to extract the path of the music game level (it is inside the assets in local)
	public string FromMatchDataToMusicFilePath (string match_data_path)
	{

		// the name of the level i'm searching for
		string music_name = FromMatchDataToLevelName (match_data_path);

		string music_path = Path.Combine (music_dataPath, music_name + ".txt");

		return music_path;

	}

	//used to extract the path of the any game level that uses the Persistent Data Path
	public string FromMatchDataToLevelFilePath (string match_data_path, GameMatch.GameType g_type)
	{
		
		//the directory of all the levels of the g_type game
		string game_level_paths_directory = Path.Combine (directoryPath, g_type.ToString ());

		// the name of the level i'm searching for
		string level_name = FromMatchDataToLevelName (match_data_path);

		//if (Directory.Exists (game_level_paths_directory)) {

		//find all the levels with that level_name part of whole path name
		string[] game_paths = Directory.GetFiles (game_level_paths_directory, 
			                      g_type.ToString () + "_" + FromNameToFilename (level_name) + "_*.json");

		int index = 0;


		for (int i = 0; i < game_paths.Length; i++) {
			//check for the level with the correspondent name inside the directory of levels
			string i_level_name = Path.GetFileName (game_paths [i]).Split ('_') [1];
			if (i_level_name.Equals (FromNameToFilename (level_name))) {
				index = i;
			}
						
		}

		Debug.Log (game_paths [index]);
		return game_paths [index];
			
			
	}
}
