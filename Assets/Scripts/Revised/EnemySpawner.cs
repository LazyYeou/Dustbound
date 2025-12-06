using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public string name;
        public GameObject prefab;
        [Range(0f, 100f)] public float spawnWeight = 10f;
        public int minDifficultyLevel = 1;
    }

    [Header("Basic Settings")]
    public Transform player;
    public List<EnemyType> enemyTypes = new List<EnemyType>();

    [Header("Spawn Rate Settings")]
    public float baseInterval = 2f;
    public float minimumInterval = 0.3f;
    public float intervalDecreaseRate = 0.1f;
    private float currentInterval;
    private float timer;

    [Header("Difficulty Scaling")]
    public float difficultyIncreaseInterval = 20f;
    public DifficultyScalingMode scalingMode = DifficultyScalingMode.SpawnRate;
    private float difficultyTimer;
    private int currentDifficultyLevel = 1;

    [Header("Enemy Count Settings")]
    public int baseEnemiesPerSpawn = 1;
    public int maxEnemiesPerSpawn = 5;
    public int enemyIncreasePerLevel = 1;

    [Header("Enemy Stats Scaling")]
    public bool scaleEnemyStats = true;
    public float statMultiplierPerLevel = 1.1f;

    [Header("Spawn Position Settings")]
    public float minSpawnDistance = 10f;
    public float maxSpawnDistance = 15f;
    public bool avoidSpawningNearPlayer = true;
    public float safeZoneRadius = 5f;

    [Header("Visual Feedback")]
    public bool showDifficultyIncrease = true;
    public GameObject difficultyIncreaseEffect;

    [Header("Advanced Patterns")]
    public bool useSpawnWaves = false;
    public int waveSize = 5;
    public float waveCooldown = 5f;
    private bool isInWaveCooldown = false;

    public enum DifficultyScalingMode
    {
        SpawnRate,
        EnemyCount,
        EnemyStats,
        Combined
    }

    void Start()
    {
        currentInterval = baseInterval;
        timer = currentInterval;
        difficultyTimer = difficultyIncreaseInterval;
    }

    void Update()
    {
        if (player == null) return;

        difficultyTimer -= Time.deltaTime;
        if (difficultyTimer <= 0)
        {
            IncreaseDifficulty();
            difficultyTimer = difficultyIncreaseInterval;
        }

        if (isInWaveCooldown)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isInWaveCooldown = false;
                timer = currentInterval;
            }
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (useSpawnWaves)
            {
                SpawnWave();
                timer = waveCooldown;
                isInWaveCooldown = true;
            }
            else
            {
                Spawn();
                timer = currentInterval;
            }
        }
    }

    void Spawn()
    {
        int enemyCount = GetEnemyCount();

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnSingleEnemy();
        }
    }

    void SpawnWave()
    {
        for (int i = 0; i < waveSize; i++)
        {
            SpawnSingleEnemy();
        }
    }

    void SpawnSingleEnemy()
    {
        GameObject prefabToSpawn = GetRandomEnemyPrefab();

        if (prefabToSpawn != null)
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

            ApplyDifficultyModifiers(enemy);
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        List<EnemyType> allowedEnemies = new List<EnemyType>();
        float totalWeight = 0f;

        foreach (var type in enemyTypes)
        {
            if (currentDifficultyLevel >= type.minDifficultyLevel)
            {
                allowedEnemies.Add(type);
                totalWeight += type.spawnWeight;
            }
        }

        if (allowedEnemies.Count == 0) return null;

        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var type in allowedEnemies)
        {
            currentWeight += type.spawnWeight;
            if (randomValue <= currentWeight)
            {
                return type.prefab;
            }
        }

        return allowedEnemies[0].prefab;
    }

    void ApplyDifficultyModifiers(GameObject enemy)
    {
        if (!scaleEnemyStats) return;

        EnemyController stats = enemy.GetComponent<EnemyController>();
        if (stats != null)
        {
            float multiplier = Mathf.Pow(statMultiplierPerLevel, currentDifficultyLevel - 1);

            stats.maxHealth *= multiplier;
            stats.health = stats.maxHealth;

            stats.damageToPlayer *= multiplier;
        }
    }

    void IncreaseDifficulty()
    {
        currentDifficultyLevel++;
        if (scalingMode == DifficultyScalingMode.SpawnRate || scalingMode == DifficultyScalingMode.Combined)
        {
            DecreaseSpawnInterval();
        }

        if (showDifficultyIncrease)
        {
            Debug.Log($"Difficulty Level: {currentDifficultyLevel}");
            if (difficultyIncreaseEffect != null && player != null)
                Instantiate(difficultyIncreaseEffect, player.position, Quaternion.identity);
        }
    }

    void DecreaseSpawnInterval()
    {
        currentInterval = Mathf.Max(minimumInterval, currentInterval - intervalDecreaseRate);
    }

    int GetEnemyCount()
    {
        if (scalingMode == DifficultyScalingMode.EnemyCount || scalingMode == DifficultyScalingMode.Combined)
        {
            int count = baseEnemiesPerSpawn + ((currentDifficultyLevel - 1) * enemyIncreasePerLevel);
            return Mathf.Min(count, maxEnemiesPerSpawn);
        }
        return baseEnemiesPerSpawn;
    }

    Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPos;
        int maxAttempts = 10;
        int attempts = 0;

        do
        {
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            spawnPos = player.position + (Vector3)(randomDir * distance);
            attempts++;
            if (attempts >= maxAttempts) break;

        } while (avoidSpawningNearPlayer && Vector3.Distance(spawnPos, player.position) < safeZoneRadius);

        return spawnPos;
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, minSpawnDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, maxSpawnDistance);
    }
}