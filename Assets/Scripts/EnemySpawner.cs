using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;  // The enemy prefab to spawn
    [SerializeField] public float spawnInterval = 1.0f;  // Spawn every second

    // References to boundary GameObjects
    [SerializeField] public Transform topLeftBoundary;  // Reference to the Top Left boundary
    [SerializeField] public Transform bottomRightBoundary;  // Reference to the Bottom Right boundary

    [SerializeField] public GameObject targetPlayer;

    public GameObject GetTargetPlayer()
    {
        return targetPlayer;
    }

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    void Update()
    {
        //Vector3 targtPayerPos = targetPlayer.transform.position;
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
