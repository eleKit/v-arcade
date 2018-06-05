using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayGameManager : MonoBehaviour
{


	public bool car, music, shooting;

	public GameObject player;

	public Vector3 player_initial_pos;


	public HandController hc;

	public bool is_playing = false;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}




	public void BaseStart (string music_title, GameMatch.GameType game_type)
	{
		switch (game_type) {

		case GameMatch.GameType.Car:
			car = true;
			break;
		case GameMatch.GameType.Shooting:
			shooting = true;
			break;
		case GameMatch.GameType.Music:
			music = true;
			break;
		}

		GameMenuScript.Instance.LoadUIOfGame (game_type);


	}
}
