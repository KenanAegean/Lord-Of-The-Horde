using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : PhysicsObject, IPausable
{
    [Header("Inventory")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float followDistance = 5.0f;
    [SerializeField] private float searchRadius = 10.0f;
    [SerializeField] private float patrolRadius = 7.0f;
    [SerializeField] private float patrolInterval = 2.0f;
    [SerializeField] private float xpAmount = 25.0f;

    [Header("Other Attributes")]
    [SerializeField] private GameObject collectablePrefab;
    [SerializeField] private List<GameObject> damageStatusPrefabs;
    [SerializeField] private float Damage = 10f;

    private bool isDealingDamage = false;
    private Transform _player;
    private bool _playerFound = false;
    private Vector3 _nextPatrolPoint;
    private bool isPaused = false;

    private SpriteRenderer spriteRenderer;

    public enum CharacterState { Normal, LightDamage, MediumDamage, HeavyDamage }
    private CharacterState currentState;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        FindPlayer();
        StartCoroutine(Patrol());
        Collectible collectible = collectablePrefab.GetComponent<Collectible>();
        collectible.SetXPAmount(xpAmount);
    }

    public void OnPause() => isPaused = true;
    public void OnResume() => isPaused = false;

    public override void Update()
    {
        if (isPaused) return;

        if (!_playerFound) FindPlayer();
        FollowPlayerIfClose();
        base.Update();
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.transform;
            _playerFound = true;
        }
    }

    private void FollowPlayerIfClose()
    {
        if (_player != null)
        {
            float distance = Vector3.Distance(transform.position, _player.position);

            // Check if the player is within follow or search distance
            if (distance <= followDistance)
            {
                _target = _player.position;

                // Flip the sprite based on the player's position relative to the enemy
                if (_player.position.x < transform.position.x)
                {
                    spriteRenderer.flipX = false; // Player is to the left, flip enemy
                }
                else
                {
                    spriteRenderer.flipX = true; // Player is to the right, default facing direction
                }
            }
            else if (distance <= searchRadius)
            {
                _target = _nextPatrolPoint;
            }
            else
            {
                _player = null;
                _playerFound = false;
            }
        }
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            if (!_playerFound)
            {
                _nextPatrolPoint = GetRandomPatrolPoint();
                _target = _nextPatrolPoint;

                // Flip the sprite based on the patrol point relative to the enemy
                if (_nextPatrolPoint.x < transform.position.x)
                {
                    spriteRenderer.flipX = false; // Patrol point is to the left, flip enemy
                }
                else
                {
                    spriteRenderer.flipX = true; // Patrol point is to the right, default facing direction
                }

                yield return new WaitForSeconds(patrolInterval);
            }
            else yield return null;
        }
    }


    private Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomPoint;

        // Generate a patrol point
        do
        {
            randomPoint = transform.position + new Vector3(
                Random.Range(-patrolRadius, patrolRadius),
                Random.Range(-patrolRadius, patrolRadius),
                0
            );
        } while (Vector3.Distance(transform.position, randomPoint) < 2.0f);

        if (Vector3.Distance(transform.position, randomPoint) > searchRadius)
        {
            randomPoint = transform.position + (randomPoint - transform.position).normalized * searchRadius;
        }

        return randomPoint;
    }

    public void TakeDamage(float someDamage)
    {
        health -= someDamage;

        float healthPercentage = health / maxHealth;

        if (healthPercentage > 0.8f) ChangeDamagedSprite(CharacterState.Normal);
        else if (healthPercentage > 0.6f) ChangeDamagedSprite(CharacterState.LightDamage);
        else if (healthPercentage > 0.4f) ChangeDamagedSprite(CharacterState.MediumDamage);
        else if (healthPercentage > 0.2f) ChangeDamagedSprite(CharacterState.HeavyDamage);
        else if (health <= 0) Die();
    }

    private void ChangeDamagedSprite(CharacterState newState)
    {
        currentState = newState;

        // Change the sprite based on state
        switch (currentState)
        {
            case CharacterState.Normal:
                if (damageStatusPrefabs.Count > 0) AssignSpriteFromPrefab(0);
                break;
            case CharacterState.LightDamage:
                if (damageStatusPrefabs.Count > 1) AssignSpriteFromPrefab(1);
                break;
            case CharacterState.MediumDamage:
                if (damageStatusPrefabs.Count > 2) AssignSpriteFromPrefab(2);
                break;
            case CharacterState.HeavyDamage:
                if (damageStatusPrefabs.Count > 3) AssignSpriteFromPrefab(3);
                break;
            default:
                Debug.LogError("Unknown CharacterState: " + currentState);
                break;
        }
    }

    private void AssignSpriteFromPrefab(int index)
    {
        if (damageStatusPrefabs.Count > index)
        {
            SpriteRenderer prefabSpriteRenderer = damageStatusPrefabs[index].GetComponent<SpriteRenderer>();
            if (prefabSpriteRenderer != null)
            {
                spriteRenderer.sprite = prefabSpriteRenderer.sprite;
                spriteRenderer.color = prefabSpriteRenderer.color;
            }
            else Debug.LogError("No SpriteRenderer found on prefab: " + damageStatusPrefabs[index].name);
        }
        else Debug.LogError("Invalid index for damageStatusPrefabs list: " + index);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PlayerBody"))
        {
            NewPlayer player = collision.collider.GetComponentInParent<NewPlayer>();
            if (player != null && !isDealingDamage) StartCoroutine(DealDamageOverTime(player));
        }
    }

    private IEnumerator DealDamageOverTime(NewPlayer player)
    {
        isDealingDamage = true;
        while (true)
        {
            if (!isPaused)
            {
                player.TakeDamage(Damage);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PlayerBody"))
        {
            StopAllCoroutines();
            isDealingDamage = false;
        }
    }

    public void Die()
    {
        Instantiate(collectablePrefab, transform.position, Quaternion.identity);
        Effects.SpawnDeathFX(transform.position);
        Destroy(gameObject);
    }
}
