using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PlayerPrefabCreator : EditorWindow
{
    private GameObject playerPrefab; // Reference to the original player prefab
    private string prefabName = "NewPlayerPrefab";

    // Player Details
    private string playerName = "PlayerName";
    private string playerDescription = "";

    // Player Stats
    private float maxHealth = -1f; // Use -1 as an indicator to take default
    private float startHealth = -1f;
    private float startXP = -1f;
    private float xpToNextLevel = -1f;

    // Player Appearance
    private Sprite playerSprite;

    // Player Weapons
    private Weapon mainWeapon;
    private List<Weapon> secondaryWeapons = new List<Weapon>();

    [MenuItem("Tools/Player Prefab Creator")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefabCreator>("Player Prefab Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a New Player Prefab", EditorStyles.boldLabel);

        // Select a reference prefab
        playerPrefab = (GameObject)EditorGUILayout.ObjectField("Base Player Prefab:", playerPrefab, typeof(GameObject), false);

        prefabName = EditorGUILayout.TextField("Prefab Name:", prefabName);

        // Set player details
        GUILayout.Label("Player Details", EditorStyles.boldLabel);
        playerName = EditorGUILayout.TextField("Player Name:", playerName);
        playerDescription = EditorGUILayout.TextField("Player Description:", playerDescription);

        // Set player stats
        GUILayout.Label("Initial Player Stats", EditorStyles.boldLabel);
        maxHealth = EditorGUILayout.FloatField("Max Health:", maxHealth);
        startHealth = EditorGUILayout.FloatField("Start Health:", startHealth);
        startXP = EditorGUILayout.FloatField("Start XP:", startXP);
        xpToNextLevel = EditorGUILayout.FloatField("XP to Next Level:", xpToNextLevel);

        // Set player appearance
        GUILayout.Label("Player Appearance", EditorStyles.boldLabel);
        playerSprite = (Sprite)EditorGUILayout.ObjectField("Player Sprite:", playerSprite, typeof(Sprite), false);

        // Configure main weapon
        GUILayout.Label("Main Weapon", EditorStyles.boldLabel);
        mainWeapon = (Weapon)EditorGUILayout.ObjectField("Main Weapon:", mainWeapon, typeof(Weapon), false);

        // Configure secondary weapons
        GUILayout.Label("Secondary Weapons", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Weapon Slot"))
        {
            secondaryWeapons.Add(null);
        }

        for (int i = 0; i < secondaryWeapons.Count; i++)
        {
            secondaryWeapons[i] = (Weapon)EditorGUILayout.ObjectField($"Weapon {i + 1}:", secondaryWeapons[i], typeof(Weapon), false);
        }

        if (GUILayout.Button("Create Player Prefab"))
        {
            CreatePlayerPrefab();
        }
    }

    private void CreatePlayerPrefab()
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("Please select a base player prefab to create the new prefab.");
            return;
        }

        // Instantiate a copy of the reference prefab
        GameObject playerInstance = Instantiate(playerPrefab);

        // Update initial values in PlayerInitializer
        PlayerInitializer initializer = playerInstance.GetComponent<PlayerInitializer>();
        if (initializer != null)
        {
            // If any value is empty or unset, use the value from the reference prefab
            initializer.playerName = string.IsNullOrEmpty(playerName) ? initializer.playerName : playerName;
            initializer.playerDescription = string.IsNullOrEmpty(playerDescription) ? initializer.playerDescription : playerDescription;

            initializer.maxHealth = maxHealth >= 0 ? maxHealth : initializer.maxHealth;
            initializer.startHealth = startHealth >= 0 ? startHealth : initializer.startHealth;
            initializer.startXP = startXP >= 0 ? startXP : initializer.startXP;
            initializer.xpToNextLevel = xpToNextLevel >= 0 ? xpToNextLevel : initializer.xpToNextLevel;

            // If secondary weapons list is empty, keep the original list from the reference prefab
            if (secondaryWeapons.Count > 0)
            {
                initializer.secondaryWeapons = secondaryWeapons;
            }
        }

        // Update the player's sprite
        SpriteRenderer spriteRenderer = playerInstance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && playerSprite != null)
        {
            spriteRenderer.sprite = playerSprite;
        }

        // Set up or replace the main weapon
        Transform weaponHand = playerInstance.transform.Find("WeaponHand");
        if (weaponHand != null && mainWeapon != null)
        {
            // Check if there's an existing main weapon
            Transform existingMainWeapon = weaponHand.Find("MainWeapon");
            if (existingMainWeapon != null)
            {
                DestroyImmediate(existingMainWeapon.gameObject);
            }

            // Instantiate and assign the new main weapon as a child of "WeaponHand"
            GameObject mainWeaponInstance = Instantiate(mainWeapon.gameObject, weaponHand);
            mainWeaponInstance.name = "MainWeapon";
        }

        // Save the modified player instance as a new prefab
        string path = "Assets/Prefabs/Player/Tests/" + prefabName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(playerInstance, path);

        // Clean up the instantiated object in the scene
        DestroyImmediate(playerInstance);

        Debug.Log($"Player prefab '{prefabName}' created at {path}");
    }
}
