using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] Vector3 _target;
    [SerializeField] GameObject _targetPlayer;
    [SerializeField] public Camera Camera;
    [SerializeField] public bool FollowMouse;
    [SerializeField] public bool FollowPlayer;
    [SerializeField] public bool ShipAccelerates;
    [SerializeField] public float ShipSpeed = 2.0f;

    void Start()
    {

    }


    public void OnEnable()
    {
        if (Camera == null)
        {
            throw new System.InvalidOperationException("Camera not set");
        }
    }

    public void Update()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (FollowMouse || Input.GetMouseButton(1))
        {
            _target = Camera.ScreenToWorldPoint(Input.mousePosition);
            _target.z = 0;
        }
        else if (FollowPlayer)
        {
            _target = Camera.ScreenToWorldPoint(Input.mousePosition);
            _target.z = 0;
        }

        var delta = ShipSpeed * Time.deltaTime;
        if (ShipAccelerates)
        {
            delta *= Vector3.Distance(transform.position, _target);
        }

        transform.position = Vector3.MoveTowards(transform.position, _target, delta);
    }


}
