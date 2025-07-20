using UnityEngine;

public class UpgradePrefab : MonoBehaviour
{
    [Header("Upgrade Details")]
    public string upgradeName;
    public UpgradeType type;
    [TextArea] public string description;
    public float value;
    public float speedValue;  

    [Header("Weapon Upgrade (Optional)")]
    public GameObject weaponPrefab;
    public Sprite icon;

    public static Sprite defaultHealthIcon;
    public static Sprite defaultSpeedIcon;
    public static Sprite defaultWeaponIcon;
    public static Sprite defaultOrbitIcon;
    public static Sprite defaultOrbitDIcon;
    public static Sprite defaultMagnetIcon;

    public static void SetDefaultIcons(Sprite healthIcon, Sprite speedIcon, Sprite weaponIcon, 
        Sprite orbitIcon, Sprite orbitDIcon, Sprite magnetIcon)
    {
        defaultHealthIcon = healthIcon;
        defaultSpeedIcon = speedIcon;
        defaultWeaponIcon = weaponIcon;
        defaultOrbitIcon = orbitIcon;
        defaultOrbitDIcon = orbitDIcon;
        defaultMagnetIcon = magnetIcon;
    }

    public Sprite GetIcon()
    {
        if (icon != null)
        {
            return icon;
        }

        switch (type)
        {
            case UpgradeType.HealthIncrease: return defaultHealthIcon;
            case UpgradeType.SpeedIncrease: return defaultSpeedIcon;
            case UpgradeType.WeaponActivation: return defaultWeaponIcon;
            case UpgradeType.OrbitalSpeed: return defaultOrbitIcon;
            case UpgradeType.OrbitDirection: return defaultOrbitDIcon;
            case UpgradeType.Magnet: return defaultMagnetIcon;
            default: return null;
        }
    }

    public string GetDescription()
    {
        if (!string.IsNullOrEmpty(description))
        {
            return description;
        }

        switch (type)
        {
            case UpgradeType.HealthIncrease: return $"Add +{value} health to the player.";
            case UpgradeType.SpeedIncrease: return $"Increase player speed by +{value}.";
            case UpgradeType.WeaponActivation: return "Unlock a new weapon!";
            case UpgradeType.OrbitalSpeed: return $"Increase weapon speed by x{value}.";
            case UpgradeType.OrbitDirection: return $"Change the weapon's orbit direction and increase weapon speed by +{speedValue}.";
            case UpgradeType.Magnet: return $"Magnet: +{value:F0} radius, +{speedValue:F0} pull speed per upgrade.";
            default:
                return "Upgrade your abilities.";
        }
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
                if (weaponPrefab != null)
                {
                    levelManager.EquipWeaponInSlot(weaponPrefab);
                }
                break;
            case UpgradeType.OrbitalSpeed:
                weapon.rotationSpeed *= value;
                break;
            case UpgradeType.OrbitDirection:
                weapon.rotationSpeed += speedValue;
                weapon.rotationSpeed *= value;
                break;
            case UpgradeType.Magnet:
                player.EnableMagnet(value, speedValue);
                break;
        }
    }
}
