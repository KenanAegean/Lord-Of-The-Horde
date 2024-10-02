using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private float xpAmount = 25f; // XP this collectible gives

    public float GetXPAmount()
    {
        return xpAmount;
    }
}
