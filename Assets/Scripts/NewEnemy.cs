using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : PhysicsObject
{
    [Header("Inventory")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float health = 100f;
    [SerializeField] private GameObject _player;
    [SerializeField] private float followDistance = 5.0f;

    EnemySpawner enemySpawner;


    private void Start()
    {
        if (_player == null)
        {
            throw new System.InvalidOperationException("Player not set");
        }
        enemySpawner = GetComponent<EnemySpawner>();
    }

    public override void Update()
    {
        FollowPlayerIfClose(); // Update _target if within range
        base.Update(); // Call PhysicsObject's MoveTowardsTarget method
    }

    private void FollowPlayerIfClose()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= followDistance)
        {
            _target = _player.transform.position;
            _target.z = 0; // Keep the enemy on the same Z-axis (2D)
        }
    }

    public void TakeDamage(float someDamage)
    {
        Debug.Log("enemy bleed");
        health -= someDamage;
        if (health <= 0) Die();
    }

    public void Die()
    {
        Destroy(gameObject);
        Debug.Log("enemy dead");
    }
}
