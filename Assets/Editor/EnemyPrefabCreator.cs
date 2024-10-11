using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EnemyPrefabCreator : EditorWindow
{
    private GameObject enemyPrefab;
    private string prefabName = "NewEnemyPrefab";

    // Enemy Stats
    private float maxHealth = -1f;
    private float damage = -1f;
    private float followDistance = -1f;
    private float searchRadius = -1f;
    private float patrolRadius = -1f;
    private float patrolInterval = -1f;
    private float escapeDistance = -1f;
    private float escapeSpeed = -1f;
    private float xpAmount = -1f;

    // Enemy Appearance
    private Sprite enemySprite;
    private List<GameObject> damageStatusPrefabs = new List<GameObject>();

    // Enemy Collectible Drops
    private GameObject collectiblePrefab;
    private Color collectibleColor = Color.clear;

    // Death Effect
    private Color deathEffectColor = Color.clear;

    // Other Attributes
    private bool? isMinion = null;

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
        isMinion = EditorGUILayout.Toggle("Is Minion", isMinion ?? false);

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
            // If any value is unset (-1 for float, null for objects), use the value from the reference prefab
            enemyScript.maxHealth = maxHealth >= 0 ? maxHealth : enemyScript.maxHealth;
            enemyScript.health = enemyScript.maxHealth;
            enemyScript.Damage = damage >= 0 ? damage : enemyScript.Damage;
            enemyScript.followDistance = followDistance >= 0 ? followDistance : enemyScript.followDistance;
            enemyScript.searchRadius = searchRadius >= 0 ? searchRadius : enemyScript.searchRadius;
            enemyScript.patrolRadius = patrolRadius >= 0 ? patrolRadius : enemyScript.patrolRadius;
            enemyScript.patrolInterval = patrolInterval >= 0 ? patrolInterval : enemyScript.patrolInterval;
            enemyScript.escapeDistance = escapeDistance >= 0 ? escapeDistance : enemyScript.escapeDistance;
            enemyScript.escapeSpeed = escapeSpeed >= 0 ? escapeSpeed : enemyScript.escapeSpeed;
            enemyScript.xpAmount = xpAmount >= 0 ? xpAmount : enemyScript.xpAmount;

            if (damageStatusPrefabs.Count > 0)
            {
                enemyScript.damageStatusPrefabs = damageStatusPrefabs;
            }

            enemyScript.collectablePrefab = collectiblePrefab != null ? collectiblePrefab : enemyScript.collectablePrefab;

            if (collectibleColor != Color.clear)
            {
                enemyScript.collectibleColor = collectibleColor;
            }

            if (deathEffectColor != Color.clear)
            {
                enemyScript.deathEffectColor = deathEffectColor;
            }

            enemyScript.isMinion = isMinion.HasValue ? isMinion.Value : enemyScript.isMinion;
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
