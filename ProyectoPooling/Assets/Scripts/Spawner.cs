using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
	public GameObject[] prefabs;
	public List<GameObject> spawnedObjects;
	public int objectsPerFrame = 1;

	void Awake () {
		spawnedObjects = new List<GameObject> ();
	}

	// Use this for initialization
	void Start () {
		StartCoroutine (DespawnCoroutine());
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < objectsPerFrame; i++) {
			var go = GameObject.Instantiate (prefabs [Random.Range (0, prefabs.Length)]);
			go.transform.position = transform.position + Vector3.up * .6f;
			spawnedObjects.Add(go);
		}
	}

	private IEnumerator DespawnCoroutine() {
		while (true) {
			yield return new WaitForSeconds (5);
			var newObjects = new List<GameObject> ();
			foreach (var spawnedObject in spawnedObjects) {
				if (spawnedObject.transform.position.y < -100 || spawnedObject.transform.position.x > 10) {
					GameObject.Destroy (spawnedObject);
				} else {
					newObjects.Add (spawnedObject);
				}
			}
			spawnedObjects = newObjects;
		}
	}
}
