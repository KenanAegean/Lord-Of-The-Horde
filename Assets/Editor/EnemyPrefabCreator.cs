using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EnemyPrefabCreator : EditorWindow
{
    private GameObject enemyPrefab; // Reference to the original enemy prefab
    private string prefabName = "NewEnemyPrefab";

    // Enemy Stats
    private float maxHealth = 100f;
    private float damage = 10f;
    private float followDistance = 5.0f;
    private float searchRadius = 10.0f;
    private float patrolRadius = 7.0f;
    private float patrolInterval = 2.0f;
    private float escapeDistance = 5.0f;
    private float escapeSpeed = 3.0f;
    private float xpAmount = 25.0f;

    // Enemy Appearance
    private Sprite enemySprite;
    private List<GameObject> damageStatusPrefabs = new List<GameObject>();

    // Enemy Collectible Drops
    private GameObject collectiblePrefab;
    private Color collectibleColor = Color.white;

    // Death Effect
    private Color deathEffectColor = Color.red;

    // Other Attributes
    private bool isMinion = false;

    [MenuItem("Tools/Enemy Prefab Creator")]
    public static void ShowWindow()
    {
        GetWindow<EnemyPrefabCreator>("Enemy Prefab Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a New Enemy Prefab", EditorStyles.boldLabel);

        // Select a reference prefab
        enemyPrefab = (GameObject)EditorGUILayout.ObjectField("Base Enemy Prefab:", enemyPrefab, typeof(GameObject), false);

        prefabName = EditorGUILayout.TextField("Prefab Name:", prefabName);

        // Set enemy stats
        GUILayout.Label("Enemy Stats", EditorStyles.boldLabel);
        maxHealth = EditorGUILayout.FloatField("Max Health:", maxHealth);
        damage = EditorGUILayout.FloatField("Damage:", damage);
        followDistance = EditorGUILayout.FloatField("Follow Distance:", followDistance);
        searchRadius = EditorGUILayout.FloatField("Search Radius:", searchRadius);
        patrolRadius = EditorGUILayout.FloatField("Patrol Radius:", patrolRadius);
        patrolInterval = EditorGUILayout.FloatField("Patrol Interval:", patrolInterval);
        escapeDistance = EditorGUILayout.FloatField("Escape Distance:", escapeDistance);
        escapeSpeed = EditorGUILayout.FloatField("Escape Speed:", escapeSpeed);
        xpAmount = EditorGUILayout.FloatField("XP Amount:", xpAmount);

        // Set enemy appearance
        GUILayout.Label("Enemy Appearance", EditorStyles.boldLabel);
        enemySprite = (Sprite)EditorGUILayout.ObjectField("Enemy Sprite:", enemySprite, typeof(Sprite), false);

        // Configure damage state sprites
        GUILayout.Label("Damage Status Prefabs", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Damage Status Slot"))
        {
            damageStatusPrefabs.Add(null);
        }
        for (int i = 0; i < damageStatusPrefabs.Count; i++)
        {
            damageStatusPrefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Damage Status {i + 1}:", damageStatusPrefabs[i], typeof(GameObject), false);
        }

        // Configure collectible drop and color
        GUILayout.Label("Collectible Drop", EditorStyles.boldLabel);
        collectiblePrefab = (GameObject)EditorGUILayout.ObjectField("Collectible Prefab:", collectiblePrefab, typeof(GameObject), false);
        collectibleColor = EditorGUILayout.ColorField("Collectible Color:", collectibleColor);

        // Set death effect color
        GUILayout.Label("Death Effect", EditorStyles.boldLabel);
        deathEffectColor = EditorGUILayout.ColorField("Death Effect Color:", deathEffectColor);

        // Other attributes
        isMinion = EditorGUILayout.Toggle("Is Minion", isMinion);

        if (GUILayout.Button("Create Enemy Prefab"))
        {
            CreateEnemyPrefab();
        }
    }

    private void CreateEnemyPrefab()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Please select a base enemy prefab to create the new prefab.");
            return;
        }

        // Instantiate a copy of the reference prefab
        GameObject enemyInstance = Instantiate(enemyPrefab);

        // Update initial values in NewEnemy
        NewEnemy enemyScript = enemyInstance.GetComponent<NewEnemy>();
        if (enemyScript != null)
        {
            enemyScript.maxHealth = maxHealth;
            enemyScript.health = maxHealth; // Set initial health to max health
            enemyScript.Damage = damage;
            enemyScript.followDistance = followDistance;
            enemyScript.searchRadius = searchRadius;
            enemyScript.patrolRadius = patrolRadius;
            enemyScript.patrolInterval = patrolInterval;
            enemyScript.escapeDistance = escapeDistance;
            enemyScript.escapeSpeed = escapeSpeed;
            enemyScript.xpAmount = xpAmount;
            enemyScript.damageStatusPrefabs = damageStatusPrefabs;
            enemyScript.collectablePrefab = collectiblePrefab;
            enemyScript.collectibleColor = collectibleColor;
            enemyScript.deathEffectColor = deathEffectColor;
            enemyScript.isMinion = isMinion;
        }

        // Update the enemy's sprite
        SpriteRenderer spriteRenderer = enemyInstance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && enemySprite != null)
        {
            spriteRenderer.sprite = enemySprite;
        }

        // Save the modified enemy instance as a new prefab
        string path = "Assets/Prefabs/Enemy/Tests/" + prefabName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(enemyInstance, path);

        // Clean up the instantiated object in the scene
        DestroyImmediate(enemyInstance);

        Debug.Log($"Enemy prefab '{prefabName}' created at {path}");
    }
}
