using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IPausable
{
    [Header("Spawner Settings")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<float> spawnWeights;
    [SerializeField] public float spawnInterval = 1.0f;

    [Header("Boundaries")]
    [SerializeField] private Transform topLeftBoundary;
    [SerializeField] private Transform bottomRightBoundary;

    private float totalWeight;
    private bool isPaused = false;

    private void Start()
    {
        if (enemyPrefabs.Count != spawnWeights.Count)
        {
            Debug.LogError("The number of enemy prefabs and weights must be the same!");
            return;
        }

        // Calculate total weight
        totalWeight = spawnWeights.Sum();

        StartCoroutine(SpawnEnemy());
    }

    public void OnPause() => isPaused = true;

    public void OnResume() => isPaused = false;

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            while (isPaused) yield return null;

            Vector2 spawnPosition = new Vector2(
                Random.Range(topLeftBoundary.position.x, bottomRightBoundary.position.x),
                Random.Range(bottomRightBoundary.position.y, topLeftBoundary.position.y)
            );

            GameObject selectedEnemyPrefab = GetRandomEnemyPrefab();
            Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);

            float elapsedTime = 0f;
            while (elapsedTime < spawnInterval)
            {
                while (isPaused) yield return null;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    // Selects an enemy prefab based on spawn weights
    private GameObject GetRandomEnemyPrefab()
    {
        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0.0f;

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            cumulativeWeight += spawnWeights[i];
            if (randomValue < cumulativeWeight) return enemyPrefabs[i];
        }

        return enemyPrefabs[0]; // Fallback
    }
}
