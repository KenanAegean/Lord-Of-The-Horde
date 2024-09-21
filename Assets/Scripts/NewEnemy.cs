using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : PhysicsObject
{
    [Header("Inventory")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float followDistance = 5.0f;
    [SerializeField] private float searchRadius = 10.0f; // Radius to search for the player
    [SerializeField] private float patrolRadius = 7.0f; // Radius for random patrol around spawn point
    [SerializeField] private float patrolInterval = 2.0f; // Time between patrol movements

    [Header("Other Attributes")]
    [SerializeField] private GameObject collectablePrefab;
    [SerializeField] private List<GameObject> damageStatusPrefabs;

    private Transform _player;
    private bool _playerFound = false;
    private Vector3 _nextPatrolPoint;

    private SpriteRenderer spriteRenderer; // Current enemy's SpriteRenderer component

    public enum CharacterState
    {
        Normal,
        LightDamage,
        MediumDamage,
        HeavyDamage
    }

    private CharacterState currentState;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        FindPlayer();
        StartCoroutine(Patrol());

        //ChangeDamagedSprite(CharacterState.Normal);
    }

    public override void Update()
    {
        if (!_playerFound)
        {
            FindPlayer(); 
        }

        FollowPlayerIfClose();

        base.Update(); // Call parents MoveTowardsTarget method

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

            if (distance <= followDistance)
            {
                _target = _player.position;  
                _target.z = 0;  
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

                yield return new WaitForSeconds(patrolInterval);
            }
            else
            {
                yield return null;
            }
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomPoint;

        // Ensure patrol point is far enough from current position to prevent gatherin
        // There is bug here sometimes enemies can gather in the center??
        do
        {
            randomPoint = transform.position + new Vector3(
                Random.Range(-patrolRadius, patrolRadius),
                Random.Range(-patrolRadius, patrolRadius),
                0
            );
        }
        while (Vector3.Distance(transform.position, randomPoint) < 2.0f);  // Ensure patrol point is far enough 

        // Ensure patrol point stays within searchRadius
        if (Vector3.Distance(transform.position, randomPoint) > searchRadius)
        {
            randomPoint = transform.position + (randomPoint - transform.position).normalized * searchRadius;
        }

        return randomPoint;
    }

    public void TakeDamage(float someDamage)
    {
        Debug.Log("Enemy took damage");
        health -= someDamage;

        float healthPercentage = health / maxHealth;

        if (healthPercentage > 0.8f)
        {
            Debug.Log("Enemy Status Normal");
            ChangeDamagedSprite(CharacterState.Normal);
        }
        else if (healthPercentage > 0.6f)
        {
            Debug.Log("Enemy Status LightDamage");
            ChangeDamagedSprite(CharacterState.LightDamage);
        }
        else if (healthPercentage > 0.4f)
        {
            Debug.Log("Enemy Status MediumDamage");
            ChangeDamagedSprite(CharacterState.MediumDamage);
        }
        else if (healthPercentage > 0.2f)
        {
            Debug.Log("Enemy Status HeavyDamage");
            ChangeDamagedSprite(CharacterState.HeavyDamage);
        }
        else if (health <= 0)
        {
            Die();
        }
    }

    private void ChangeDamagedSprite(CharacterState newState)
    {
        currentState = newState;

        // Select one of the prefabs based on the state and assign its sprite to the enemy
        switch (currentState)
        {
            case CharacterState.Normal:
                if (damageStatusPrefabs != null && damageStatusPrefabs.Count > 0)
                    AssignSpriteFromPrefab(0);
                break;
            case CharacterState.LightDamage:
                if (damageStatusPrefabs != null && damageStatusPrefabs.Count > 1)
                    AssignSpriteFromPrefab(1);
                break;
            case CharacterState.MediumDamage:
                if (damageStatusPrefabs != null && damageStatusPrefabs.Count > 2)
                    AssignSpriteFromPrefab(2);
                break;
            case CharacterState.HeavyDamage:
                if (damageStatusPrefabs != null && damageStatusPrefabs.Count > 3)
                    AssignSpriteFromPrefab(3);
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
            // Get the SpriteRenderer from the prefab and assign its sprite to the current enemy
            SpriteRenderer prefabSpriteRenderer = damageStatusPrefabs[index].GetComponent<SpriteRenderer>();
            if (prefabSpriteRenderer != null)
            {
                Debug.Log("Enemy Sprite Changed");
                spriteRenderer.sprite = prefabSpriteRenderer.sprite;
                spriteRenderer.color = prefabSpriteRenderer.color; //opsional
            }
            else
            {
                Debug.LogError("No SpriteRenderer found on prefab: " + damageStatusPrefabs[index].name);
            }
        }
        else
        {
            Debug.LogError("Invalid index for damageStatusPrefabs list: " + index);
        }
    }

    public void Die()
    {
        Vector3 enemyLastPosition = transform.position;
        Destroy(gameObject);
        Debug.Log("Enemy died");
        Instantiate(collectablePrefab, enemyLastPosition, Quaternion.identity);
    }


}
