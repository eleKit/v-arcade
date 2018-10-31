using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpacePitchGesture : MonoBehaviour
{

	/* Space Gesture: a sliding movement made by all the fingers and the palm 
	 * The wrist is not moved and the palm goes up and down (like the pitch part of the shooting gesture)
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
	public int num_frames_in_pitch_average_list = 6;

	//spaceship movements speed
	[Range (0f, 30f)]
	public float speed = 10f;


	private Color transparent_white;
	private Color medium_white;
	private Color solid_white;

	private HandController hc;
	private LinkedList<float> pitch_average = new LinkedList<float> ();

	private int frames_since_last_reconnection;


	// In order to recognize the gesture a minimum angle should be done by the hand movement in RAD
	private float left_pitch_threshold = -15f;
	private float right_pitch_threshold = -15f;

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
	public void PitchStart (HandController handController, Color t_white, Color m_white, Color s_white)
	{

		m_animator = this.GetComponent<Animator> ();
		m_renderer = this.GetComponent<SpriteRenderer> ();

		frames_since_last_reconnection = 0;


		left_pitch_threshold = -GlobalPlayerData.globalPlayerData.player_data.left_pitch_scale;
		right_pitch_threshold = -GlobalPlayerData.globalPlayerData.player_data.right_pitch_scale;
		hc = handController;

		transparent_white = t_white;
		medium_white = m_white;
		solid_white = s_white;
	}

	// Update is called once per frame
	public void PitchUpdate ()
	{
		if (hc.GetFrame ().Hands.Count == 1) {


			//change pointer colour
			if (m_renderer.color.Equals (transparent_white)) {
				m_renderer.color = medium_white;
			}

			frames_since_last_reconnection++;	


			//save average list
			if (pitch_average.Count >= num_frames_in_pitch_average_list && frames_since_last_reconnection >= K_lost) {

				CheckMoveSpaceshipGesture (hc.GetFrame ().Hands.Leftmost.IsLeft);

				//change pointer colour
				if (m_renderer.color.Equals (medium_white)) {
					m_renderer.color = solid_white;
				}
			}
			if (pitch_average.Count >= num_frames_in_pitch_average_list) {

				pitch_average.RemoveFirst ();
			}

			pitch_average.AddLast (hc.GetFrame ().Hands.Leftmost.Direction.Pitch);




		} else {
			//if no hand is visible change colour in black
			m_renderer.color = transparent_white;
			pitch_average.Clear ();

			frames_since_last_reconnection = 0;
		}
	}









	void CheckMoveSpaceshipGesture (bool is_left)
	{
		float threshold;

		if (is_left) {
			threshold = left_pitch_threshold;
		} else {
			threshold = right_pitch_threshold;
		}

		float current_pitch = pitch_average.Average ();

		//move left
		if (current_pitch < threshold) {


			if ((transform.position.x + (Vector3.right * Time.smoothDeltaTime * speed).x) >= x_min_player_posiion) {

				transform.Translate (Vector3.right * Time.smoothDeltaTime * speed);

				//if spaceship is not already moving left the left-movement animation starts
				if (!(m_animator.GetInteger ("direction") == 2)) {
					m_animator.SetInteger ("direction", 2);
				}
			}

			//move right
		} else if (current_pitch > (-threshold)) {


			if ((transform.position.x + (Vector3.left * Time.smoothDeltaTime * speed).x) <= x_max_player_position) {

				transform.Translate (Vector3.left * Time.smoothDeltaTime * speed);

				//if spaceship is not already moving right the right-movement animation starts
				if (!(m_animator.GetInteger ("direction") == 1)) {
					m_animator.SetInteger ("direction", 1);
				}
			}


		}
	}
}
