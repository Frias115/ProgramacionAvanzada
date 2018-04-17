using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontRotate : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(Vector3.zero);
	}
}
