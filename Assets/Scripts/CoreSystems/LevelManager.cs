using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    private NewPlayer player;

    [SerializeField] private Weapon weapon;
    [SerializeField] private List<Weapon> secondaryWeapons;

    [SerializeField] private List<UpgradePrefab> allUpgradePrefabs; // Use a list of upgrade prefabs

    [SerializeField] private EnemySpawner enemySpawner;

    private Weapon activeSecondaryWeapon;

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

        // Deactivate all secondary weapons, then activate initial ones
        foreach (Weapon secondaryWeapon in secondaryWeapons)
        {
            secondaryWeapon.gameObject.SetActive(false);
        }
        foreach (Weapon startingSecondaryWeapon in initializer.secondaryWeapons)
        {
            startingSecondaryWeapon.gameObject.SetActive(true);
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

        // Randomly pick 3 upgrade prefabs
        List<UpgradePrefab> selectedUpgrades = new List<UpgradePrefab>();
        while (selectedUpgrades.Count < 3)
        {
            int randomIndex = Random.Range(0, allUpgradePrefabs.Count);

            // Make sure the upgrade is not already in the selected list
            UpgradePrefab randomUpgrade = allUpgradePrefabs[randomIndex];
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

    // Activates a new secondary weapon, replacing any existing one
    public void ActivateSecondaryWeapon(Weapon weaponToActivate)
    {
        if (activeSecondaryWeapon != null)
        {
            activeSecondaryWeapon.StopShooting();
            activeSecondaryWeapon.gameObject.SetActive(false);
        }

        if (secondaryWeapons.Contains(weaponToActivate))
        {
            weaponToActivate.gameObject.SetActive(true);
            weaponToActivate.StartShooting();
            activeSecondaryWeapon = weaponToActivate;
        }
    }

    public void DeactivateActiveSecondaryWeapon()
    {
        if (activeSecondaryWeapon != null)
        {
            activeSecondaryWeapon.gameObject.SetActive(false);
            activeSecondaryWeapon = null;
        }
    }
}
