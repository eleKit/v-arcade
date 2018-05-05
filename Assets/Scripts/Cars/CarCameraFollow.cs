using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCameraFollow : MonoBehaviour
{
	

	private Transform m_Player;
	// Reference to the player's transform.

	private Vector3 lastPlayerPosition;

	private void Awake ()
	{
		// Setting up the reference.
		m_Player = GameObject.FindGameObjectWithTag ("Player").transform;
		lastPlayerPosition = m_Player.position;
	}


	private void LateUpdate ()
	{

		Vector3 currentPlayerPosition = m_Player.position; //Get current player position
		Vector3 distanceMoved = currentPlayerPosition - lastPlayerPosition; //Figure out how much the player moved since the last frame
		lastPlayerPosition = currentPlayerPosition;

		transform.position = new Vector3 (transform.position.x, transform.position.y + distanceMoved.y, transform.position.z); //Move the camera

	}
}
