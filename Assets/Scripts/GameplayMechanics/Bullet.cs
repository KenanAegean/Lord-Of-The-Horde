using UnityEngine;

public class Bullet : MonoBehaviour, IPausable
{
    private float damage = 10f;
    private bool isPaused = false;
    private Rigidbody2D rb;
    private Vector2 storedVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found on Bullet!");
        }

        Destroy(gameObject, 3f);
    }

    public void OnPause()
    {
        isPaused = true;

        if (rb != null)
        {
            storedVelocity = rb.velocity;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }

    public void OnResume()
    {
        isPaused = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = storedVelocity;
        }
    }

    private void Update()
    {
        if (isPaused) return;
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            NewEnemy enemy = collision.GetComponent<NewEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
