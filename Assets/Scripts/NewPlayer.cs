using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  
using TMPro;          

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

    // UI references
    [Header("UI Elements")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI levelText;

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

        // Initialize UI elements
        UpdateHealthUI();
        UpdateXPUI();
        UpdateLevelUI();
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        UpdateHealthUI();

        if (health <= 0)
        {
            Die();
        }
    }

    public void CollectXP(float xpAmount)
    {
        currentXP += xpAmount;
        UpdateXPUI();

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        playerLevel++;
        currentXP -= xpToNextLevel; // Carry over extra XP
        xpToNextLevel *= 1.5f;      // Increase XP threshold for the next level
        maxHealth += 10f;           // Increase max health as a bonus
        health += 15f;           // Increase health as a bonus
        //health = maxHealth;         // Restore health upon leveling up

        UpdateXPUI();
        UpdateLevelUI();
        UpdateHealthUI();           // Health changes on level up
    }

    private void Die()
    {
        isAlive = false;
        Debug.Log("Player died");
        // Handle game over logic here
    }

    // UI update methods
    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }
    }

    private void UpdateXPUI()
    {
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = currentXP;
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "LEVEL : " + playerLevel;
        }
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
