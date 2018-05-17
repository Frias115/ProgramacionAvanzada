using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, List<GameObject>> pool;
    private Transform poolParent;

    public List<GameObject> prefabsPosibles;

    public int numberOfObjectsInPool = 10;

    void Awake()
    {
        pool = new Dictionary<string, List<GameObject>>();
        poolParent = new GameObject("Pool").transform;

    }

    void Start()
    {
        foreach (var prefab in prefabsPosibles)
        {
            Load(prefab, numberOfObjectsInPool / prefabsPosibles.Count);
        }
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
        try
        {
            if (prefab == null)
            {
                throw new NullPrefabException();
            }

            if (!pool.ContainsKey(prefab.name) || pool[prefab.name].Count == 0)
            {
                throw new EmptyPoolException();
            }
        }
        catch (EmptyPoolException)
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

        go.transform.SetParent(poolParent);
        go.SetActive(false);
        pool[go.name].Add(go);
    }



    //Añadir método para spawnear un prefab en una posición concreta

    public static GameObject Spawn(GameObject prefab, Vector3 posicion)
    {
        GameObject instance = null;

        try
        {
            instance = Instance.SpawnInternal(prefab);
            instance.transform.position = posicion;
        }
        catch (NullPrefabException)
        {
            throw new NullPrefabException();
        }

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

    public static GameObject Instantiate(GameObject prefab, Vector3 position)
    {
        GameObject go = null;

        try
        {
            go = Spawn(prefab, position);
        }
        catch (NullPrefabException)
        {
            throw new NullPrefabException();
        }

        return go;
    }
}