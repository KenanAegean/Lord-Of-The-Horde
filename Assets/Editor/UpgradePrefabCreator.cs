using UnityEditor;
using UnityEngine;

public class UpgradePrefabCreator : EditorWindow
{
    private string prefabName = "NewUpgradePrefab";

    // Upgrade Details
    private string upgradeName = "UpgradeName";
    private UpgradeType upgradeType = UpgradeType.HealthIncrease;
    private string upgradeDescription = "";
    private float upgradeValue = 1f;
    public GameObject weaponPrefab; 
    private Sprite upgradeIcon;

    [MenuItem("Tools/Upgrade Prefab Creator")]
    public static void ShowWindow()
    {
        GetWindow<UpgradePrefabCreator>("Upgrade Prefab Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a New Upgrade Prefab", EditorStyles.boldLabel);

        prefabName = EditorGUILayout.TextField("Prefab Name:", prefabName);

        // Set upgrade details
        GUILayout.Label("Upgrade Details", EditorStyles.boldLabel);
        upgradeName = EditorGUILayout.TextField("Upgrade Name:", upgradeName);
        upgradeType = (UpgradeType)EditorGUILayout.EnumPopup("Upgrade Type:", upgradeType);
        upgradeDescription = EditorGUILayout.TextField("Upgrade Description:", upgradeDescription);
        upgradeValue = EditorGUILayout.FloatField("Upgrade Value:", upgradeValue);

        // Set optional weapon to activate
        GUILayout.Label("Weapon to Activate (Optional)", EditorStyles.boldLabel);
        weaponPrefab = (GameObject)EditorGUILayout.ObjectField("Weapon Prefab:", weaponPrefab, typeof(GameObject), false); // Correct type is GameObject

        // Set upgrade icon
        GUILayout.Label("Upgrade Icon", EditorStyles.boldLabel);
        upgradeIcon = (Sprite)EditorGUILayout.ObjectField("Icon:", upgradeIcon, typeof(Sprite), false);

        if (GUILayout.Button("Create Upgrade Prefab"))
        {
            CreateUpgradePrefab();
        }
    }

    private void CreateUpgradePrefab()
    {
        // Create an empty GameObject for the new upgrade prefab
        GameObject upgradeGO = new GameObject(prefabName);

        // Add UpgradePrefab script to the GameObject
        UpgradePrefab upgradePrefab = upgradeGO.AddComponent<UpgradePrefab>();

        // Set all values based on the editor inputs
        upgradePrefab.upgradeName = upgradeName;
        upgradePrefab.type = upgradeType;
        upgradePrefab.description = upgradeDescription;
        upgradePrefab.value = upgradeValue;
        upgradePrefab.weaponPrefab = weaponPrefab;
        upgradePrefab.icon = upgradeIcon;

        // Define the path
        string path = $"Assets/Prefabs/Upgrade/{prefabName}.prefab";
        System.IO.Directory.CreateDirectory("Assets/Prefabs/Upgrade");

        // Save the GameObject as a prefab
        PrefabUtility.SaveAsPrefabAsset(upgradeGO, path);

        // Clean up by destroying the GameObject in the scene
        DestroyImmediate(upgradeGO);

        Debug.Log($"Upgrade prefab '{prefabName}' created at {path}");
    }
}
