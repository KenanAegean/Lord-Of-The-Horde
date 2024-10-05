using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour, IPausable
{
    [Header("General Settings")]
    [SerializeField] private Transform player;
    [SerializeField] public float rotationSpeed = 120f;
    [SerializeField] private bool isGunWeapon = false;

    [Header("Melee Settings")]
    [SerializeField] private float meleeDamage = 10f;

    [Header("Gun Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] public float spawnInterval = 1.0f;
    [SerializeField] private float bulletDamage = 10f;

    private bool isPaused = false;
    private Coroutine shootingCoroutine;

    void Start()
    {
        if (isGunWeapon) StartShooting();
    }

    public void OnPause() => isPaused = true;

    public void OnResume() => isPaused = false;

    void Update()
    {
        if (isPaused) return;
        if (player != null) RotateAroundPlayer();
    }

    private void RotateAroundPlayer()
    {
        transform.RotateAround(player.position, Vector3.forward, rotationSpeed * Time.deltaTime * -1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            NewEnemy enemy = collision.GetComponent<NewEnemy>();
            if (enemy != null) enemy.TakeDamage(meleeDamage);
        }
    }

    // Start shooting (start the coroutine)
    public void StartShooting()
    {
        if (isGunWeapon && shootingCoroutine == null)
        {
            shootingCoroutine = StartCoroutine(SpawnBullet());
        }
    }

    // Stop shooting (stop the coroutine)
    public void StopShooting()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    IEnumerator SpawnBullet()
    {
        while (true)
        {
            while (isPaused) yield return null;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = transform.right * bulletSpeed;

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null) bulletScript.SetDamage(bulletDamage);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
