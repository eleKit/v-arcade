using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTargetScript : MonoBehaviour
{
	//target speed
	[Header ("Target movement speed")]
	[Range (0f, 30f)]
	public float speed = 0.5f;

	//if the target is a stopping target
	public bool stop = false;


	public float stop_x_coord = 0f;


	float min_x_coord = -10;

	/* the targets must appear in the scene in specific order, since they are always moving
	 * the correct order depends on the position where they are instantiated
	 */


	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (stop && transform.position.x < stop_x_coord) {
		} else if (transform.position.x < min_x_coord) {
			Destroy (this);
		} else {
			transform.position = new Vector3 (
				transform.position.x - (Time.deltaTime * speed),
				transform.position.y,
				transform.position.z);
		}
		
	}
}
