using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NewPlayer : PhysicsObject
{
    //configs
    [SerializeField] public Camera Camera;

    [Header("Inventory")]
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int health = 100;

    //state
    bool isAlive = true;

    //Singleton instantation
    private static NewPlayer instance;
    public static NewPlayer Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<NewPlayer>();
            return instance;
        }
    }


    private void Start()
    {
        if (!isAlive) { return; }
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

    private void Die()
    {
        
        isAlive = false;
            
    }
}
