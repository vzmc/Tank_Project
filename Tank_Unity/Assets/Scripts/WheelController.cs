using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] private Transform visualWheel;
    [SerializeField] private WheelCollider wheelCollider;

    private void FixedUpdate()
    {
        wheelCollider.GetWorldPose(out var position, out var rotation);
        visualWheel.position = position;
        visualWheel.rotation = rotation;
    }
}
