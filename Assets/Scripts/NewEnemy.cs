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
    [SerializeField] private float patrolRadius = 5.0f; // Radius for random patrol while searching
    [SerializeField] private float patrolInterval = 2.0f; // Time between patrol movements

    private Transform _player;
    private bool _playerFound = false;
    private Vector3 _nextPatrolPoint;

    void Start()
    {
        FindPlayer();
        StartCoroutine(Patrol());
    }

    public override void Update()
    {
        if (!_playerFound)
        {
            FindPlayer(); // Search for the player if not found
        }
        FollowPlayerIfClose(); // Follow player if within range
        base.Update(); // Call PhysicsObject's MoveTowardsTarget method
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
                _target = _player.position;
                _target.z = 0; // Keep the enemy on the same Z-axis (2D)
            }
            else if (distance <= searchRadius)
            {
                // Continue searching if within search radius
                _target = _nextPatrolPoint;
            }
            else
            {
                _player = null; // Player is too far, stop following
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
                _nextPatrolPoint = GetRandomPatrolPoint();
                yield return new WaitForSeconds(patrolInterval);
            }
            else
            {
                yield return null; // Continue to check for player if found
            }
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomPoint = transform.position + new Vector3(
            Random.Range(-patrolRadius, patrolRadius),
            Random.Range(-patrolRadius, patrolRadius),
            0
        );

        // Ensure patrol point is within search radius
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
