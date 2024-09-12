using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public Transform player;  // The player to orbit around
    [SerializeField] public float rotationSpeed = 120f; // Speed of rotation in degrees per second
    [SerializeField] public float weaponDemage = 50f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // Rotate the weapon around the player's center
            transform.RotateAround(player.position, Vector3.forward, rotationSpeed * Time.deltaTime * -1);
        }
    }
}
