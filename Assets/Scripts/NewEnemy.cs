using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : PhysicsObject
{
    [Header("Inventory")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float followDistance = 5.0f;
    [SerializeField] private float searchRadius = 10.0f; // Radius to search for the player
    [SerializeField] private float patrolRadius = 7.0f; // Radius for random patrol around spawn point
    [SerializeField] private float patrolInterval = 2.0f; // Time between patrol movements

    private Transform _player;
    private bool _playerFound = false;
    private Vector3 _nextPatrolPoint;

    void Start()
    {
        FindPlayer();
        StartCoroutine(Patrol());  // Patrol behavior
    }

    public override void Update()
    {
        if (!_playerFound)
        {
            FindPlayer(); // Search for the player if not found
        }

        // Set target depending on whether player is detected
        FollowPlayerIfClose();

        // Call the base class's Update method to handle movement
        base.Update();
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.transform;
            _playerFound = true;
        }
    }

    private void FollowPlayerIfClose()
    {
        if (_player != null)
        {
            float distance = Vector3.Distance(transform.position, _player.position);

            if (distance <= followDistance)
            {
                _target = _player.position;  // Set target to the player’s position
                _target.z = 0;  // Ensure enemy stays in 2D plane
            }
            else if (distance <= searchRadius)
            {
                // Continue patrolling if within the search radius
                _target = _nextPatrolPoint;
            }
            else
            {
                // Player is too far, reset player reference
                _player = null;
                _playerFound = false;
            }
        }
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            if (!_playerFound)
            {
                // Get a new random patrol point
                _nextPatrolPoint = GetRandomPatrolPoint();

                // Set patrol point as target
                _target = _nextPatrolPoint;

                // Wait for patrolInterval seconds before moving to a new point
                yield return new WaitForSeconds(patrolInterval);
            }
            else
            {
                yield return null; // If the player is found, don't patrol
            }
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomPoint;

        // Ensure patrol point is far enough from current position to prevent gathering
        do
        {
            randomPoint = transform.position + new Vector3(
                Random.Range(-patrolRadius, patrolRadius),
                Random.Range(-patrolRadius, patrolRadius),
                0
            );
        }
        while (Vector3.Distance(transform.position, randomPoint) < 2.0f);  // Ensure patrol point is far enough away

        // Ensure patrol point stays within searchRadius
        if (Vector3.Distance(transform.position, randomPoint) > searchRadius)
        {
            randomPoint = transform.position + (randomPoint - transform.position).normalized * searchRadius;
        }

        return randomPoint;
    }

    public void TakeDamage(float someDamage)
    {
        Debug.Log("Enemy took damage");
        health -= someDamage;
        if (health <= 0) Die();
    }

    public void Die()
    {
        Destroy(gameObject);
        Debug.Log("Enemy died");
    }
}
