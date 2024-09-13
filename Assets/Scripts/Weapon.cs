using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // General Weapon Settings
    [Header("General Settings")]
    [SerializeField] private Transform player;          // The player the weapon orbits around
    [SerializeField] private float rotationSpeed = 120f; // Speed of rotation around the player
    [SerializeField] private bool isTwoHandWeapon = true; // True if it's a two-hand weapon
    [SerializeField] private bool isGunWeapon = false;  // True if the weapon can shoot bullets

    // Melee (Punch) Settings
    [Header("Melee Settings")]
    [SerializeField] private float meleeDamage = 10f;   // Damage dealt when colliding with an enemy

    // Gun (Shooting) Settings
    [Header("Gun Settings")]
    [SerializeField] private GameObject bulletPrefab;   // The bullet prefab for shooting
    [SerializeField] private float bulletSpeed = 100f;  // Speed of the bullet
    [SerializeField] private float spawnInterval = 1.0f; // Time between bullet shots
    [SerializeField] private float bulletDamage = 10f;  // Damage dealt by the bullet

    private bool canShoot = true;  // Flag to control shooting behavior

    void Start()
    {
        // Start bullet spawning only if the weapon is a gun
        if (isGunWeapon)
        {
            StartCoroutine(SpawnBullet());
        }
    }

    void Update()
    {
        // Rotate the weapon around the player
        if (player != null)
        {
            RotateAroundPlayer();
        }
    }

    private void RotateAroundPlayer()
    {
        // Rotate the weapon around the player's center
        transform.RotateAround(player.position, Vector3.forward, rotationSpeed * Time.deltaTime * -1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            NewEnemy enemy = collision.GetComponent<NewEnemy>();
            if (enemy != null)
            {
                // Deal melee damage (punch) on contact
                enemy.TakeDamage(meleeDamage);
            }
        }
    }

    IEnumerator SpawnBullet()
    {
        while (true)
        {
            if (canShoot)
            {
                // Instantiate bullet and set its position and rotation
                GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

                // Assign velocity to the bullet to move it forward
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = transform.right * bulletSpeed;  // Moves the bullet in the "right" direction
                }

                // Set the bullet damage via a method
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetDamage(bulletDamage);  // Pass bullet damage to the bullet script
                }
            }

            yield return new WaitForSeconds(spawnInterval);  // Wait before spawning the next bullet
        }
    }
}
