using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PhysicsObject : MonoBehaviour
{
    [Header("Physics Object Attributes")]
    [SerializeField] protected Vector3 _target;
    [SerializeField] public float ObjectSpeed = 2.0f;
    [SerializeField] public bool ObjectAccelerates;
   

    public virtual void Update()
    {
        MoveTowardsTarget();
    }

    protected void MoveTowardsTarget()
    {
        var delta = ObjectSpeed * Time.deltaTime;
        if (ObjectAccelerates)
        {
            delta *= Vector3.Distance(transform.position, _target);
        }

        transform.position = Vector3.MoveTowards(transform.position, _target, delta);
    }

}

