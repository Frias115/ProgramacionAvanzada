using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerNoPooling : MonoBehaviour {
	Rigidbody2D rb;
	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Start () {
		var angV = Random.Range (-1f, 1f);
		rb.velocity = new Vector2 (6, angV);
		rb.angularVelocity = angV * 15;
	}
}
