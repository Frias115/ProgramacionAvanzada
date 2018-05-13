using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    public GameObject bossPrefab;
    public GameObject[] wavePrefabs;
    public int numberOfWaves = 5;
    public float timeBetweenWaves = 2.5f;
    public int numberOfWavesAtTheSameTime = 1;


    private float nextWaveTime = 0.0f;
    private float timeForBoss = 12f;
    private int _numberOfWaves;
    private EnemyController[] currentWaveToSpawn;
    private GameObject boss;
    private bool bossTime = true;
    private int bossHealth;
    
    void Start() {
        _numberOfWaves = numberOfWaves;
    }

    void Update () {
        if (_numberOfWaves > 0)
        {
            if (nextWaveTime > timeBetweenWaves)
            {
                for (int i = 0; i < numberOfWavesAtTheSameTime; i++)
                {
                    int rand = Random.Range(0, wavePrefabs.Length);
                    currentWaveToSpawn = wavePrefabs[rand].GetComponentsInChildren<EnemyController>();
                    for (int j = 0; j < currentWaveToSpawn.Length; j++)
                    {
                        Instantiate(currentWaveToSpawn[j], currentWaveToSpawn[j].transform.position, currentWaveToSpawn[j].transform.rotation);
                    }
                    _numberOfWaves--;
                }
                nextWaveTime = 0;
            }

            nextWaveTime += Time.deltaTime;
        }
        else
        {
            if (bossTime && nextWaveTime > timeForBoss)
            {
                boss = Instantiate(bossPrefab);
                bossHealth = boss.GetComponent<BossController>().GetHealth();
                nextWaveTime = 0;
                bossTime = false;
            }

            if(boss != null && nextWaveTime > 1f)
            {
                bossHealth = boss.GetComponent<BossController>().GetHealth();
            }

            if (!bossTime && bossHealth <= 0 && nextWaveTime > 1f)
            {
                bossTime = true;
                _numberOfWaves = numberOfWaves;
                nextWaveTime = 0;
            }

            nextWaveTime += Time.deltaTime;
        }
    }
}
