using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Basic Settings")]
    public GameObject enemyPrefab;
    public Transform player;

    [Header("Spawn Rate Settings")]
    public float baseInterval = 2f;
    public float minimumInterval = 0.3f; // Fastest spawn rate
    public float intervalDecreaseRate = 0.1f; // How much to decrease each difficulty increase
    private float currentInterval;
    private float timer;

    [Header("Difficulty Scaling")]
    public float difficultyIncreaseInterval = 20f; // Time between difficulty increases
    public DifficultyScalingMode scalingMode = DifficultyScalingMode.SpawnRate;
    private float difficultyTimer;
    private int currentDifficultyLevel = 1;

    [Header("Enemy Count Settings")]
    public int baseEnemiesPerSpawn = 1;
    public int maxEnemiesPerSpawn = 5;
    public int enemyIncreasePerLevel = 1; // How many more enemies per difficulty level

    [Header("Enemy Stats Scaling")]
    public bool scaleEnemyHealth = true;
    public float healthMultiplierPerLevel = 1.15f; // 15% more health per level
    public bool scaleEnemySpeed = true;
    public float speedMultiplierPerLevel = 1.1f; // 10% faster per level
    public bool scaleEnemyDamage = true;
    public float damageMultiplierPerLevel = 1.1f;

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
        SpawnRate,          // Spawn enemies faster
        EnemyCount,         // Spawn more enemies at once
        EnemyStats,         // Make enemies stronger
        Combined            // All of the above
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

        // Difficulty progression timer
        difficultyTimer -= Time.deltaTime;
        if (difficultyTimer <= 0)
        {
            IncreaseDifficulty();
            difficultyTimer = difficultyIncreaseInterval;
        }

        // Wave cooldown
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

        // Spawn timer
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

    void IncreaseDifficulty()
    {
        currentDifficultyLevel++;

        switch (scalingMode)
        {
            case DifficultyScalingMode.SpawnRate:
                DecreaseSpawnInterval();
                break;
            case DifficultyScalingMode.EnemyCount:
                // Enemy count is handled in GetEnemyCount()
                break;
            case DifficultyScalingMode.EnemyStats:
                // Stats are applied when spawning
                break;
            case DifficultyScalingMode.Combined:
                DecreaseSpawnInterval();
                // Other scalings happen automatically
                break;
        }

        // Visual feedback
        if (showDifficultyIncrease)
        {
            Debug.Log($"Difficulty Increased! Level: {currentDifficultyLevel}");

            if (difficultyIncreaseEffect != null && player != null)
            {
                Instantiate(difficultyIncreaseEffect, player.position, Quaternion.identity);
            }
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

    void Spawn()
    {
        int enemyCount = GetEnemyCount();

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            ApplyDifficultyModifiers(enemy);
        }
    }

    void SpawnWave()
    {
        for (int i = 0; i < waveSize; i++)
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            ApplyDifficultyModifiers(enemy);
        }
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

            // If we can't find a valid position after max attempts, just use the last one
            if (attempts >= maxAttempts)
                break;

        } while (avoidSpawningNearPlayer && Vector3.Distance(spawnPos, player.position) < safeZoneRadius);

        return spawnPos;
    }

    void ApplyDifficultyModifiers(GameObject enemy)
    {
        if (scalingMode != DifficultyScalingMode.EnemyStats && scalingMode != DifficultyScalingMode.Combined)
            return;

        // Try to find common enemy component types and scale them

        // Health scaling
        // if (scaleEnemyHealth)
        // {
        //     var health = enemy.GetComponent<EnemyHealth>();
        //     if (health != null)
        //     {
        //         float multiplier = Mathf.Pow(healthMultiplierPerLevel, currentDifficultyLevel - 1);
        //         health.maxHealth = Mathf.RoundToInt(health.maxHealth * multiplier);
        //         health.currentHealth = health.maxHealth;
        //     }
        // }

        // Speed scaling
        // if (scaleEnemySpeed)
        // {
        //     var movement = enemy.GetComponent<EnemyMovement>();
        //     if (movement != null)
        //     {
        //         float multiplier = Mathf.Pow(speedMultiplierPerLevel, currentDifficultyLevel - 1);
        //         movement.speed *= multiplier;
        //     }
        // }

        // Damage scaling
        // if (scaleEnemyDamage)
        // {
        //     var combat = enemy.GetComponent<EnemyCombat>();
        //     if (combat != null)
        //     {
        //         float multiplier = Mathf.Pow(damageMultiplierPerLevel, currentDifficultyLevel - 1);
        //         combat.damage = Mathf.RoundToInt(combat.damage * multiplier);
        //     }
        // }
    }

    // Public methods for external control
    public void ResetDifficulty()
    {
        currentDifficultyLevel = 1;
        currentInterval = baseInterval;
        difficultyTimer = difficultyIncreaseInterval;
    }

    public void SetDifficultyLevel(int level)
    {
        currentDifficultyLevel = Mathf.Max(1, level);
        for (int i = 1; i < currentDifficultyLevel; i++)
        {
            DecreaseSpawnInterval();
        }
    }

    public int GetCurrentDifficultyLevel()
    {
        return currentDifficultyLevel;
    }

    public float GetCurrentSpawnRate()
    {
        return currentInterval;
    }

    // Visualize spawn range in editor
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Min spawn distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, minSpawnDistance);

        // Max spawn distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, maxSpawnDistance);

        // Safe zone
        if (avoidSpawningNearPlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, safeZoneRadius);
        }
    }
}