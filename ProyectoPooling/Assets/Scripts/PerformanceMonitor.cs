using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceMonitor : MonoBehaviour {
	private Text text;
	private float worstFrameTime;
	private int frameCount;

	// Use this for initialization
	void Awake () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		frameCount++;

		if (frameCount > 10) {
			var lastFrameTime = Time.deltaTime;
			if (lastFrameTime > worstFrameTime) {
				worstFrameTime = lastFrameTime;
			}
		}

		text.text = "Worst: " + worstFrameTime + "\nAverage: " + Time.timeSinceLevelLoad/frameCount;
	}
}
