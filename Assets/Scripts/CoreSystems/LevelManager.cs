using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    private NewPlayer player;

    [SerializeField] private Weapon weapon;
    [SerializeField] private List<UpgradePrefab> allUpgradePrefabs;
    [SerializeField] private EnemySpawner enemySpawner;

    private void Start()
    {
        player = NewPlayer.Instance;
        InitializeValues(player);
    }

    public void InitializeValues(NewPlayer player)
    {
        PlayerInitializer initializer = player.GetComponent<PlayerInitializer>();

        if (initializer == null)
        {
            Debug.LogError("PlayerInitializer not found on the player object!");
            return;
        }

        player.transform.position = Vector3.zero;

        // Set player initial stats
        player.maxHealth = initializer.maxHealth;
        player.health = initializer.startHealth;
        player.currentXP = initializer.startXP;
        player.xpToNextLevel = initializer.xpToNextLevel;

        // Initialize main weapon dynamically
        Transform mainWeaponTransform = player.transform.Find("WeaponHand/MainWeapon");
        if (mainWeaponTransform != null)
        {
            Weapon mainWeapon = mainWeaponTransform.GetComponent<Weapon>();
            if (mainWeapon != null)
            {
                mainWeapon.gameObject.SetActive(true);
            }
        }

        weapon.rotationSpeed = 100.0f;
        enemySpawner.spawnInterval = 1.2f;

        player.playerLevel = 0;
        player.ResetPlayerScore();
        player.UpdateUI();
    }

    public void UpdatePlayerStats()
    {
        player.playerLevel++;

        TriggerUpgradeSelection();

        player.currentXP -= player.xpToNextLevel;
        player.xpToNextLevel *= 1.5f;

        enemySpawner.spawnInterval /= 1.2f;
    }

    private void TriggerUpgradeSelection()
    {
        // Ensure the allUpgradePrefabs list has enough items
        if (allUpgradePrefabs == null || allUpgradePrefabs.Count < 3)
        {
            Debug.LogError("Not enough upgrade prefabs available in the allUpgradePrefabs list.");
            return;
        }

        // Filter upgrades if weapon slots are full
        List<UpgradePrefab> filteredUpgrades = new List<UpgradePrefab>();
        foreach (var upgrade in allUpgradePrefabs)
        {
            if (upgrade.type == UpgradeType.WeaponActivation && player.AreWeaponSlotsFull())
            {
                continue;
            }
            filteredUpgrades.Add(upgrade);
        }

        // Randomly pick 3 upgrade
        List<UpgradePrefab> selectedUpgrades = new List<UpgradePrefab>();
        while (selectedUpgrades.Count < 3 && filteredUpgrades.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredUpgrades.Count);

            // Make sure the upgrade is not already in
            UpgradePrefab randomUpgrade = filteredUpgrades[randomIndex];
            if (!selectedUpgrades.Contains(randomUpgrade))
            {
                selectedUpgrades.Add(randomUpgrade);
            }
        }

        GameSceneManager.Instance.ShowUpgradeChoices(selectedUpgrades, OnUpgradeSelected);
    }

    private void OnUpgradeSelected(UpgradePrefab selectedUpgrade)
    {
        selectedUpgrade.ApplyUpgrade(player, weapon, this);
        player.UpdateUI();
    }

    public void EquipWeaponInSlot(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            Debug.LogError("Weapon prefab is null.");
            return;
        }

        GameObject newWeaponInstance = Instantiate(weaponPrefab);

        Weapon weaponScript = newWeaponInstance.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            if (player.TryAddWeaponToSlot(weaponScript))
            {
                Debug.Log("Weapon added to a free slot.");
            }
            else
            {
                Debug.LogError("No free slots available.");
                Destroy(newWeaponInstance);
            }
        }
        else
        {
            Debug.LogError("Weapon script not found on the prefab.");
            Destroy(newWeaponInstance);
        }
    }

    public void RemoveActiveSecondaryWeapon()
    {
        // Could be need in Future :) 
    }
}
