using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : MonoBehaviour
{
    [SerializeField] public GameObject bulletPrefab;  // The Bullet prefab to spawn
    [SerializeField] public float spawnInterval = 1.0f;  // Spawn every second
    [SerializeField] public float weaponDamage = 10f;
    [SerializeField] public float bulletSpeed = 100f;   // Speed of the bullet

    void Start()
    {
        StartCoroutine(SpawnBullet());
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Enemy collided");
            NewEnemy enemy = collision.GetComponent<NewEnemy>();
            if (enemy != null)
            {
                Debug.Log("Applying damage to enemy");
                enemy.TakeDamage(weaponDamage);
                Destroy(gameObject);  // Destroy the bullet after hitting an enemy
            }
        }
    }

    IEnumerator SpawnBullet()
    {
        while (true)
        {
            // Instantiate bullet and set its position and rotation
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

            // Assign velocity to the bullet to move it forward
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = transform.right * bulletSpeed;  // Bullet moves in the forward direction (right)
            }

            yield return new WaitForSeconds(spawnInterval);  // Wait before spawning the next bullet
        }
    }
}
