using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage = 10f;  // Default damage value

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
