using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Selection")]
    [SerializeField] private List<GameObject> playerPrefabs; // List of player prefabs for initial data
    [SerializeField] private GameObject characterSelectionCanvas; // Reference to the Character Selection Canvas

    [Header("UI Elements")]
    [SerializeField] private Image playerImage; // Player portrait image
    [SerializeField] private Image playerMainWeaponImage; // Main weapon image
    [SerializeField] private TextMeshProUGUI playerNameText; // Player name text
    [SerializeField] private TextMeshProUGUI playerDescriptionText; // Player description text

    [Header("In-Scene Player")]
    [SerializeField] private NewPlayer existingPlayer; // Reference to the single player in the scene

    private int currentIndex = 0;

    private void Start()
    {
        // Start with the selection canvas inactive
        characterSelectionCanvas.SetActive(false);
    }

    public void StartCharacterSelection()
    {
        // Activate the character selection canvas when called
        characterSelectionCanvas.SetActive(true);
        UpdateUI();
    }

    private void Update()
    {
        // Handle navigation input for switching players
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Navigate(-1); // Move to the previous player
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Navigate(1); // Move to the next player
        }
    }

    private void Navigate(int direction)
    {
        // Calculate new index with wrap-around
        currentIndex += direction;
        if (currentIndex < 0) currentIndex = playerPrefabs.Count - 1;
        if (currentIndex >= playerPrefabs.Count) currentIndex = 0;

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Get the PlayerInitializer component from the current selection
        GameObject selectedPlayerPrefab = playerPrefabs[currentIndex];
        PlayerInitializer initializer = selectedPlayerPrefab.GetComponent<PlayerInitializer>();
        if (initializer == null) return;

        // Update UI elements with data from the initializer
        playerNameText.text = initializer.playerName;
        playerDescriptionText.text = initializer.playerDescription;

        // Get the portrait from the SpriteRenderer of the root object
        SpriteRenderer spriteRenderer = selectedPlayerPrefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            playerImage.sprite = spriteRenderer.sprite;
        }

        // Get the main weapon's sprite
        Transform mainWeaponTransform = selectedPlayerPrefab.transform.Find("WeaponHand/MainWeapon");
        if (mainWeaponTransform != null)
        {
            Weapon mainWeapon = mainWeaponTransform.GetComponent<Weapon>();
            if (mainWeapon != null)
            {
                SpriteRenderer weaponSpriteRenderer = mainWeapon.GetComponent<SpriteRenderer>();
                if (weaponSpriteRenderer != null)
                {
                    playerMainWeaponImage.sprite = weaponSpriteRenderer.sprite;
                }
            }
        }
    }

    public void ConfirmSelection()
    {
        // Get the PlayerInitializer component from the selected prefab
        GameObject selectedPlayerPrefab = playerPrefabs[currentIndex];
        PlayerInitializer initializer = selectedPlayerPrefab.GetComponent<PlayerInitializer>();
        if (initializer == null) return;

        // Apply the initializer's data to the existing player
        ApplyDataToPlayer(initializer, selectedPlayerPrefab);

        // Hide the character selection canvas and proceed to gameplay
        characterSelectionCanvas.SetActive(false);

        // Proceed with game start logic (e.g., unpausing the game)
        GameSceneManager.Instance.StartGame();
    }

    private void ApplyDataToPlayer(PlayerInitializer initializer, GameObject selectedPlayerPrefab)
    {
        // Apply initial stats and properties to the existing player
        existingPlayer.maxHealth = initializer.maxHealth;
        existingPlayer.health = initializer.startHealth;
        existingPlayer.currentXP = initializer.startXP;
        existingPlayer.xpToNextLevel = initializer.xpToNextLevel;

        // Get and apply the main weapon
        Transform mainWeaponTransform = selectedPlayerPrefab.transform.Find("WeaponHand/MainWeapon");
        if (mainWeaponTransform != null)
        {
            Weapon mainWeapon = mainWeaponTransform.GetComponent<Weapon>();
            if (mainWeapon != null)
            {
                existingPlayer.ApplyInitialValues(initializer); // Use ApplyInitialValues to set stats and weapons
            }
        }

        // Update the player's UI
        existingPlayer.UpdateUI();
    }
}
