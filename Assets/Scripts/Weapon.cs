using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public Transform player;  // The player to orbit around
    [SerializeField] public float rotationSpeed = 120f; // Speed of rotation in degrees per second
    [SerializeField] public float weaponDemage = 50f;
    [SerializeField] public bool isTwoHandWeapon;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (isTwoHandWeapon)
            {
                TwoHandWeapon();
            }
        }
    }

    private void TwoHandWeapon()
    {
        // Rotate the weapon around the player's center
        transform.RotateAround(player.position, Vector3.forward, rotationSpeed * Time.deltaTime * -1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("colisiooun");
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("enemy collide");
            NewEnemy e = null;
            if (collision.TryGetComponent(out e))
            {
                e.TakeDamage(50f);
            }
        }
    }
}
