using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.Linq;

public class SpaceGesture : MonoBehaviour
{

	/* Space Gesture: a sliding movement made by all the fingers and the palm 
	 * The wrist is not moved and the palm goes left and right (like the yaw part of the shooting gesture)
	 * 
	 * N.B. it can be done one gesture per second
	 */

	/* used average method seen in Leap Motion Forum topic: custom gesture cration guide
	* https://forums.leapmotion.com/t/creating-my-own-gesture/603/7
	*/



	//if connection is lost are waited k_lost frames before starting again to check gestures
	[Range (10, 100)]
	public int K_lost = 30;

	//no more than K previous frames are taken into account
	[Range (0, 20)]
	public int num_frames_in_yaw_average_list = 6;

	//spaceship movements speed
	[Range (0f, 30f)]
	public float speed = 10f;

	//spaceship movements delta
	[Range (0, 10)]
	public float x_movement = 1;


	[Header ("Colors used when leap does/does not see the hands")]
	public Color transparent_white = new Color (1f, 1f, 1f, 0.2f);
	public Color medium_white = new Color (1f, 1f, 1f, 0.5f);
	public Color solid_white = new Color (1f, 1f, 1f, 1f);

	private HandController hc;
	private LinkedList<float> yaw_average = new LinkedList<float> ();

	private int frames_since_last_reconnection;


	// In order to recognize the gesture a minimum angle should be done by the hand movement in RAD
	private float left_yaw_threshold = -15f;
	private float right_yaw_threshold = -15f;

	//The spaceship has a minimum position and a maximum one, it must not escape from the screen
	private float x_max_player_position = 16f;
	private float x_min_player_posiion = -16f;


	/* spaceship movements animator:
	 * direction = 1 left movement
	 * direction = 2 right movement
	 * there are 2 animators set with the CarColorUI script, one for the blue spaceship, one for the red spaceship
	 */
	private Animator m_animator;

	/* set transparent_white if no hand is visible
	 * set medium_white if the hand is back visible in that moment
	 * set solid_white if the gesture recognizer is active in checking gestures
	 */
	private SpriteRenderer m_renderer;




	// Use this for initialization
	public void YawStart (HandController hand_controller)
	{
		m_animator = this.GetComponent<Animator> ();
		m_renderer = this.GetComponent<SpriteRenderer> ();

		frames_since_last_reconnection = 0;

		//retreive hand thresholds
		left_yaw_threshold = -GlobalPlayerData.globalPlayerData.player_data.left_yaw_scale;
		right_yaw_threshold = -GlobalPlayerData.globalPlayerData.player_data.right_yaw_scale;

		hc = hand_controller;

	}



	//method called by the SpaceGestureREcognizerManager
	public void YawUpdate ()
	{
		if (hc.GetFrame ().Hands.Count == 1) {


			//change pointer colour
			if (m_renderer.color.Equals (transparent_white)) {
				m_renderer.color = medium_white;
			}

			frames_since_last_reconnection++;	


			//save average list
			if (yaw_average.Count >= num_frames_in_yaw_average_list && frames_since_last_reconnection >= K_lost) {

				CheckMoveSpaceshipGesture (hc.GetFrame ().Hands.Leftmost.IsLeft);

				//change pointer colour
				if (m_renderer.color.Equals (medium_white)) {
					m_renderer.color = solid_white;
				}
			}
			if (yaw_average.Count >= num_frames_in_yaw_average_list) {

				yaw_average.RemoveFirst ();
			}

			yaw_average.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Yaw);




		} else {
			//if no hand is visible change colour in black
			m_renderer.color = transparent_white;
			yaw_average.Clear ();

			frames_since_last_reconnection = 0;
		}
	}




	void CheckMoveSpaceshipGesture (bool is_left)
	{
		float threshold;

		if (is_left) {
			threshold = left_yaw_threshold;
		} else {
			threshold = right_yaw_threshold;
		}

		float current_yaw = yaw_average.Average ();

		//move left
		if (current_yaw < threshold) {


			if ((transform.position.x + (Vector3.left * Time.smoothDeltaTime * speed).x) >= x_min_player_posiion) {

				transform.Translate (Vector3.left * Time.smoothDeltaTime * speed);

				//if spaceship is not already moving left the left-movement animation starts
				if (!(m_animator.GetInteger ("direction") == 1)) {
					m_animator.SetInteger ("direction", 1);
				}
			}

			//move right
		} else if (current_yaw > (-threshold)) {
			

			if ((transform.position.x + (Vector3.right * Time.smoothDeltaTime * speed).x) <= x_max_player_position) {

				transform.Translate (Vector3.right * Time.smoothDeltaTime * speed);

				//if spaceship is not already moving right the right-movement animation starts
				if (!(m_animator.GetInteger ("direction") == 2)) {
					m_animator.SetInteger ("direction", 2);
				}
			}


		}
	}


}
