using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightBarManager : MonoBehaviour
{
	/* this script is used in all the games (not in the tuning!) to show if the hands are correctly seen by the leap or not
	 */

	public HandController hc;

	public Image colour_line;

	//current game playing
	GameMatch.GameType game_type;

	// Use this for initialization
	void Start ()
	{
		colour_line.color = Color.green;
		game_type = GameManager.Instance.GetCurrentGameType ();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{

		if (colour_line.color.Equals (Color.green)) {

			if (hc.GetFixedFrame ().Hands.Count == 0) {
				colour_line.color = Color.red;
			} else if (hc.GetFixedFrame ().Hands.Count == 1 && game_type.Equals (GameMatch.GameType.Music)) {
				colour_line.color = Color.red;
			}
		}

		if (colour_line.color.Equals (Color.red)) {

			if (hc.GetFixedFrame ().Hands.Count == 2 && game_type.Equals (GameMatch.GameType.Music)) {
				colour_line.color = Color.green;
			} else if (hc.GetFixedFrame ().Hands.Count == 1
			           && (!game_type.Equals (GameMatch.GameType.Music))) {
				colour_line.color = Color.green;
			}
		}
	}
			
}
