using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NewPlayer : PhysicsObject
{
    [SerializeField] public Camera Camera;

    private void Start()
    {
        if (Camera == null)
        {
            throw new System.InvalidOperationException("Camera not set");
        }
    }

    public override void Update()
    {
        FollowMouse(); // Update _target to mouse position
        base.Update(); // Call PhysicsObject's MoveTowardsTarget method
    }

    private void FollowMouse()
    {
        _target = Camera.ScreenToWorldPoint(Input.mousePosition);
        _target.z = 0; // Keep the player on the same Z-axis (2D)
    }
}
