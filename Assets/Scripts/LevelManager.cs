using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Reference to the player
    private NewPlayer player;

    // Reference to the weapon
    [SerializeField] private Weapon weapon;
    [SerializeField] private Weapon weaponGun;
    [SerializeField] private Weapon weaponPunch;
    [SerializeField] private EnemySpawner enemySpawner;
    

    private void Start()
    {
        // Get reference
        player = NewPlayer.Instance;

        initalValues();
    }

    public void initalValues()
    {
        weaponGun.gameObject.SetActive(false);
        weapon.rotationSpeed = 100.0f;
        enemySpawner.spawnInterval = 2.0f;
        player.playerLevel = 0;
    }

    // Function to handle player stats update upon leveling up
    public void UpdatePlayerStats()
    {
        player.playerLevel++;

        if (player.playerLevel == 1)
        {
            weaponGun.gameObject.SetActive(true);
        }

        
        player.currentXP -= player.xpToNextLevel; // Carry over extra XP
        player.xpToNextLevel *= 1.5f;      // Increase XP threshold for next level
        player.maxHealth += 10f;           // Increase max health
        player.health += 15f;              // Heal a bit upon leveling up
        player.ObjectSpeed *= 1.2f;
        
        weapon.rotationSpeed *= 1.2f;
        weaponGun.spawnInterval /= 1.2f;

        enemySpawner.spawnInterval /= 1.2f;

    }
}
