using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private NewPlayer player;

    [SerializeField] private Weapon weapon;
    [SerializeField] private Weapon weaponGun;
    [SerializeField] private Weapon weaponPunch;
    [SerializeField] private Weapon weaponPunchSecond;
    [SerializeField] private EnemySpawner enemySpawner;

    private void Start()
    {
        player = NewPlayer.Instance;
        InitalValues();
    }

    public void InitalValues()
    {
        weaponPunch.gameObject.SetActive(true);
        weaponPunchSecond.gameObject.SetActive(false);
        weaponGun.gameObject.SetActive(false);

        weapon.rotationSpeed = 100.0f;

        enemySpawner.spawnInterval = 1.2f;

        player.playerLevel = 0;
        player.xpToNextLevel = 50.0f;
    }

    public void UpdatePlayerStats()
    {
        Effects.LeveltUpFX(player.transform);
        player.playerLevel++;

        if (player.playerLevel == 1)
        {
            weaponPunchSecond.gameObject.SetActive(true);
        }
        else if (player.playerLevel == 2)
        {
            weaponPunchSecond.gameObject.SetActive(false);
            weaponGun.gameObject.SetActive(true);
        }

        player.currentXP -= player.xpToNextLevel;
        player.xpToNextLevel *= 1.5f;
        player.maxHealth += 10f;
        player.health += 15f;
        player.ObjectSpeed *= 1.3f;

        weapon.rotationSpeed *= 1.5f;
        weaponGun.spawnInterval /= 1.5f;

        enemySpawner.spawnInterval /= 1.5f;
    }
}
