using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.Linq;

public class PushGesture : MonoBehaviour
{

	/* push gesture: a tapping movement made by all the fingers and the palm
	 * substitutes the old jump gesture
	 */

	/* TODO vi è una differenza: 
	* il palmo deve essere sempre nella stessa posizione e le dita si muovono
	* oppure
	* il palmo si muove co le dita?
	* previa verifica io ho scelto la prima opzione
	*/


	//Poiché si vuole ricreare idealmente il funzionamento di un bottone che mandi un singolo
	//impulso quando attivato (e non uno continuato, come ad esempio quello della tastiera)
	//si controlla che, dopo aver riconosciuto un movimento, la mano sia tornata in una posizione
	// nell'intorno (-minOffsetToRelease, +minOffsetTorelease), prima di poter riconoscere nuovamente lo stesso movimento
	static float minOffsetToRelease = 5f;

	[Range (-5f, 0f)]
	public float offset = -0.2f;

	[Range (-5f, 0f)]
	public float threshold = -0.5f;

	[Range (0, 20)]
	public int K = 10;

	//the hand controller prefab in the scene
	public GameObject handController;


	private HandController hc;

	//save the past K frame used to check if a push gesture happens
	private LinkedList<float> left_pitch = new LinkedList<float> ();
	private LinkedList<float> right_pitch = new LinkedList<float> ();


	// Use this for initialization
	void Start ()
	{
		hc = handController.GetComponent<HandController> ();
		if (hc.GetFrame ().Hands.Count == 2) {
			if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
				left_pitch.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
			}
			if (hc.GetFrame ().Hands.Rightmost.IsRight) {
				right_pitch.AddLast (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
			}
		}

		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (hc.GetFrame ().Hands.Count == 2) {
			//save left hand pitch and check left push gesture
			if (hc.GetFrame ().Hands.Leftmost.IsLeft) {
				if (left_pitch.Count >= K) {
					CheckLeftPushGesture (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
					left_pitch.RemoveFirst ();
				}
				left_pitch.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);
			}

			//save right hand pitch and check right push gesture
			if (hc.GetFrame ().Hands.Rightmost.IsRight) {
				if (right_pitch.Count >= K) {
					CheckRightPushGesture (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
					right_pitch.RemoveFirst ();
				}
				right_pitch.AddLast (hc.GetFrame ().Hands.Rightmost.Direction.Pitch);
			}
				
		}
		//TODO fare i controlli se le due mani non sono viste o ne è vista una sola



	}

	void CheckLeftPushGesture (float pitch)
	{
		//search for the max pitch in the previous K frames
		float max_pitch = left_pitch.Max ();
		if ((pitch - max_pitch) < threshold && pitch < offset) {
			Debug.Log ("left push" + " max " + max_pitch.ToString ());
		}



			
	}

	void CheckRightPushGesture (float pitch)
	{
		//search for the max pitch in the previous K frames
		float max_pitch = right_pitch.Max ();
		if ((pitch - max_pitch) < threshold && pitch < offset) {
			Debug.Log ("right push" + " max " + max_pitch.ToString ());
		}
		
	}


}
