using UnityEngine;
using System.Collections.Generic;

public class PlayerInitializer : MonoBehaviour
{
    [Header("Player Details")]
    [SerializeField] public string playerName;
    [TextArea] public string playerDescription;

    [Header("Initial Stats")]
    [SerializeField] public float maxHealth;
    [SerializeField] public float startHealth;
    [SerializeField] public float startXP;
    [SerializeField] public float xpToNextLevel;

    [SerializeField] public float speed;
    [SerializeField] public float orbitalSpeed;


    [SerializeField] public List<Weapon> secondaryWeapons;
}
