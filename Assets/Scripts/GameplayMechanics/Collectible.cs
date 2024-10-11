using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private float xpAmount = 25f;

    public float GetXPAmount()
    {
        return xpAmount;
    }

    public void SetXPAmount(float value)
    { 
       xpAmount = value; 
    }
}
