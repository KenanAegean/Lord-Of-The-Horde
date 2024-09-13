using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject enemyPrefab;  // The enemy prefab to spawn
    [SerializeField] private float spawnInterval = 1.0f;  // Spawn every second

    [Header("Boundaries")]
    [SerializeField] private Transform topLeftBoundary;  // Reference to the Top Left boundary
    [SerializeField] private Transform bottomRightBoundary;  // Reference to the Bottom Right boundary

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            // Generate random position within the defined boundary area
            Vector2 spawnPosition = new Vector2(
                Random.Range(topLeftBoundary.position.x, bottomRightBoundary.position.x),
                Random.Range(bottomRightBoundary.position.y, topLeftBoundary.position.y)
            );

            // Instantiate the enemy prefab at the random position
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // Wait for the defined spawn interval before spawning the next enemy
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
