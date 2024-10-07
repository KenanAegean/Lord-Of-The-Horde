using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewPlayer : PhysicsObject, IPausable
{
    [Header("Current Player Stats")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float health = 100f;
    [SerializeField] public float currentXP = 0f;
    [SerializeField] public float xpToNextLevel = 100f;
    [SerializeField] public int playerLevel = 0;

    // Configurations
    [SerializeField] public Camera Camera;

    [SerializeField] public float score = 0f;

    private UIManager uiManager;
    [SerializeField] private LevelManager levelManager;

    public bool isAlive = true;
    private bool isPaused = false;
    private SpriteRenderer spriteRenderer;

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

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer could not be found on the parent object!");
        }

        // Initialize UI
        uiManager.UpdateHealthUI(health, maxHealth);
        uiManager.UpdateXPUI(currentXP, xpToNextLevel);
        uiManager.UpdateLevelUI(playerLevel);
        uiManager.UpdateScoreUI(currentXP);
        uiManager.ShowLastScore(score);
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
    
        Vector3 playerPosition = transform.position;
        if (_target.x < playerPosition.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
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
        score += xpAmount;

        uiManager.UpdateXPUI(currentXP, xpToNextLevel);
        uiManager.UpdateScoreUI(score);
        uiManager.ShowXPGainPopup(xpAmount);

        if (currentXP >= xpToNextLevel) LevelUp();
    }

    private void LevelUp()
    {
        levelManager.UpdatePlayerStats();
        UpdateUI();
    }

    public void UpdateUI()
    {
        uiManager.UpdateHealthUI(health, maxHealth);
        uiManager.UpdateXPUI(currentXP, xpToNextLevel);
        uiManager.UpdateLevelUI(playerLevel);
        uiManager.UpdateScoreUI(score);
        uiManager.ShowLastScore(score);
    }

    private void Die()
    {
        isAlive = false;
        Debug.Log("Player died");

        // Save the last run score
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.SetLastRunScore(score);
        }

        // Update the score on the die screen directly from NewPlayer's score
        ScoreManager.Instance.UpdateDieCanvasScore(score);

        if (GameSceneManager.Instance.dieMenuUI != null)
        {
            GameSceneManager.Instance.dieMenuUI.SetActive(true);
        }

        GameSceneManager.Instance.SetPausableObjectsState(false);
    }

    public void ResetPlayerScore()
    {
        score = 0;
        uiManager.UpdateScoreUI(score);
    }

    public void ResetPlayerState()
    {
        // Reset player to initial state
        health = maxHealth;
        currentXP = 0f;
        transform.position = Vector3.zero;

        UIManager.Instance.UpdateHealthUI(health, maxHealth);
        UIManager.Instance.UpdateXPUI(currentXP, xpToNextLevel);
    }

    public void ApplyInitialValues(PlayerInitializer initializer)
    {
        maxHealth = initializer.maxHealth;
        health = initializer.startHealth;
        currentXP = initializer.startXP;
        xpToNextLevel = initializer.xpToNextLevel;

        Transform mainWeaponTransform = transform.Find("WeaponHand/MainWeapon");
        if (mainWeaponTransform != null)
        {
            Weapon mainWeapon = mainWeaponTransform.GetComponent<Weapon>();
            if (mainWeapon != null)
            {
                EquipMainWeapon(mainWeapon);
            }
        }

        // Equip secondary weapons (if available)
        foreach (var secondaryWeapon in initializer.secondaryWeapons)
        {
            EquipSecondaryWeapon(secondaryWeapon);
        }

        UpdateUI();
    }

    private void EquipMainWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(true);
    }

    private void EquipSecondaryWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(true);
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
