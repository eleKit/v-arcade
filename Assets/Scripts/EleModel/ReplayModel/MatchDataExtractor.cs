using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MatchDataExtractor : MonoBehaviour
{
	string directoryPath;

	//music levels data path
	const string music_folder_name = "MusicTexts";
	//standard levels data path
	const string standard_levels_folder_name = "LevelsTexts";


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
		/* N.B. the level name is an attribute of the GameMatch class, 
		 * there is no difference between a standard level and a training one
		 */

		string matchString = File.ReadAllText (match_data_path);

		GameMatch match_data = JsonUtility.FromJson<GameMatch> (matchString);

		return match_data.id_path;

	}


	//returns true if the MatchData refers to a Training level
	public bool CheckFromMatchDataIfTrainingLevel (string match_data_path)
	{
		/* N.B. the level type is an attribute of the GameMatch class, 
		 * there is no difference between a standard level and a training one
		 */

		string matchString = File.ReadAllText (match_data_path);

		GameMatch match_data = JsonUtility.FromJson<GameMatch> (matchString);

		return (match_data.levelType.Equals (GameMatch.LevelType.Training));

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

		//The music path is loaded with Resource.Load method that doesn't accept the .txt
		string music_path = Path.Combine (music_folder_name, music_name);

		return music_path;

	}

	//used to extract the path of the any game level that uses the Persistent Data Path
	/* in case of a standard level the name is taken from a folder in the assets
	 * otherwise if it is a training level it is taken from the persistent data path
	 */
	public string FromMatchDataToLevelFilePath (string match_data_path, GameMatch.GameType g_type)
	{
		string game_level_paths_directory = "";

		// the name of the level I'm searching for
		string level_name = FromMatchDataToLevelName (match_data_path);


		if (CheckFromMatchDataIfTrainingLevel (match_data_path)) {
			//the Training directory of all the levels of the g_type game
			game_level_paths_directory = Path.Combine (directoryPath, g_type.ToString ());


			/* find all the levels with that level_name part of whole path name:
			 * it may happen that a level title is contain in a different longer level title
			 */
			string[] game_paths = Directory.GetFiles (game_level_paths_directory, 
				                      g_type.ToString () + "_" + FromNameToFilename (level_name) + "_*.json");

			//search for the file with the exact level name
			int index = 0;


			//TODO check if this is useful!!
			for (int i = 0; i < game_paths.Length; i++) {
				//check for the level with the correspondent name inside the directory of levels
				string i_level_name = Path.GetFileName (game_paths [i]).Split ('_') [1];
				if (i_level_name.Equals (FromNameToFilename (level_name))) {
					index = i;
				}

			}

		
			return game_paths [index];

		} else {
			//the Standard directory of all the levels of the g_type game
			game_level_paths_directory = Path.Combine (standard_levels_folder_name, g_type.ToString ());

			//The Standard path is loaded with Resource.Load method that doesn't accept the .json
			string standard_path = Path.Combine (game_level_paths_directory, g_type.ToString () + "_" + FromNameToFilename (level_name));
			Debug.Log (standard_path);
			return standard_path;

		}



			
			
	}
}
