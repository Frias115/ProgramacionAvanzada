using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPooling : MonoBehaviour
{
    public int numberOfObjectsInPool = 10;
    public GameObject[] prefabs;

    private List<GameObject> spawnedObjects;

    void Awake()
    {
        spawnedObjects = new List<GameObject>();
    }

    void Start()
    {
        foreach (var prefab in prefabs)
        {
            PoolManager.Load(prefab, numberOfObjectsInPool / prefabs.Length);
        }
        StartCoroutine(DespawnCoroutine());
    }

    public GameObject Instantiate(Vector3 position)
    {
        GameObject go;
        go = PoolManager.Spawn(prefabs[Random.Range(0, prefabs.Length)], position);
        spawnedObjects.Add(go);
        return go;
    }

    private IEnumerator DespawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            var newSpawnedObjects = new List<GameObject>();
            foreach (var spawnedObject in spawnedObjects)
            {
                newSpawnedObjects.Add(spawnedObject);
            }
            spawnedObjects = newSpawnedObjects;
        }
    }

}
