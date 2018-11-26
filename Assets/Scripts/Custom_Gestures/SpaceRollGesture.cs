using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpaceRollGesture : MonoBehaviour
{


	//if connection is lost are waited k_lost frames before starting again to check gestures
	[Range (10, 100)]
	public int K_lost = 30;

	//no more than K previous frames are taken into account
	[Range (0, 20)]
	public int num_frames_in_roll_average_list = 6;

	//spaceship movements speed
	[Range (0f, 30f)]
	public float speed = 10f;


	private Color transparent_white;
	private Color medium_white;
	private Color solid_white;

	private HandController hc;
	private LinkedList<float> roll_average = new LinkedList<float> ();

	private int frames_since_last_reconnection;


	// In order to recognize the gesture a minimum angle should be done by the hand movement in RAD
	private float roll_left_threshold = -0.05f;
	private float roll_right_threshold = -0.05f;

	//The spaceship has a minimum position and a maximum one, it must not escape from the screen
	private float x_max_player_position = 16f;
	private float x_min_player_position = -16f;


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
	public void RollStart (HandController handController, Color t_white, Color m_white, Color s_white)
	{

		m_animator = this.GetComponent<Animator> ();
		m_renderer = this.GetComponent<SpriteRenderer> ();

		frames_since_last_reconnection = 0;


		hc = handController;

		transparent_white = t_white;
		medium_white = m_white;
		solid_white = s_white;
	}

	// Update is called once per frame
	public void RollUpdate ()
	{
		var current_frame = GameManager.Instance.GetCurrentFrame ();
		if (current_frame.Hands.Count == 1) {


			//change pointer colour
			if (m_renderer.color.Equals (transparent_white)) {
				m_renderer.color = medium_white;
			}

			frames_since_last_reconnection++;	


			//save average list
			if (roll_average.Count >= num_frames_in_roll_average_list && frames_since_last_reconnection >= K_lost) {

				CheckMoveSpaceshipGesture (current_frame.Hands.Leftmost.IsLeft);

				//change pointer colour
				if (m_renderer.color.Equals (medium_white)) {
					m_renderer.color = solid_white;
				}
			}
			if (roll_average.Count >= num_frames_in_roll_average_list) {

				roll_average.RemoveFirst ();
			}

			roll_average.AddLast (current_frame.Hands.Leftmost.PalmNormal.Roll);




		} else {
			//if no hand is visible change colour in black
			m_renderer.color = transparent_white;
			roll_average.Clear ();

			frames_since_last_reconnection = 0;
		}
	}


	void CheckMoveSpaceshipGesture (bool is_left)
	{
		float threshold;

		if (is_left) {
			threshold = roll_left_threshold;
		} else {
			threshold = roll_right_threshold;
		}

		float current_pitch = roll_average.Average ();

		//move left
		if (current_pitch < threshold) {


			if ((transform.position.x + (Vector3.right * Time.deltaTime * speed).x) <= x_max_player_position) {

				transform.Translate (Vector3.right * Time.deltaTime * speed);

				//if spaceship is not already moving left the left-movement animation starts
				if (!(m_animator.GetInteger ("direction") == 2)) {
					m_animator.SetInteger ("direction", 2);
				}
			}

			//move right
		} else if (current_pitch > (-threshold)) {


			if ((transform.position.x + (Vector3.left * Time.deltaTime * speed).x) >= x_min_player_position) {

				transform.Translate (Vector3.left * Time.deltaTime * speed);

				//if spaceship is not already moving right the right-movement animation starts
				if (!(m_animator.GetInteger ("direction") == 1)) {
					m_animator.SetInteger ("direction", 1);
				}
			}


		}
	}
}
