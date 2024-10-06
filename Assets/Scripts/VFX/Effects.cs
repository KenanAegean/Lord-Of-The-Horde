using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public static void SpawnDeathFX(Vector3 position)
    {
        SpawnDeathFX(position, Color.white);
    }

    public static void SpawnDeathFX(Vector3 position, Color effectColor)
    {
        // Instantiate the effect
        GameObject deathEffect = Instantiate(Resources.Load<GameObject>("DeathEffect"), position, Quaternion.identity);

        // Get the ParticleSystem component
        ParticleSystem particleSystem = deathEffect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            // Access the main module to change the start color
            var mainModule = particleSystem.main;
            mainModule.startColor = effectColor;
        }
    }

    public static GameObject LeveltUpFX(Transform playerTransform)
    {
        GameObject levelEffect = Instantiate(Resources.Load<GameObject>("LevelEffect"), playerTransform.position, Quaternion.identity);
        // Set the player as the parent so the effect follows the player.
        levelEffect.transform.SetParent(playerTransform);
        // Reset the local scale to avoid inheriting the player's scale.
        levelEffect.transform.localScale = Vector3.one;
        return levelEffect;
    }
}
