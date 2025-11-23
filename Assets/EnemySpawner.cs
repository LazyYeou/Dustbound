using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player; // Must be assigned in the Inspector!
    public float interval = 2f;
    float timer;

    void Update()
    {
        // 1. ADD NULL CHECK HERE
        if (player == null)
        {
            // Stop spawning if the player is gone (Game Over state)
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Spawn();
            timer = interval;
        }
    }

    void Spawn()
    {
        // No need for a null check here if we checked in Update()

        Vector2 randomPos = Random.insideUnitCircle.normalized * 10f;

        // 2. The code that caused the original error:
        Vector3 spawnPos = player.position + (Vector3)randomPos;

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}