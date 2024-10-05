using UnityEngine;
using System.Collections.Generic;

public class PlayerInitializer : MonoBehaviour
{
    [Header("Player Details")]
    public string playerName;
    [TextArea] public string playerDescription;

    [Header("Initial Stats")]
    public float maxHealth;
    public float startHealth;
    public float startXP;
    public float xpToNextLevel;

    // Optional: List of starting secondary weapons
    public List<Weapon> secondaryWeapons;
}
