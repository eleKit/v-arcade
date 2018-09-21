using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileNamesOfPaths
{

	/* the name is the name of the Game Path, it used as the text inside the button in the game UI menu
	 * and it is saved inside the GameManager (and then saved inside the GameMatch element saved in the NameGame_Ts.json
	 * 
	 * the file_path is the complete file path of the file containing all the game path data 
	 * to generate a level (the game path designed by the doctor with the PathGenerator scene)
	 */
	public string file_path;

	public string name;
}
