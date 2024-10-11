using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    void Start()
    {
        cinemachineVirtualCamera.Follow = NewPlayer.Instance.transform;
    }

}