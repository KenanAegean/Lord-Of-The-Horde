using UnityEngine;

public class UpgradePrefab : MonoBehaviour
{
    [Header("Upgrade Details")]
    public string upgradeName;
    public UpgradeType type;
    [TextArea] public string description;
    public float value;

    [Header("Weapon Upgrade (Optional)")]
    public GameObject weaponPrefab;

    public Sprite icon; // The icon for this upgrade

    // Default icons for different upgrade types
    public static Sprite defaultHealthIcon;
    public static Sprite defaultSpeedIcon;
    public static Sprite defaultWeaponIcon;
    public static Sprite defaultOrbitIcon;
    public static Sprite defaultOrbitDIcon;

    // Assign default icons during initialization (static method to be called when setting defaults)
    public static void SetDefaultIcons(Sprite healthIcon, Sprite speedIcon, Sprite weaponIcon, Sprite orbitIcon, Sprite orbitDIcon)
    {
        defaultHealthIcon = healthIcon;
        defaultSpeedIcon = speedIcon;
        defaultWeaponIcon = weaponIcon;
        defaultOrbitIcon = orbitIcon;
        defaultOrbitDIcon = orbitDIcon;
    }

    // Method to get the correct icon (either the assigned one or a default one based on type)
    public Sprite GetIcon()
    {
        // If a custom icon is assigned, use it
        if (icon != null)
        {
            return icon;
        }

        // Use default icons based on upgrade type
        switch (type)
        {
            case UpgradeType.HealthIncrease:
                return defaultHealthIcon;
            case UpgradeType.SpeedIncrease:
                return defaultSpeedIcon;
            case UpgradeType.WeaponActivation:
                return defaultWeaponIcon;
            case UpgradeType.OrbitalSpeed:
                return defaultOrbitIcon;
            case UpgradeType.OrbitDirection:
                return defaultOrbitDIcon;
            default:
                return null; // Return null if no matching type or default icon exists
        }
    }

    // Method to get the description dynamically if not manually provided
    public string GetDescription()
    {
        // If a manual description is provided, return it
        if (!string.IsNullOrEmpty(description))
        {
            return description;
        }

        // Generate a description based on the upgrade type
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
                return $"Change the weapon's orbit direction.";
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
                if (weaponPrefab != null)
                {
                    levelManager.EquipSecondaryWeapon(weaponPrefab);
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
