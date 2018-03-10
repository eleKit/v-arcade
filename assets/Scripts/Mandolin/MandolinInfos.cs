using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MandolinInfos{

	public static float buttonSpeed = 17;
	public static float leftGuideX = -10f;
	public static float rightGuideX = 10f;
	public static float centerY = -13.75f;
	public static float buttonZ = -2f;
	public static bool leftGesture, rightGesture, usingPower, start;

	public static int score = 0;
	public static int targetScore = 20; 

	public static List<string> songs;
}
