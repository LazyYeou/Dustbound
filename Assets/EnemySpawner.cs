using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public string name;
        public GameObject prefab;
        [Range(0f, 100f)] public float spawnWeight = 10f; // Higher number = higher chance
        public int minDifficultyLevel = 1; // Only spawn after this level
    }

    [Header("Basic Settings")]
    public Transform player;
    // REPLACED single prefab with a List
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
    public bool scaleEnemyStats = true; // Simplified boolean
    public float statMultiplierPerLevel = 1.1f; // 10% stronger per level

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

        // Difficulty Timer
        difficultyTimer -= Time.deltaTime;
        if (difficultyTimer <= 0)
        {
            IncreaseDifficulty();
            difficultyTimer = difficultyIncreaseInterval;
        }

        // Wave Logic
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

        // Spawn Timer
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

    // --- NEW SPAWN LOGIC ---

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
        // 1. Pick a random enemy based on weights and level
        GameObject prefabToSpawn = GetRandomEnemyPrefab();

        if (prefabToSpawn != null)
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

            // 2. Make them stronger based on level
            ApplyDifficultyModifiers(enemy);
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        // Filter list: Get only enemies allowed at this difficulty level
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

        // Weighted Random Selection
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

        return allowedEnemies[0].prefab; // Fallback
    }

    void ApplyDifficultyModifiers(GameObject enemy)
    {
        if (!scaleEnemyStats) return;

        EnemyController stats = enemy.GetComponent<EnemyController>();
        if (stats != null)
        {
            // Calculate multiplier: 1.1 ^ (Level - 1)
            float multiplier = Mathf.Pow(statMultiplierPerLevel, currentDifficultyLevel - 1);

            // Apply to Max HP
            stats.maxHealth *= multiplier;
            stats.health = stats.maxHealth; // Reset current HP to new max

            // Apply to Damage
            stats.damageToPlayer *= multiplier;

            // Optional: Scale speed slightly less aggressively (half power)
            // stats.speed *= (1 + (multiplier - 1) * 0.5f);
        }
    }

    // --- (Keeping your existing helper methods below) ---

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