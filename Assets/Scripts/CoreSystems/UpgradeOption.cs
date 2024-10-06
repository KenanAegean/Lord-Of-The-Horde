using UnityEngine;

public enum UpgradeType
{
    HealthIncrease,
    SpeedIncrease,
    WeaponActivation,
    OrbitalSpeed,
    OrbitDirection
}

[System.Serializable]
public class UpgradeOption
{
    public string upgradeName;
    public UpgradeType type;
    public string description; // Add a description field
    public float value; // Amount to increase health or speed
    public Weapon weaponToActivate; // The weapon to activate if it's a weapon upgrade
    public Sprite icon; // The icon for this upgrade

    // Default icons for different types
    private static Sprite defaultHealthIcon;
    private static Sprite defaultSpeedIcon;
    private static Sprite defaultWeaponIcon;
    private static Sprite defaultOrbitIcon;
    private static Sprite defaultOrbitDIcon;

    // Method to get the correct icon (either the assigned one or a default one based on type)
    public Sprite GetIcon()
    {
        // If a custom icon is assigned, use it
        if (icon != null)
        {
            return icon;
        }

        // Assign default icons based on upgrade type
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
                return null;
        }
    }

    // Static method to set the default icons (called once during setup)
    public static void SetDefaultIcons(Sprite healthIcon, Sprite speedIcon, Sprite weaponIcon, Sprite orbitIcon, Sprite orbitDIcon)
    {
        defaultHealthIcon = healthIcon;
        defaultSpeedIcon = speedIcon;
        defaultWeaponIcon = weaponIcon;
        defaultOrbitIcon = orbitIcon;
        defaultOrbitDIcon = orbitDIcon;
    }

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
                return $"Change the weapons direction.";
            default:
                return "Upgrade your abilities.";
        }
    }
}