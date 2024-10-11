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

    [Header("Dynamic Boundaries")]
    [SerializeField] private Transform player;       
    [SerializeField] private float spawnRadius = 20f; 

    [Header("Enemies Parent")]
    [SerializeField] private Transform enemiesParent; 

    private float totalWeight;
    private bool isPaused = false;

    private void Start()
    {
        if (enemyPrefabs.Count != spawnWeights.Count)
        {
            Debug.LogError("The number of enemy prefabs and weights must be the same!");
            return;
        }

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

            Vector2 spawnPosition = GetRandomSpawnPositionAroundPlayer();
            GameObject selectedEnemyPrefab = GetRandomEnemyPrefab();

            // Spawn the enemy and set its parent to the "Enemies" GameObject
            GameObject enemy = Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity, enemiesParent);

            float elapsedTime = 0f;
            while (elapsedTime < spawnInterval)
            {
                while (isPaused) yield return null;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    // Get a random spawn position around the player
    private Vector2 GetRandomSpawnPositionAroundPlayer()
    {
        float angle = Random.Range(0f, 2f * Mathf.PI); 
        float distance = Random.Range(0f, spawnRadius); 
        Vector2 spawnPosition = new Vector2(
            player.position.x + Mathf.Cos(angle) * distance,
            player.position.y + Mathf.Sin(angle) * distance
        );
        return spawnPosition;
    }

    private GameObject GetRandomEnemyPrefab()
    {
        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0.0f;

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            cumulativeWeight += spawnWeights[i];
            if (randomValue < cumulativeWeight) return enemyPrefabs[i];
        }

        return enemyPrefabs[0];
    }
}
