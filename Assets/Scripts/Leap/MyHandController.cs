using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class MyHandController : HandController
{
	public Vector3 leftPalmRotation = Vector3.zero;
	public Vector3 rightPalmRotation = Vector3.zero;
	public int visibleHands = 0;
	public bool rightHandVisible, leftHandVisible;
	public HandList hands;
	public Hand leftHand, rightHand;

	void Update()
	{
		base.Update();

		if (!SaveInfos.replay)
		{
			if (leap_controller_ == null)
				return;

			Frame frame = leap_controller_.Frame();
			UpdateHandModels(hand_graphics_, frame.Hands, leftGraphicsModel, rightGraphicsModel);
			hands = frame.Hands;
			visibleHands = hands.Count;

			switch (visibleHands)
			{
				case 0:
					leftHandVisible = false;
					rightHandVisible = false;
					leftHand = null;
					rightHand = null;
					break;
				case 1:
					Hand hand = frame.Hands[0];
					if (hand.IsLeft)
					{
						leftHandVisible = true;
						rightHandVisible = false;
						leftHand = hand;
					}
					if (hand.IsRight)
					{
						leftHandVisible = false;
						rightHandVisible = true;
						rightHand = hand;
					}
					break;
				case 2:
					leftHandVisible = true;
					rightHandVisible = true;
					leftHand = frame.Hands.Leftmost;
					rightHand = frame.Hands.Rightmost;
					break;
			}
		}
	}

	void FixedUpdate()
	{
		base.FixedUpdate();

		if (!SaveInfos.replay)
		{
			if (leap_controller_ == null)
				return;

			Frame frame = leap_controller_.Frame();
			UpdateHandModels(hand_physics_, frame.Hands, leftPhysicsModel, rightPhysicsModel);
			UpdateHandModels(hand_graphics_, frame.Hands, leftGraphicsModel, rightGraphicsModel);
		}
	}

	public void SetLeftPalmRotation(Vector3 rot)
	{
		leftPalmRotation = rot;
	}

	public void SetRightPalmRotation(Vector3 rot)
	{
		rightPalmRotation = rot;
	}

	public void SetLeftHandVisible(bool x)
	{
		leftHandVisible = x;
	}

	public void SetRightHandVisible(bool x)
	{
		rightHandVisible = x;
	}
}