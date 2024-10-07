using UnityEngine;

public class UpgradePrefab : MonoBehaviour
{
    [Header("Upgrade Details")]
    public string upgradeName;
    public UpgradeType type;
    [TextArea] public string description;
    public float value; // The amount for health, speed, or weapon effects
    public Weapon weaponToActivate; // The weapon to activate for WeaponActivation type
    public Sprite icon; // The icon for the upgrade

    // Returns the icon for this upgrade
    public Sprite GetIcon()
    {
        return icon;
    }

    // Returns a description for the upgrade
    public string GetDescription()
    {
        if (!string.IsNullOrEmpty(description))
            return description;

        // Generate a default description based on the type and value
        switch (type)
        {
            case UpgradeType.HealthIncrease:
                return $"Add +{value} health to the player.";
            case UpgradeType.SpeedIncrease:
                return $"Increase player speed by +{value}.";
            case UpgradeType.WeaponActivation:
                return "Unlock a new weapon!";
            case UpgradeType.OrbitalSpeed:
                return $"Increase weapon speed by x{value}.";
            case UpgradeType.OrbitDirection:
                return $"Change the weapons direction.";
            default:
                return "Upgrade your abilities.";
        }
    }

    // Applies the upgrade to the player
    public void ApplyUpgrade(NewPlayer player, Weapon weapon, LevelManager levelManager)
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
                    levelManager.ActivateSecondaryWeapon(weaponToActivate);
                }
                break;
            case UpgradeType.OrbitalSpeed:
                weapon.rotationSpeed *= value;
                break;
            case UpgradeType.OrbitDirection:
                weapon.rotationSpeed *= value;
                break;
        }
    }
}
