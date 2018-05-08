using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMovement : MonoBehaviour
{


	public GameObject m_pointer;

	Transform tr;

	// Use this for initialization
	void Start ()
	{
		tr = gameObject.GetComponent<Transform> ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Mathf.Abs (m_pointer.transform.position.x + 2) <= 8) {
			tr.position = new Vector3 (m_pointer.transform.position.x + 2, tr.position.y, tr.position.z);
		}
		
	}
}
