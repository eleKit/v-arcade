using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
	/* RotateCamera is used in the Car game: 
	 * if the player is training the flexion/extension the game camera is rotated 90 deg
	 * and the car moves from left to right
	 * if the player is trainid ulnar/radial dev. the game camera is at 0 and the car moves up
	 */

	[Range (10f, 30f)]
	public float m_rotated_camera_size = 14f;
	public Vector3 m_rotated_camera_position;

	Vector3 original_position;
	Quaternion original_rotation;
	float original_size;


	// Use this for initialization
	void Start ()
	{
		original_rotation = transform.rotation;
		original_position = transform.position;
		original_size = this.GetComponent<Camera> ().orthographicSize;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void Rotate ()
	{
		this.transform.Rotate (Vector3.forward, 90f);
		this.GetComponent<Camera> ().orthographicSize = m_rotated_camera_size;
		this.transform.position = m_rotated_camera_position;

	}

	/* NB: the Reset() function must be called before it is called the Rotate()
	 * because it rotates of 90 deg, 
	 * if it is called 2 times without resetting the total rotation is of 180 deg
	 */

	/* Reset() is called by the "Scegli Prescorso" button in the Intro UI 
	 * and by the "Indietro" button in the CarColour UI
	 */
	public void Reset ()
	{
		this.transform.rotation = original_rotation;
		this.transform.position = original_position;
		this.GetComponent<Camera> ().orthographicSize = original_size;
	}
}
