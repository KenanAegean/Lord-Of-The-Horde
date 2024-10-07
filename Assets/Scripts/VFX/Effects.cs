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
        GameObject deathEffect = Instantiate(Resources.Load<GameObject>("DeathEffect"), position, Quaternion.identity);

        ParticleSystem particleSystem = deathEffect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            var mainModule = particleSystem.main;
            mainModule.startColor = effectColor;
        }
    }

    public static GameObject LeveltUpFX(Transform playerTransform)
    {
        GameObject levelEffect = Instantiate(Resources.Load<GameObject>("LevelEffect"), playerTransform.position, Quaternion.identity);
        
        levelEffect.transform.SetParent(playerTransform);
        levelEffect.transform.localScale = Vector3.one;
        
        return levelEffect;
    }
}
