using UnityEngine;

public enum UpgradeType
{
    HealthIncrease,
    SpeedIncrease,
    WeaponActivation,
    SpeedyIncrease
}

[System.Serializable]
public class UpgradeOption
{
    public string upgradeName;
    public UpgradeType type;
    public string description; // Add a description field
    public float value; // Amount to increase health or speed
    public Weapon weaponToActivate; // The weapon to activate if it's a weapon upgrade

    public void ApplyUpgrade(NewPlayer player)
    {
        switch (type)
        {
            case UpgradeType.HealthIncrease:
                player.maxHealth += value;
                player.health += value;
                break;
            case UpgradeType.SpeedIncrease:
                player.ObjectSpeed += value;
                break;
            case UpgradeType.WeaponActivation:
                if (weaponToActivate != null)
                {
                    weaponToActivate.gameObject.SetActive(true);
                }
                break;
            case UpgradeType.SpeedyIncrease:
                player.ObjectSpeed += value;
                break;
        }
    }
}