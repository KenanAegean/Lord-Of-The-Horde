using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : PhysicsObject
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float followDistance = 5.0f;

    private void Start()
    {
        if (_player == null)
        {
            throw new System.InvalidOperationException("Player not set");
        }
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
}
