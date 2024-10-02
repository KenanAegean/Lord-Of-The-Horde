using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public static void SpawnDeathFX(Vector3 aPosition)
    {
        Instantiate(Resources.Load<GameObject>("DeathEffect"), aPosition, Quaternion.identity);
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
