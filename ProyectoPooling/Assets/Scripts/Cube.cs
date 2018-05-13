using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {
	private Rigidbody rb;
	private MeshRenderer[] renderers;

	void Awake () {
		rb = GetComponent<Rigidbody> ();	
		renderers = GetComponentsInChildren<MeshRenderer> ();
	}

	// Use this for initialization
	void Start () {
		var targetColor = new Color(Random.value, Random.value, Random.value);
		foreach (var mr in renderers) {
			mr.material.color = targetColor;
		}
		rb.AddForce (new Vector3(Random.Range(-10f, 10f), Random.Range(0, 10f), Random.Range(-10f, 10f)).normalized * Random.Range(3f, 10f), ForceMode.Impulse);
		rb.AddTorque (new Vector3(Random.Range(-10f, 10f), Random.Range(-10, 10f), Random.Range(-10f, 10f)).normalized * Random.Range(3f, 10f), ForceMode.Impulse);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
