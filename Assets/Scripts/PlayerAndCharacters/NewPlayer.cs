using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewPlayer : PhysicsObject, IPausable
{
    // Configurations
    [SerializeField] public Camera Camera;

    [Header("Inventory")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float health = 100f;

    [Header("XP and Level")]
    [SerializeField] public int playerLevel = 0;
    [SerializeField] public float currentXP = 0f;
    [SerializeField] public float xpToNextLevel = 100f;

    private UIManager uiManager;
    [SerializeField] private LevelManager levelManager;

    public bool isAlive = true;
    private bool isPaused = false;

    // Singleton instantiation
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
        if (!isAlive) return;

        if (Camera == null) throw new InvalidOperationException("Camera not set");

        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in the scene!");
            return;
        }

        // Initialize UI
        uiManager.UpdateHealthUI(health, maxHealth);
        uiManager.UpdateXPUI(currentXP, xpToNextLevel);
        uiManager.UpdateLevelUI(playerLevel);
    }

    public void OnPause() => isPaused = true;

    public void OnResume() => isPaused = false;

    public override void Update()
    {
        if (isPaused) return;

        FollowMouse();
        base.Update();
    }

    private void FollowMouse()
    {
        _target = Camera.ScreenToWorldPoint(Input.mousePosition);
        _target.z = 0;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        uiManager.UpdateHealthUI(health, maxHealth);
        uiManager.ShowDamagePopup(damage);

        if (health <= 0) Die();
    }

    public void CollectXP(float xpAmount)
    {
        currentXP += xpAmount;
        uiManager.UpdateXPUI(currentXP, xpToNextLevel);
        uiManager.ShowXPGainPopup(xpAmount);

        if (currentXP >= xpToNextLevel) LevelUp();
    }

    private void LevelUp()
    {
        levelManager.UpdatePlayerStats();
        UpdateUI();
    }

    private void UpdateUI()
    {
        uiManager.UpdateHealthUI(health, maxHealth);
        uiManager.UpdateXPUI(currentXP, xpToNextLevel);
        uiManager.UpdateLevelUI(playerLevel);
    }

    private void Die()
    {
        isAlive = false;
        Debug.Log("Player died");

        // Activate the die menu
        if (GameSceneManager.Instance.dieMenuUI != null)
        {
            GameSceneManager.Instance.dieMenuUI.SetActive(true);
        }

        // Optionally, stop all movement or interactions
        // This could involve disabling controls, stopping enemies, etc.
        GameSceneManager.Instance.SetPausableObjectsState(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            GameObject playerBody = GameObject.FindGameObjectWithTag("PlayerBody");
            if (playerBody != null)
            {
                Collider2D playerBodyCollider = playerBody.GetComponent<Collider2D>();

                if (playerBodyCollider != null && other.IsTouching(playerBodyCollider))
                {
                    Collectible collectible = other.GetComponent<Collectible>();
                    if (collectible != null)
                    {
                        CollectXP(collectible.GetXPAmount());
                        Destroy(other.gameObject);
                    }
                }
            }
        }
    }
}
