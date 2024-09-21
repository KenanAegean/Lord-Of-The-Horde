using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayer : PhysicsObject
{
    //configs
    [SerializeField] public Camera Camera;

    [Header("Inventory")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float health = 100f;

    [Header("XP and Level")]
    [SerializeField] private int playerLevel = 1;
    [SerializeField] private float currentXP = 0f;
    [SerializeField] private float xpToNextLevel = 100f; 

    //state
    bool isAlive = true;

    //Singleton instantiation
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
        base.Update(); // Call parent's MoveTowardsTarget method
    }

    private void FollowMouse()
    {
        _target = Camera.ScreenToWorldPoint(Input.mousePosition);
        _target.z = 0;
    }

    public void TakeDamage(float someDamage)
    {
        health -= someDamage;
        if (health <= 0) Die();
    }

    public void Die()
    {
        isAlive = false;
        Debug.Log("Player Died");
        // You can add game over logic or reset the level here
    }

    // XP collection logic
    public void CollectXP(float xpAmount)
    {
        currentXP += xpAmount;
        Debug.Log($"Collected {xpAmount} XP. Current XP: {currentXP}/{xpToNextLevel}");

        // Check if player should level up
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        playerLevel++;
        currentXP -= xpToNextLevel; // Carry over the extra XP
        xpToNextLevel *= 1.5f; // Increase XP threshold for next level (can be adjusted)
        Debug.Log($"Level up! New level: {playerLevel}. XP needed for next level: {xpToNextLevel}");

        // Optionally grant player bonuses for leveling up (e.g., more health, speed, damage, etc.)
        maxHealth += 10f; // Example bonus
        health = maxHealth; // Restore health upon leveling up
    }

    // Handle collectible collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we collided with is a Collectible
        if (other.CompareTag("Collectible"))
        {
            // Find the child object tagged "PlayerBody"
            GameObject playerBody = GameObject.FindGameObjectWithTag("PlayerBody");
            if (playerBody != null)
            {
                Collider2D playerBodyCollider = playerBody.GetComponent<Collider2D>();

                // Ensure the collision is with PlayerBody
                if (playerBodyCollider != null && other.IsTouching(playerBodyCollider))
                {
                    // Assume the collectible has a script that defines the XP amount
                    Collectible collectible = other.GetComponent<Collectible>();
                    if (collectible != null)
                    {
                        CollectXP(collectible.GetXPAmount()); // Collect the XP
                        Destroy(other.gameObject); // Destroy the collectible after it's picked up
                    }
                }
            }
        }
    }

}
