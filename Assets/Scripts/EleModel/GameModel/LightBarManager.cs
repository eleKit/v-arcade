using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightBarManager : MonoBehaviour
{


	public HandController hc;

	public Image colour_line;

	// Use this for initialization
	void Start ()
	{
		colour_line.color = Color.green;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{

		if (colour_line.color.Equals (Color.green)) {

			if (hc.GetFixedFrame ().Hands.Count == 0) {
				colour_line.color = Color.red;
			} else if (hc.GetFixedFrame ().Hands.Count == 1 && GameManager.Instance.music) {
				colour_line.color = Color.red;
			}
		}

		if (colour_line.color.Equals (Color.red)) {

			if (hc.GetFixedFrame ().Hands.Count == 2 && GameManager.Instance.music) {
				colour_line.color = Color.green;
			} else if (hc.GetFixedFrame ().Hands.Count == 1
			           && (GameManager.Instance.shooting || GameManager.Instance.car)) {
				colour_line.color = Color.green;
			}
		}
	}
			
}
