using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IPausable
{
    [Header("Spawner Settings")]
    [SerializeField] private List<GameObject> enemyPrefabs;  // List of enemy prefabs
    [SerializeField] private List<float> spawnWeights;       // List of weights for each enemy type (rarity)
    [SerializeField] public float spawnInterval = 1.0f;     // Spawn every second

    [Header("Boundaries")]
    [SerializeField] private Transform topLeftBoundary;      // Reference to the Top Left boundary
    [SerializeField] private Transform bottomRightBoundary;  // Reference to the Bottom Right boundary

    private float totalWeight;
    private bool isPaused = false;

    private void Start()
    {
        if (enemyPrefabs.Count != spawnWeights.Count)
        {
            Debug.LogError("The number of enemy prefabs and weights must be the same!");
            return;
        }

        // Calculate the total weight for later use
        foreach (float weight in spawnWeights)
        {
            totalWeight += weight;
        }

        StartCoroutine(SpawnEnemy());
    }

    public void OnPause()
    {
        isPaused = true;
    }

    public void OnResume()
    {
        isPaused = false;
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            // Check if the game is paused
            while (isPaused)
            {
                yield return null;  // While paused, do nothing and wait for the next frame
            }

            // Generate random position within the defined boundary area
            Vector2 spawnPosition = new Vector2(
                Random.Range(topLeftBoundary.position.x, bottomRightBoundary.position.x),
                Random.Range(bottomRightBoundary.position.y, topLeftBoundary.position.y)
            );

            // Select an enemy prefab based on weighted random selection
            GameObject selectedEnemyPrefab = GetRandomEnemyPrefab();

            // Instantiate the selected enemy prefab at the random position
            Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);

            // Wait for the defined spawn interval before spawning the next enemy, but still respect the pause state
            float elapsedTime = 0f;
            while (elapsedTime < spawnInterval)
            {
                // If paused, stop the countdown
                while (isPaused)
                {
                    yield return null;
                }

                // Continue counting the spawn interval
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    // This function selects an enemy prefab based on the weights (rarity)
    private GameObject GetRandomEnemyPrefab()
    {
        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0.0f;

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            cumulativeWeight += spawnWeights[i];
            if (randomValue < cumulativeWeight)
            {
                return enemyPrefabs[i];
            }
        }

        // Fallback (should not occur if weights are properly set)
        return enemyPrefabs[0];
    }
}
