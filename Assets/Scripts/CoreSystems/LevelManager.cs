using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    private NewPlayer player;

    [SerializeField] private Weapon weapon;
    [SerializeField] private List<Weapon> secondaryWeapons;

    [SerializeField] private List<UpgradeOption> allUpgrades;

    [SerializeField] private EnemySpawner enemySpawner;

    private Weapon activeSecondaryWeapon;

    private void Start()
    {
        player = NewPlayer.Instance;
        InitializeValues(player); // Initialize values when the game starts
    }

    public void InitializeValues(NewPlayer player)
    {
        // Find the PlayerInitializer on the player to get initial values
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

        // Reset player score and other stats
        player.ResetPlayerScore();

        // Update the UI after resetting values
        player.UpdateUI();
    }

    public void UpdatePlayerStats()
    {
        // Effects.LeveltUpFX(player.transform);
        player.playerLevel++;

        TriggerUpgradeSelection();
        // Effects.LeveltUpFX(player.transform);

        player.currentXP -= player.xpToNextLevel;
        player.xpToNextLevel *= 1.5f;
        // player.maxHealth += 10f;
        // player.health += 15f;
        // player.ObjectSpeed *= 1.3f;

        // weapon.rotationSpeed *= 1.5f;
        // weaponGun.spawnInterval /= 1.5f;

        enemySpawner.spawnInterval /= 1.2f;
    }

    private void TriggerUpgradeSelection()
    {
        // Ensure the allUpgrades list has enough items
        if (allUpgrades == null || allUpgrades.Count < 3)
        {
            Debug.LogError("Not enough upgrades available in the allUpgrades list.");
            return;
        }

        // Randomly pick 3 upgrades
        List<UpgradeOption> selectedUpgrades = new List<UpgradeOption>();
        while (selectedUpgrades.Count < 3)
        {
            int randomIndex = Random.Range(0, allUpgrades.Count);

            // Make sure the upgrade is not already in the selected list
            UpgradeOption randomUpgrade = allUpgrades[randomIndex];
            if (!selectedUpgrades.Contains(randomUpgrade))
            {
                selectedUpgrades.Add(randomUpgrade);
            }
        }

        GameSceneManager.Instance.ShowUpgradeChoices(selectedUpgrades, OnUpgradeSelected);
    }

    private void OnUpgradeSelected(UpgradeOption selectedUpgrade)
    {
        selectedUpgrade.ApplyUpgrade(player, weapon, this);
        player.UpdateUI();
    }

    // Activates a new secondary weapon, replacing any existing one
    public void ActivateSecondaryWeapon(Weapon weaponToActivate)
    {
        // If there's an active secondary weapon, deactivate it
        if (activeSecondaryWeapon != null)
        {
            activeSecondaryWeapon.StopShooting(); // Stop shooting before deactivating
            activeSecondaryWeapon.gameObject.SetActive(false);
        }

        // Activate the new weapon and set it as the active secondary weapon
        if (secondaryWeapons.Contains(weaponToActivate))
        {
            weaponToActivate.gameObject.SetActive(true);
            weaponToActivate.StartShooting(); // Ensure shooting is started after activation
            activeSecondaryWeapon = weaponToActivate;
        }
    }

    // Optional: Method to deactivate the current active secondary weapon
    public void DeactivateActiveSecondaryWeapon()
    {
        if (activeSecondaryWeapon != null)
        {
            activeSecondaryWeapon.gameObject.SetActive(false);
            activeSecondaryWeapon = null;
        }
    }
}
