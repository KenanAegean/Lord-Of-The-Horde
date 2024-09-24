using UnityEngine;

public class Bullet : MonoBehaviour, IPausable
{
    private float damage = 10f;  // Default damage value
    private bool isPaused = false;
    private Rigidbody2D rb;
    private Vector2 storedVelocity;  // To store the bullet's velocity when paused

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found on Bullet!");
        }
    }

    public void OnPause()
    {
        isPaused = true;

        if (rb != null)
        {
            storedVelocity = rb.velocity;  // Store the current velocity
            rb.velocity = Vector2.zero;    // Stop the bullet
            rb.isKinematic = true;         // Set Rigidbody to kinematic to avoid any physics interactions
        }
    }

    public void OnResume()
    {
        isPaused = false;

        if (rb != null)
        {
            rb.isKinematic = false;       // Restore Rigidbody to dynamic
            rb.velocity = storedVelocity; // Restore the stored velocity
        }
    }

    private void Update()
    {
        if (isPaused) return;

        // Regular bullet update logic can go here (if any)
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;  // Set the bullet damage
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            NewEnemy enemy = collision.GetComponent<NewEnemy>();
            if (enemy != null)
            {
                // Deal damage to the enemy on bullet impact
                enemy.TakeDamage(damage);
                Destroy(gameObject);  // Destroy the bullet after hitting an enemy
            }
        }
    }
}
