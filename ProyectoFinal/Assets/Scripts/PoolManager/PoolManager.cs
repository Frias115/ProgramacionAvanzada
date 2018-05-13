using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, List<GameObject>> pool;
    private Transform poolParent;

    public List<GameObject> prefabsPosibles;

    void Awake()
    {
        pool = new Dictionary<string, List<GameObject>>();
        poolParent = new GameObject("Pool").transform;
    }

    public static void Load(GameObject prefab, int quantity = 1)
    {
        Instance.LoadInternal(prefab, quantity);
    }

    public static void Despawn(GameObject go)
    {
        Instance.DespawnInternal(go);
    }

    public static GameObject Spawn(GameObject prefab)
    {
        return Instance.SpawnInternal(prefab);
    }

    private void LoadInternal(GameObject prefab, int quantity = 1)
    {
        var goName = prefab.name;

        if (!pool.ContainsKey(goName))
        {
            pool[goName] = new List<GameObject>();
        }

        for (int i = 0; i < quantity; i++)
        {
            var go = Instantiate(prefab);
            go.name = goName;

            go.transform.SetParent(poolParent);
            go.SetActive(false);
            pool[goName].Add(go);
        }
    }

    private GameObject SpawnInternal(GameObject prefab)
    {
        if (!pool.ContainsKey(prefab.name) || pool[prefab.name].Count == 0)
        {
            Debug.LogWarning("Requested item " + prefab.name + " but it's pool was empty");
            Load(prefab, 1);
        }

        var l = pool[prefab.name];
        var go = l[0];
        l.RemoveAt(0);
        go.SetActive(true);
        go.transform.SetParent(null);
        return go;
    }

    private void DespawnInternal(GameObject go)
    {
        if (!pool.ContainsKey(go.name))
        {
            pool[go.name] = new List<GameObject>();
        }

        go.SetActive(false);
        go.transform.SetParent(poolParent);
        pool[go.name].Add(go);
    }



    //Añadir método para spawnear un prefab en una posición concreta

    public static GameObject Spawn(GameObject prefab, Vector3 posicion)
    {
        var instance = Instance.SpawnInternal(prefab);
        instance.transform.position = posicion;
        return instance;
    }

    //Añadir un método que nos diga cuántas instancias de un prefab tenemos disponibles en el pool

    public int GetNumeroPrefabs(string prefab)
    {
        return pool[prefab].Count;
    }

    //Añadir método que nos devuelva la lista de todas las instancias disponibles de un prefab

    public List<GameObject> GetListOfPrefabs(string prefab)
    {
        return pool[prefab];
    }

    //Añadir métodos que nos permitan spawnear objetos por nombre

    public static GameObject Spawn(string prefab, Vector3 posicion)
    {
        var instance = Instance.SpawnInternal(prefab);
        instance.transform.position = posicion;
        return instance;
    }

    private GameObject SpawnInternal(string nombrePrefab)
    {
        GameObject prefab = null;
        foreach (GameObject item in prefabsPosibles)
        {
            if (item.name == nombrePrefab)
            {
                prefab = item;
            }
        }

        if (prefab == null)
        {
            return null;
        }

        if (!pool.ContainsKey(prefab.name) || pool[prefab.name].Count == 0)
        {
            Debug.LogWarning("Requested item " + prefab.name + " but it's pool was empty");
            Load(prefab, 1);
        }

        var l = pool[prefab.name];
        var go = l[0];
        l.RemoveAt(0);
        go.SetActive(true);
        go.transform.SetParent(null);
        return go;
    }
}