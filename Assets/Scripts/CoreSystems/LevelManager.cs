using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    private NewPlayer player;

    [SerializeField] private Weapon weapon;
    [SerializeField] private Weapon weaponGun;
    [SerializeField] private Weapon weaponPunch;
    [SerializeField] private Weapon weaponPunchSecond;

    [SerializeField] private List<UpgradeOption> allUpgrades;

    [SerializeField] private EnemySpawner enemySpawner;

    private void Start()
    {
        player = NewPlayer.Instance;
        InitalValues();
    }

    public void InitalValues()
    {
        player.transform.position = new Vector3(0, 0, 0);

        weaponPunch.gameObject.SetActive(true);
        weaponPunchSecond.gameObject.SetActive(false);
        weaponGun.gameObject.SetActive(false);

        weapon.rotationSpeed = 100.0f;

        enemySpawner.spawnInterval = 1.2f;

        player.playerLevel = 0;
        player.xpToNextLevel = 50.0f;

        // Reset player health, score, and other stats as necessary
        player.health = player.maxHealth;
        player.currentXP = 0f;
        player.ResetPlayerScore();

        // Update the UI after resetting values
        player.UpdateUI();
    }

    public void UpdatePlayerStats()
    {
        Effects.LeveltUpFX(player.transform);
        player.playerLevel++;

        TriggerUpgradeSelection();

        player.currentXP -= player.xpToNextLevel;
        player.xpToNextLevel *= 1.5f;
        //player.maxHealth += 10f;
        //player.health += 15f;
        //player.ObjectSpeed *= 1.3f;

        weapon.rotationSpeed *= 1.5f;
        weaponGun.spawnInterval /= 1.5f;

        enemySpawner.spawnInterval /= 1.5f;
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
        selectedUpgrade.ApplyUpgrade(player);
        player.UpdateUI();
    }
}
