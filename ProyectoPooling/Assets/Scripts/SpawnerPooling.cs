using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPooling : MonoBehaviour {
	public GameObject[] prefabs;
	public List<GameObject> spawnedObjects;
	public int objectsPerFrame = 1;
	public Vector3 posicion;

	public string nombrePrefabASpawnear;

	void Awake () {
		spawnedObjects = new List<GameObject> ();
	}

	// Use this for initialization
	void Start () {
		foreach (var prefab in prefabs) {
			PoolManager.Load (prefab, 600/prefabs.Length);
		}
		StartCoroutine (DespawnCoroutine());
	}

	// Update is called once per frame
	void Update () {
		for (int i = 0; i < objectsPerFrame; i++) {
			GameObject go;
			if ( nombrePrefabASpawnear == ""){
				go = PoolManager.Spawn (prefabs [Random.Range (0, prefabs.Length)], posicion);
			} else{
				go = PoolManager.Spawn (nombrePrefabASpawnear, posicion);
			}
			//go.transform.position = transform.position + Vector3.up * .6f;
			spawnedObjects.Add (go);
		}
	}

	private IEnumerator DespawnCoroutine() {
		while (true) {
			yield return new WaitForSeconds (5);
			var newSpawnedObjects = new List<GameObject> ();
			foreach (var spawnedObject in spawnedObjects) {
				if (spawnedObject.transform.position.y < -100 || spawnedObject.transform.position.x > 10) {
					PoolManager.Despawn (spawnedObject);
				} else {
					newSpawnedObjects.Add (spawnedObject);
				}
			}
			spawnedObjects = newSpawnedObjects;
		}
	}

}
