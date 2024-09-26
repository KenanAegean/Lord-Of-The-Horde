using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  
using TMPro;          

public class NewPlayer : PhysicsObject, IPausable
{
    //configs
    [SerializeField] public Camera Camera;

    [Header("Inventory")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float health = 100f;

    [Header("XP and Level")]
    [SerializeField] public int playerLevel = 0;
    [SerializeField] public float currentXP = 0f;
    [SerializeField] public float xpToNextLevel = 100f;

    // Reference to UIManager and LevelManager
    private UIManager uiManager;
    [SerializeField] private LevelManager levelManager;


    //state
    bool isAlive = true;
    private bool isPaused = false;

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

        // Dynamically find the UIManager in the scene
        uiManager = FindObjectOfType<UIManager>();

        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in the scene!");
            return;
        }

        // Initialize UI elements using UIManager
        uiManager.UpdateHealthUI(health, maxHealth);
        uiManager.UpdateXPUI(currentXP, xpToNextLevel);
        uiManager.UpdateLevelUI(playerLevel);

        //weaponStats = FindObjectOfType<WeaponHand>();
    }

    public void OnPause()
    {
        isPaused = true;
    }

    public void OnResume()
    {
        isPaused = false;
    }

    public override void Update()
    {
        if (isPaused) return;

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
        uiManager.UpdateHealthUI(health, maxHealth); // Update health bar

        // Show damage pop-up
        uiManager.ShowDamagePopup(damage);

        if (health <= 0)
        {
            Die();
        }
    }

    public void CollectXP(float xpAmount)
    {
        currentXP += xpAmount;
        uiManager.UpdateXPUI(currentXP, xpToNextLevel); // Update XP bar

        // Show XP gain pop-up
        uiManager.ShowXPGainPopup(xpAmount);

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }
        
    private void LevelUp()
    {
        levelManager.UpdatePlayerStats();
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update UI via UIManager
        uiManager.UpdateHealthUI(health, maxHealth);
        uiManager.UpdateXPUI(currentXP, xpToNextLevel);
        uiManager.UpdateLevelUI(playerLevel);
    }

    private void Die()
    {
        isAlive = false;
        Debug.Log("Player died");
        // Handle game over logic here
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
