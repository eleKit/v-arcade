using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayNamesOfPaths
{

	/* to load a replay the hand data recordings and the game path chosen by the player are fundamental:
	 * the hand data are saved in the NameGame_TS_hand_data.json and the game path is an attribute in the GameMatch class,
	 * an entity of that class is saved in the NameGame_TS.json file
	 */

	public string hand_data_path;

	public string button_name;

	public string match_data_path;
}
