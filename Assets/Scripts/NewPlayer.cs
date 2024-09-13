using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NewPlayer : PhysicsObject
{
    //configs
    [SerializeField] public Camera Camera;

    [Header("Inventory")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float health = 100f;

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
        base.Update(); // Call parents MoveTowardsTarget method
    }

    private void FollowMouse()
    {
        _target = Camera.ScreenToWorldPoint(Input.mousePosition);
        _target.z = 0; 
    }

    public void TakeDamage(float someDamage)
    {
        health -= someDamage;
        if (health < 0) Die();
    }

    public void Die()
    {
        throw new NotImplementedException();
    }
}
