using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private float xpAmount = 25f;
    
    //–– Magnet fields
    private Transform magnetTarget;
    private float magnetSpeed;
    private float magnetRadius;
    private const float arrivalThreshold = 0.1f;

    public float GetXPAmount() => xpAmount;
    public void SetXPAmount(float value) => xpAmount = value;

    /// <summary>
    /// Start attracting this collectible.
    /// </summary>
    public void StartMagnet(Transform target, float speed, float radius)
    {
        magnetTarget = target;
        magnetSpeed  = speed;
        magnetRadius = radius;
    }

    private void Update()
    {
        if (magnetTarget == null) return;

        float dist = Vector3.Distance(transform.position, magnetTarget.position);
        if (dist > magnetRadius)
        {
            // out of range—stop following
            magnetTarget = null;
            return;
        }

        // move in
        transform.position = Vector3.MoveTowards(
            transform.position,
            magnetTarget.position,
            magnetSpeed * Time.deltaTime
        );

        // arrived!
        if (dist < arrivalThreshold)
        {
            var player = magnetTarget.GetComponent<NewPlayer>();
            if (player != null)
                player.CollectXP(xpAmount);
            Destroy(gameObject);
        }
    }
}
