using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        [HideInInspector] public int waveQuota;
        public float spawnInterval;
        [HideInInspector] public int spawnCount;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        private string enemyName;
        public int enemyCount;
        [HideInInspector] public int spawnCount;
        public GameObject enemyPrefab;
    }
    
    public List<Wave> waves = new List<Wave>();
    public int currentWaveCount = 0;
    
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform statueTransform;
    
    public List<Transform> spawnPoints = new List<Transform>();

    private float spawnTimer;
    [SerializeField] private float waveInterval;

    private void Start()
    {
        CalculateWaveQuota();
    }

    private void Update()
    {
        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0)
        {
            StartCoroutine(BeginNextWave());
        }
        
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            SpawnEnemies();
            spawnTimer = 0f;
        }
    }
    
    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(waveInterval);
        if (currentWaveCount < waves.Count - 1)
        {
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    private void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }
        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.Log(currentWaveCount);
    }

    private void SpawnEnemies()
    {
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    Debug.Log(spawnPoints);
                    Vector2 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
                    var spawnedEnemy = Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);
                    SetEnemyTarget(spawnedEnemy);
                    
                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                }
            }
        }
    }

    private void SetEnemyTarget(GameObject enemy)
    {
        AIDestinationSetter aiDestinationSetter = enemy.GetComponent<AIDestinationSetter>();
        if (!aiDestinationSetter) return;
        if (!playerTransform) aiDestinationSetter.target = statueTransform;
        if (!statueTransform) aiDestinationSetter.target = playerTransform;
        aiDestinationSetter.target = Random.Range(0f, 1f) <= 0.5f ? playerTransform : statueTransform;
    }
}
